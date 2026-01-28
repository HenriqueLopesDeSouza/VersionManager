import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { createVersion, getVersion, updateVersion } from "../../api/versionApi";
import { getApiErrorMessage } from "../../api/handleApiError";

type Props = { mode: "create" | "edit" };

export function VersionForm({ mode }: Props) {
  const { softwareId, versionId } = useParams();
  const navigate = useNavigate();

  const [version, setVersion] = useState("");
  const [releaseDateISO, setReleaseDateISO] = useState(""); 
  const [isDeprecated, setIsDeprecated] = useState(false);

  const [loading, setLoading] = useState(mode === "edit");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function load() {
      if (mode !== "edit") return;
      setLoading(true);
      setError(null);

      try {
        const v = await getVersion(softwareId!, versionId!);
        setVersion(v.version);
        setIsDeprecated(v.isDeprecated);
        setReleaseDateISO(v.releaseDate ? v.releaseDate.slice(0, 10) : ""); 
      } catch (e) {
        setError(getApiErrorMessage(e));
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [mode, softwareId, versionId]);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!version.trim()) {
      setError("Versão é obrigatória.");
      return;
    }

    if (!releaseDateISO.trim()) {
      setError("Data do release é obrigatória.");
      return;
    }

    setSaving(true);
    try {
      const payload = {
        version: version.trim(),
        releaseDate: releaseDateISO, 
        isDeprecated,
      };

      if (mode === "create") {
        await createVersion(softwareId!, payload);
      } else {
        await updateVersion(softwareId!, versionId!, payload);
      }

      navigate(`/softwares/${softwareId}/versions`);
    } catch (e) {
      setError(getApiErrorMessage(e));
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="card">
      <div className="card-body">
        <div className="d-flex justify-content-between align-items-center mb-3">
          <h4 className="m-0">{mode === "create" ? "Nova Versão" : "Editar Versão"}</h4>
          <Link className="btn btn-outline-secondary" to={`/softwares/${softwareId}/versions`}>
            Voltar
          </Link>
        </div>

        {error && <div className="alert alert-danger">{error}</div>}

        {loading ? (
          <div className="alert alert-secondary">Carregando...</div>
        ) : (
          <form onSubmit={onSubmit} className="d-grid gap-3">
            <div>
              <label className="form-label">Versão *</label>
              <input
                className="form-control"
                value={version}
                onChange={(e) => setVersion(e.target.value)}
                placeholder="Ex: 1.0.0"
              />
            </div>

            <div>
              <label className="form-label">Data do release *</label>
              <input
                type="date"                    
                className="form-control"
                value={releaseDateISO}         
                onChange={(e) => setReleaseDateISO(e.target.value)}
              />
            </div>

            <div className="form-check">
              <input
                className="form-check-input"
                type="checkbox"
                checked={isDeprecated}
                onChange={(e) => setIsDeprecated(e.target.checked)}
                id="isDeprecated"
              />
              <label className="form-check-label" htmlFor="isDeprecated">
                Depreciada
              </label>
            </div>

            <div className="d-flex gap-2">
              <button className="btn btn-primary" type="submit" disabled={saving}>
                {saving ? "Salvando..." : "Salvar"}
              </button>
              <Link className="btn btn-outline-secondary" to={`/softwares/${softwareId}/versions`}>
                Cancelar
              </Link>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}
