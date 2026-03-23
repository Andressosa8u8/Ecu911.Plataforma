import { useEffect, useMemo, useState } from "react";
import {
  createDocumentItem,
  downloadDocumentFile,
  getAllDocuments,
  getChildNodes,
  getDocumentTypes,
  getRootNodes,
  hasDocumentFile,
  uploadDocumentFile,
} from "../api/catalogApi";
import { useAuth } from "../auth/AuthContext";
import type {
  CreateDocumentItemRequest,
  DocumentItem,
  DocumentType,
  RepositoryNode,
} from "../types/catalog";

type NodeMap = Record<string, RepositoryNode[]>;
type FileAvailabilityMap = Record<string, boolean>;

export default function NodesPage() {
  const { token, user } = useAuth();

  const [rootNodes, setRootNodes] = useState<RepositoryNode[]>([]);
  const [childrenMap, setChildrenMap] = useState<NodeMap>({});
  const [expandedNodes, setExpandedNodes] = useState<string[]>([]);
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null);
  const [documents, setDocuments] = useState<DocumentItem[]>([]);
  const [documentTypes, setDocumentTypes] = useState<DocumentType[]>([]);
  const [fileAvailability, setFileAvailability] = useState<FileAvailabilityMap>({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [isCreating, setIsCreating] = useState(false);
  const [uploadingDocId, setUploadingDocId] = useState<string | null>(null);
  const [selectedFileByDoc, setSelectedFileByDoc] = useState<Record<string, File | null>>({});

  const [newTitle, setNewTitle] = useState("");
  const [newDescription, setNewDescription] = useState("");
  const [newDocumentTypeId, setNewDocumentTypeId] = useState("");

  useEffect(() => {
    async function loadInitialData() {
      if (!token) return;

      try {
        setLoading(true);
        setError("");

        const [nodesData, documentsData, documentTypesData] = await Promise.all([
          getRootNodes(token),
          getAllDocuments(token),
          getDocumentTypes(token),
        ]);

        setRootNodes(nodesData);
        setDocuments(documentsData);
        setDocumentTypes(documentTypesData);

        if (documentTypesData.length > 0) {
          setNewDocumentTypeId(documentTypesData[0].id);
        }
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "No se pudo cargar la información del repositorio."
        );
      } finally {
        setLoading(false);
      }
    }

    loadInitialData();
  }, [token]);

  async function handleToggleNode(nodeId: string) {
    if (!token) return;

    const isExpanded = expandedNodes.includes(nodeId);

    if (isExpanded) {
      setExpandedNodes((prev) => prev.filter((id) => id !== nodeId));
      return;
    }

    if (!childrenMap[nodeId]) {
      try {
        const children = await getChildNodes(nodeId, token);
        setChildrenMap((prev) => ({
          ...prev,
          [nodeId]: children,
        }));
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "No se pudieron cargar los hijos del nodo."
        );
        return;
      }
    }

    setExpandedNodes((prev) => [...prev, nodeId]);
  }

  async function handleDownload(documentId: string) {
    if (!token) return;

    try {
      setError("");
      await downloadDocumentFile(documentId, token);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "No se pudo descargar el archivo."
      );
    }
  }

  async function handleUpload(documentId: string) {
    if (!token) return;

    const file = selectedFileByDoc[documentId];
    if (!file) {
      setError("Debe seleccionar un archivo antes de subirlo.");
      return;
    }

    try {
      setUploadingDocId(documentId);
      setError("");

      await uploadDocumentFile(documentId, file, token);

      setFileAvailability((prev) => ({
        ...prev,
        [documentId]: true,
      }));

      setSelectedFileByDoc((prev) => ({
        ...prev,
        [documentId]: null,
      }));
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "No se pudo subir el archivo."
      );
    } finally {
      setUploadingDocId(null);
    }
  }

  async function handleCreateDocument(e: React.FormEvent) {
    e.preventDefault();

    if (!token || !selectedNodeId) return;

    if (!newTitle.trim()) {
      setError("El título del documento es obligatorio.");
      return;
    }

    if (!newDocumentTypeId) {
      setError("Debe seleccionar un tipo documental.");
      return;
    }

    try {
      setIsCreating(true);
      setError("");

      const payload: CreateDocumentItemRequest = {
        title: newTitle.trim(),
        description: newDescription.trim() || null,
        repositoryNodeId: selectedNodeId,
        documentTypeId: newDocumentTypeId,
      };

      const created = await createDocumentItem(payload, token);

      setDocuments((prev) => [created, ...prev]);
      setFileAvailability((prev) => ({
        ...prev,
        [created.id]: false,
      }));

      setNewTitle("");
      setNewDescription("");
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "No se pudo crear el documento."
      );
    } finally {
      setIsCreating(false);
    }
  }

  const selectedDocuments = useMemo(() => {
    if (!selectedNodeId || !Array.isArray(documents)) {
      return [];
    }

    return documents.filter((doc) => doc.repositoryNodeId === selectedNodeId);
  }, [documents, selectedNodeId]);

  useEffect(() => {
    async function loadFileAvailability() {
      if (!token || selectedDocuments.length === 0) {
        return;
      }

      const pendingDocs = selectedDocuments.filter(
        (doc) => fileAvailability[doc.id] === undefined
      );

      if (pendingDocs.length === 0) {
        return;
      }

      try {
        const results = await Promise.all(
          pendingDocs.map(async (doc) => ({
            id: doc.id,
            hasFile: await hasDocumentFile(doc.id, token),
          }))
        );

        setFileAvailability((prev) => {
          const next = { ...prev };
          for (const item of results) {
            next[item.id] = item.hasFile;
          }
          return next;
        });
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "No se pudo verificar si los documentos tienen archivo."
        );
      }
    }

    loadFileAvailability();
  }, [token, selectedDocuments, fileAvailability]);

  const canManage =
    user?.roles.includes("ADMIN") || user?.roles.includes("GESTOR_DOCUMENTAL");

  function renderNodes(nodes: RepositoryNode[], level = 0) {
    return (
      <ul className={level === 0 ? "node-list" : "node-children"}>
        {nodes.map((node) => {
          const isExpanded = expandedNodes.includes(node.id);
          const children = childrenMap[node.id] || [];

          return (
            <li key={node.id} style={{ marginBottom: 10 }}>
              <button
                type="button"
                className={`node-row ${selectedNodeId === node.id ? "selected" : ""}`}
                onClick={() => setSelectedNodeId(node.id)}
              >
                <span
                  className="node-toggle"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleToggleNode(node.id);
                  }}
                >
                  {isExpanded ? "-" : "+"}
                </span>

                <span className="node-text">
                  <strong>{node.name}</strong>
                  <span>{node.description || "Sin descripción"}</span>
                </span>
              </button>

              {isExpanded && children.length > 0 && renderNodes(children, level + 1)}
            </li>
          );
        })}
      </ul>
    );
  }

  return (
    <div>
      {error && <div className="error-box">{error}</div>}

      {loading && <p className="muted">Cargando información...</p>}

      {!loading && !error && rootNodes.length === 0 && (
        <div className="empty-box">No existen nodos raíz registrados.</div>
      )}

      {!loading && rootNodes.length > 0 && (
        <div className="page-grid">
          <section className="card panel">
            <h2 className="panel-title">Estructura del repositorio</h2>
            <p className="muted">
              Navega por los nodos y selecciona la ubicación documental que deseas consultar.
            </p>

            <div className="section-divider" />

            {renderNodes(rootNodes)}
          </section>

          <aside className="card panel">
            <h2 className="panel-title">Detalle del nodo</h2>

            {!selectedNodeId && (
              <div className="empty-box">
                Selecciona un nodo para ver su información y sus documentos asociados.
              </div>
            )}

            {selectedNodeId && (
              <>
                <span className="field-label">Identificador del nodo</span>
                <p className="muted" style={{ wordBreak: "break-word", marginTop: 0 }}>
                  {selectedNodeId}
                </p>

                <div className="section-divider" />

                {canManage && (
                  <>
                    <h3 className="panel-title" style={{ fontSize: 16 }}>
                      Nuevo documento
                    </h3>

                    <form onSubmit={handleCreateDocument}>
                      <div style={{ marginBottom: 12 }}>
                        <label className="field-label">Título</label>
                        <input
                          className="input"
                          value={newTitle}
                          onChange={(e) => setNewTitle(e.target.value)}
                        />
                      </div>

                      <div style={{ marginBottom: 12 }}>
                        <label className="field-label">Descripción</label>
                        <textarea
                          className="textarea"
                          rows={3}
                          value={newDescription}
                          onChange={(e) => setNewDescription(e.target.value)}
                        />
                      </div>

                      <div style={{ marginBottom: 12 }}>
                        <label className="field-label">Tipo documental</label>
                        <select
                          className="select"
                          value={newDocumentTypeId}
                          onChange={(e) => setNewDocumentTypeId(e.target.value)}
                        >
                          {documentTypes.map((type) => (
                            <option key={type.id} value={type.id}>
                              {type.name}
                            </option>
                          ))}
                        </select>
                      </div>

                      <button type="submit" className="primary-button" disabled={isCreating}>
                        {isCreating ? "Creando..." : "Crear documento"}
                      </button>
                    </form>

                    <div className="section-divider" />
                  </>
                )}

                <h3 className="panel-title" style={{ fontSize: 16 }}>
                  Documentos del nodo
                </h3>

                {selectedDocuments.length === 0 && (
                  <div className="empty-box">No hay documentos asociados a este nodo.</div>
                )}

                {selectedDocuments.length > 0 && (
                  <ul className="doc-list">
                    {selectedDocuments.map((doc) => (
                      <li key={doc.id} className="doc-item">
                        <div className="doc-title">{doc.title || "Documento sin título"}</div>
                        <div className="doc-meta">
                          {doc.description || "Sin descripción"}
                        </div>
                        <div className="doc-meta">
                          Tipo: {doc.documentTypeName || "No definido"}
                        </div>

                        {fileAvailability[doc.id] === true && (
                          <button
                            type="button"
                            className="download-button"
                            onClick={() => handleDownload(doc.id)}
                          >
                            Descargar archivo
                          </button>
                        )}

                        {fileAvailability[doc.id] === false && canManage && (
                          <div style={{ marginTop: 10 }}>
                            <input
                              className="input"
                              type="file"
                              onChange={(e) => {
                                const file = e.target.files?.[0] ?? null;
                                setSelectedFileByDoc((prev) => ({
                                  ...prev,
                                  [doc.id]: file,
                                }));
                              }}
                            />

                            <div style={{ marginTop: 8 }}>
                              <button
                                type="button"
                                className="secondary-button"
                                onClick={() => handleUpload(doc.id)}
                                disabled={uploadingDocId === doc.id || !selectedFileByDoc[doc.id]}
                              >
                                {uploadingDocId === doc.id ? "Subiendo..." : "Subir archivo"}
                              </button>
                            </div>
                          </div>
                        )}

                        {fileAvailability[doc.id] === false && !canManage && (
                          <p className="muted">Sin archivo asociado</p>
                        )}
                      </li>
                    ))}
                  </ul>
                )}

                <div className="section-divider" />

                <h3 className="panel-title" style={{ fontSize: 16 }}>
                  Acciones disponibles
                </h3>

                <ul style={{ paddingLeft: 18, margin: 0 }}>
                  <li>Visualización de documentos</li>
                  {canManage ? (
                    <li>Gestión documental y carga de archivos</li>
                  ) : (
                    <li>Acceso en modo consulta</li>
                  )}
                </ul>
              </>
            )}
          </aside>
        </div>
      )}
    </div>
  );
}