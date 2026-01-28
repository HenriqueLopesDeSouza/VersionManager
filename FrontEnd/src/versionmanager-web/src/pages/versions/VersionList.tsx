import { useEffect, useState } from "react";
import { Link, useParams, useSearchParams } from "react-router-dom";
import { deleteVersion, getLatestVersion, getVersions } from "../../api/versionApi";
import { getSoftware } from "../../api/softwareApi";
import type { Version, Software } from "../../api/types";
import type { PagedResult } from "../../api/versionApi";
import { getApiErrorMessage } from "../../api/handleApiError";
import { isoToBR } from "../../utils/date"; 

export function VersionList() {
  const { softwareId } = useParams();
  const [params] = useSearchParams();
  const first = params.get("first") === "1";

  const [software, setSoftware] = useState<Software | null>(null);
  const [latest, setLatest] = useState<Version | null>(null);
  const [data, setData] = useState<PagedResult<Version> | null>(null);

  const [page, setPage] = useState(1);
  const size = 20;

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setLoading(true);
    setError(null);

    try {
      const [sw, paged] = await Promise.all([
        getSoftware(softwareId!),
        getVersions(softwareId!, page, size),
      ]);

      setSoftware(sw);
      setData(paged);

      if (paged.total > 0) {
        const l = await getLatestVersion(softwareId!);
        setLatest(l);
      } else {
        setLatest(null);
      }
    } catch (e) {
      setError(getApiErrorMessage(e));
    } finally {
      setLoading(false);
    }
  }

  async function onDelete(versionId: string) {
    if (!confirm("Excluir versão?")) return;
    await deleteVersion(softwareId!, versionId);
    load();
  }

  useEffect(() => {
    load();
  }, [softwareId, page]);

  if (loading) return <div>Carregando...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  const items = data?.items ?? [];
  const hasVersions = (data?.total ?? 0) > 0;

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div>
          <h3 className="m-0">Versões</h3>
          <div className="text-muted">
            Nome do Software - {software?.name ?? "Software"}
          </div>
        </div>

        <div className="d-flex gap-2">
          <Link className="btn btn-outline-secondary" to="/softwares">Voltar</Link>
          <Link className="btn btn-primary" to={`/softwares/${softwareId}/versions/new`}>Nova versão</Link>
        </div>
      </div>

      {first && (
        <div className="alert alert-success">
          Software criado com sucesso. Cadastre a primeira versão.
        </div>
      )}

      {hasVersions && latest ? (
        <div className="alert alert-info d-flex justify-content-between align-items-center">
          <div>
            <strong>Última versão:</strong> {latest.version}{" "}
            {latest.releaseDate ? `— ${isoToBR(latest.releaseDate)}` : ""}
          </div>
          <span className={`badge ${latest.isDeprecated ? "text-bg-warning" : "text-bg-success"}`}>
            {latest.isDeprecated ? "Depreciada" : "Ativa"}
          </span>
        </div>
      ) : (
        <div className="alert alert-secondary">
          Ainda não existe versão cadastrada para este software.
        </div>
      )}

      {hasVersions && (
        <>
          <table className="table table-striped align-middle">
            <thead>
              <tr>
                <th>Versão</th>
                <th>Release</th>
                <th>Depreciada</th>
                <th style={{ width: 220 }}></th>
              </tr>
            </thead>
            <tbody>
              {items.map((v) => (
                <tr key={v.id}>
                  <td>{v.version}</td>
                  <td>{v.releaseDate ? isoToBR(v.releaseDate) : "-"}</td>
                  <td>{v.isDeprecated ? "Sim" : "Não"}</td>
                  <td className="text-end">
                    <Link
                      className="btn btn-sm btn-outline-secondary me-2"
                      to={`/softwares/${softwareId}/versions/${v.id}/edit`}
                    >
                      Editar
                    </Link>
                    <button
                      className="btn btn-sm btn-outline-danger"
                      onClick={() => onDelete(v.id)}
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="d-flex gap-2">
            <button
              className="btn btn-outline-secondary"
              disabled={page === 1}
              onClick={() => setPage((p) => p - 1)}
            >
              Anterior
            </button>
            <button
              className="btn btn-outline-secondary"
              disabled={page * size >= (data?.total ?? 0)}
              onClick={() => setPage((p) => p + 1)}
            >
              Próxima
            </button>
          </div>
        </>
      )}
    </>
  );
}
