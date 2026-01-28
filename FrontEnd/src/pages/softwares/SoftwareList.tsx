import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getSoftwares, deleteSoftware } from "../../api/softwareApi";
import type { Software } from "../../api/types";
import { getApiErrorMessage } from "../../api/handleApiError";

type PagedResult<T> = {
  items: T[];
  page: number;
  size: number;
  total: number;
};

export function SoftwareList() {
  const [data, setData] = useState<PagedResult<Software> | null>(null);
  const [page, setPage] = useState(1);
  const size = 20;

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setLoading(true);
    setError(null);
    try {
      const r = await getSoftwares(page, size);
      setData(r);
    } catch (e) {
      setError(getApiErrorMessage(e));
    } finally {
      setLoading(false);
    }
  }

  async function onDelete(id: string) {
    if (!confirm("Excluir software?")) return;
    await deleteSoftware(id);
    load();
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page]);

  if (loading) return <div>Carregando...</div>;

  if (error) return <div className="alert alert-danger">{error}</div>;

  if (!data || data.items.length === 0) {
    return (
      <>
        <div className="d-flex justify-content-between align-items-center mb-3">
          <h3 className="m-0">Softwares</h3>
          <Link className="btn btn-primary" to="/softwares/new">Novo software</Link>
        </div>
        <div className="alert alert-secondary">Nenhum software cadastrado.</div>
      </>
    );
  }

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h3 className="m-0">Softwares</h3>
        <Link className="btn btn-primary" to="/softwares/new">Novo software</Link>
      </div>

      <table className="table table-striped align-middle">
        <thead>
          <tr>
            <th>Nome</th>
            <th>Descrição</th>
            <th style={{ width: 280 }}></th>
          </tr>
        </thead>
        <tbody>
          {data.items.map((s) => (
            <tr key={s.id}>
              <td>{s.name}</td>
              <td>{s.description ?? "-"}</td>
              <td className="text-end">
                <Link className="btn btn-sm btn-outline-secondary me-2" to={`/softwares/${s.id}/edit`}>Editar</Link>
                <Link className="btn btn-sm btn-outline-info me-2" to={`/softwares/${s.id}/versions`}>Versões</Link>
                <button className="btn btn-sm btn-outline-danger" onClick={() => onDelete(s.id)}>Excluir</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="d-flex gap-2">
        <button className="btn btn-outline-secondary" disabled={page === 1} onClick={() => setPage(p => p - 1)}>
          Anterior
        </button>
        <button className="btn btn-outline-secondary" disabled={page * size >= data.total} onClick={() => setPage(p => p + 1)}>
          Próxima
        </button>
      </div>
    </>
  );
}
