import axios from "axios";

type ProblemDetails = {
  title?: string;
  detail?: string;
  status?: number;
};

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ProblemDetails | undefined;

    if (data?.detail) return data.detail;

    switch (error.response?.status) {
      case 400: return "Dados inválidos.";
      case 404: return "Registro não encontrado.";
      case 409: return "Conflito de dados.";
      case 500: return "Erro interno do servidor.";
      default: return "Erro de comunicação com a API.";
    }
  }
  return "Erro inesperado.";
}
