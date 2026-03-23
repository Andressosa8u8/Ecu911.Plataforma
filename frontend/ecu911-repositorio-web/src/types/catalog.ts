export type RepositoryNode = {
  id: string;
  name: string;
  description?: string | null;
  parentId?: string | null;
  isActive: boolean;
  createdAt: string;
};

export type DocumentItem = {
  id: string;
  title: string;
  description?: string | null;
  createdAt: string;
  documentTypeId: string;
  documentTypeName?: string;
  repositoryNodeId: string;
  repositoryNodeName?: string;
};

export type DocumentType = {
  id: string;
  name: string;
  description?: string | null;
  isActive: boolean;
  createdAt: string;
};

export type CreateDocumentItemRequest = {
  title: string;
  description?: string | null;
  repositoryNodeId: string;
  documentTypeId: string;
};