import type {
  CreateDocumentItemRequest,
  DocumentItem,
  DocumentType,
  RepositoryNode,
} from "../types/catalog";

const CATALOG_BASE_URL = "https://localhost:7221/api";

async function apiGet<T>(url: string, token: string): Promise<T> {
  const response = await fetch(url, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Error al consultar CatalogService.");
  }

  return response.json();
}

async function apiPost<T>(url: string, token: string, payload: unknown): Promise<T> {
  const response = await fetch(url, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Error al crear el recurso.");
  }

  return response.json();
}

export async function getRootNodes(token: string): Promise<RepositoryNode[]> {
  return apiGet<RepositoryNode[]>(
    `${CATALOG_BASE_URL}/RepositoryNodes/roots`,
    token
  );
}

export async function getChildNodes(nodeId: string, token: string): Promise<RepositoryNode[]> {
  return apiGet<RepositoryNode[]>(
    `${CATALOG_BASE_URL}/RepositoryNodes/${nodeId}/children`,
    token
  );
}

export async function getAllDocuments(token: string): Promise<DocumentItem[]> {
  const result = await apiGet<unknown>(
    `${CATALOG_BASE_URL}/DocumentItems`,
    token
  );

  if (Array.isArray(result)) {
    return result as DocumentItem[];
  }

  if (
    result &&
    typeof result === "object" &&
    "items" in result &&
    Array.isArray((result as { items: unknown[] }).items)
  ) {
    return (result as { items: DocumentItem[] }).items;
  }

  return [];
}

export async function getDocumentTypes(token: string): Promise<DocumentType[]> {
  return apiGet<DocumentType[]>(
    `${CATALOG_BASE_URL}/DocumentTypes`,
    token
  );
}

export async function createDocumentItem(
  payload: CreateDocumentItemRequest,
  token: string
): Promise<DocumentItem> {
  return apiPost<DocumentItem>(
    `${CATALOG_BASE_URL}/DocumentItems`,
    token,
    payload
  );
}

export async function hasDocumentFile(documentId: string, token: string): Promise<boolean> {
  const response = await fetch(
    `${CATALOG_BASE_URL}/DocumentItems/${documentId}/file`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (response.ok) {
    return true;
  }

  if (response.status === 404) {
    return false;
  }

  const text = await response.text();
  throw new Error(text || "No se pudo verificar el archivo del documento.");
}

export async function downloadDocumentFile(documentId: string, token: string): Promise<void> {
  const response = await fetch(
    `${CATALOG_BASE_URL}/DocumentItems/${documentId}/file/download`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "No se pudo descargar el archivo.");
  }

  const blob = await response.blob();
  const url = URL.createObjectURL(blob);

  const contentDisposition = response.headers.get("Content-Disposition");
  let fileName = `documento-${documentId}`;

  if (contentDisposition) {
    const match = /filename="?([^"]+)"?/.exec(contentDisposition);
    if (match?.[1]) {
      fileName = match[1];
    }
  }

  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  link.remove();

  URL.revokeObjectURL(url);
}

export async function uploadDocumentFile(
  documentId: string,
  file: File,
  token: string
): Promise<void> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch(
    `${CATALOG_BASE_URL}/DocumentItems/${documentId}/file`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: formData,
    }
  );

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "No se pudo subir el archivo.");
  }
}