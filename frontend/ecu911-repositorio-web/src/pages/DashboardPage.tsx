import { useAuth } from "../auth/AuthContext";

export default function DashboardPage() {
  const { user } = useAuth();

  const rolesCount = user?.roles.length ?? 0;
  const systemsCount = user?.systems.length ?? 0;
  const hasManageRole =
    user?.roles.includes("ADMIN") || user?.roles.includes("GESTOR_DOCUMENTAL");

  return (
    <div>
      <div className="card panel">
        <h2 className="panel-title">Bienvenido de nuevo</h2>
        <p className="muted">
          Has iniciado sesión en el sistema <strong>{user?.currentSystem ?? "N/D"}</strong>.
          Desde aquí puedes navegar la estructura documental y operar según tus permisos.
        </p>

        <div className="kpi-grid">
          <div className="kpi-card primary">
            <div className="kpi-label">Usuario autenticado</div>
            <div className="kpi-value">{user?.username ?? "N/D"}</div>
            <div className="kpi-subtext">Sesión institucional activa</div>
          </div>

          <div className="kpi-card success">
            <div className="kpi-label">Roles asignados</div>
            <div className="kpi-value">{rolesCount}</div>
            <div className="kpi-subtext">{user?.roles.join(", ") || "Sin roles"}</div>
          </div>

          <div className="kpi-card warning">
            <div className="kpi-label">Sistemas habilitados</div>
            <div className="kpi-value">{systemsCount}</div>
            <div className="kpi-subtext">
              {user?.systems.join(", ") || "Sin sistemas"}
            </div>
          </div>
        </div>
      </div>

      <div style={{ height: 24 }} />

      <div className="page-grid">
        <section className="card panel">
          <h3 className="panel-title">Resumen de acceso</h3>

          <div className="section-divider" />

          <p className="muted">
            <strong>Nombre completo:</strong> {user?.fullName}
          </p>
          <p className="muted">
            <strong>Correo:</strong> {user?.email}
          </p>
          <p className="muted">
            <strong>Unidad organizacional:</strong>{" "}
            {user?.organizationalUnitId ?? "No definida"}
          </p>
          <p className="muted">
            <strong>Modo de operación:</strong>{" "}
            {hasManageRole ? "Gestión documental" : "Consulta"}
          </p>
        </section>

        <aside className="card panel">
          <h3 className="panel-title">Acciones rápidas</h3>

          <div className="section-divider" />

          <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            <div className="info-tag-soft">Consultar nodos del repositorio</div>
            <div className="info-tag-soft">Visualizar documentos asociados</div>
            {hasManageRole && (
              <div className="info-tag-soft">Crear documentos y cargar archivos</div>
            )}
            <div className="info-tag-soft">Descargar archivos permitidos</div>
          </div>
        </aside>
      </div>
    </div>
  );
}