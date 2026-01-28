import axios from "axios";
import { http } from "./http";
import type { Version } from "./types";

export type PagedResult<T> = {
  items: T[];
  page: number;
  size: number;
  total: number;
};

export async function getVersions(softwareId: string, page: number, size: number) {
  const { data } = await http.get<PagedResult<Version>>(
    `/api/softwares/${softwareId}/versions?page=${page}&size=${size}`
  );
  return data;
}

export async function getLatestVersion(softwareId: string): Promise<Version | null> {
  try {
    const { data } = await http.get<Version>(
      `/api/softwares/${softwareId}/versions/latest`
    );
    return data;
  } catch (err) {
    if (axios.isAxiosError(err) && err.response?.status === 404) return null;
    throw err;
  }
}

export async function getVersion(softwareId: string, versionId: string) {
  const { data } = await http.get<Version>(
    `/api/softwares/${softwareId}/versions/${versionId}`
  );
  return data;
}

export async function createVersion(
  softwareId: string,
  payload: { version: string; releaseDate?: string | null; isDeprecated: boolean }
) {
  await http.post(`/api/softwares/${softwareId}/versions`, payload);
}

export async function updateVersion(
  softwareId: string,
  versionId: string,
  payload: { version: string; releaseDate?: string | null; isDeprecated: boolean }
) {
  await http.put(`/api/softwares/${softwareId}/versions/${versionId}`, payload);
}

export async function deleteVersion(softwareId: string, versionId: string) {
  await http.delete(`/api/softwares/${softwareId}/versions/${versionId}`);
}
