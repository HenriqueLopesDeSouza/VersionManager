export type Software = {
  id: string;
  name: string;
  description?: string | null;
  createdAt: string;
};

export type Version = {
  id: string;
  version: string;
  releaseDate?: string | null;
  isDeprecated: boolean;
};


export type PagedResult<T> = {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
};
