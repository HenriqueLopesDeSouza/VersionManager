import { useEffect, useState } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import { createSoftware, getSoftware, updateSoftware } from "../../api/softwareApi";
import { getApiErrorMessage } from "../../api/handleApiError";

type Props = { mode: "create" | "edit" };

export function SoftwareForm({ mode }: Props) {
  const { id } = useParams();
  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [description, setDescription] = useState<string>("");
  const [loading, setLoading] = useState(mode === "edit");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function load() {
      if (mode !== "edit" || !id) return;
      setLoading(true);
      try {
        const s = await getSoftware(id);
        setName(s.name);
        setDescription(s.description ?? "");
      } catch {
        setError("Não foi possível carregar o software.");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [mode, id]);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!name.trim()) {
      setError("Nome é obrigatório.");
      return;
    }

    setSaving(true);
    try {
      if (mode === "create") {
        const created = await createSoftware({ name: name.trim(), description: description.trim() || null });


        navigate(`/softwares/${created.id}/versions?first=1`);
      } else {
        await updateSoftware(id!, { name: name.trim(), description: description.trim() || null });
        navigate("/softwares");
      }
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
          <h4 className="m-0">{mode === "create" ? "Novo Software" : "Editar Software"}</h4>
          <Link className="btn btn-outline-secondary" to="/softwares">Voltar</Link>
        </div>

        {error && <div className="alert alert-danger">{error}</div>}
        {loading ? (
          <div className="alert alert-secondary">Carregando...</div>
        ) : (
          <form onSubmit={onSubmit} className="d-grid gap-3">
            <div>
              <label className="form-label">Nome *</label>
              <input
                className="form-control"
                value={name}
                onChange={(e) => setName(e.target.value)}
                maxLength={120}
                placeholder="Ex: Portal RH"
              />
            </div>

            <div>
              <label className="form-label">Descrição</label>
              <textarea
                className="form-control"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                rows={3}
                placeholder="Opcional..."
              />
            </div>

            <div className="d-flex gap-2">
              <button className="btn btn-primary" type="submit" disabled={saving}>
                {saving ? "Salvando..." : "Salvar"}
              </button>
              <Link className="btn btn-outline-secondary" to="/softwares">Cancelar</Link>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}
