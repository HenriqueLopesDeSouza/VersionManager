import { http } from "./http";
import type { Software } from "./types";

export type PagedResult<T> = {
  items: T[];
  page: number;
  size: number;
  total: number;
};

export async function getSoftwares(page: number, size: number) {
  const { data } = await http.get<PagedResult<Software>>(
    `/api/softwares?page=${page}&size=${size}`
  );
  return data;
}

export async function getSoftware(id: string) {
  const { data } = await http.get<Software>(`/api/softwares/${id}`);
  return data;
}

export async function createSoftware(payload: { name: string; description?: string | null }) {
  const { data } = await http.post<Software>(`/api/softwares`, payload);
  return data;
}

export async function updateSoftware(id: string, payload: { name: string; description?: string | null }) {
  await http.put(`/api/softwares/${id}`, payload);
}

export async function deleteSoftware(id: string) {
  await http.delete(`/api/softwares/${id}`);
}
