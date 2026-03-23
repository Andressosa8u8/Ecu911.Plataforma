import { NavLink, Outlet } from "react-router";
import { useAuth } from "../auth/AuthContext";

export default function AppLayout() {
  const { user, logout } = useAuth();

  const initials = user?.fullName
    ? user.fullName
        .split(" ")
        .slice(0, 2)
        .map((x) => x[0]?.toUpperCase())
        .join("")
    : "U";

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand-block">
          <div className="brand-logo">EC</div>

          <div className="brand-meta">
            <span className="brand-kicker">ECU 911</span>
            <span className="brand-title">Repositorio Digital</span>
            <span className="brand-subtitle">Gestión documental institucional</span>
          </div>
        </div>

        <div className="user-card">
          <p><strong>Usuario:</strong> {user?.fullName}</p>
          <p><strong>Rol:</strong> {user?.roles.join(", ")}</p>
          <p><strong>Sistema:</strong> {user?.currentSystem ?? "N/D"}</p>
        </div>

        <nav className="sidebar-nav">
          <NavLink
            to="/"
            end
            className={({ isActive }) => `sidebar-link ${isActive ? "active" : ""}`}
          >
            Panel principal
          </NavLink>

          <NavLink
            to="/nodes"
            className={({ isActive }) => `sidebar-link ${isActive ? "active" : ""}`}
          >
            Nodos del repositorio
          </NavLink>
        </nav>

        <div className="sidebar-footer">
          <button type="button" className="logout-button" onClick={logout}>
            Cerrar sesión
          </button>
        </div>
      </aside>

      <div className="main-panel">
        <header className="topbar">
          <div className="topbar-title">
            <h1>Repositorio Digital</h1>
            <span>ECU 911 · Plataforma documental institucional</span>
          </div>

          <div className="topbar-user">
            <div className="info-tag">{user?.currentSystem ?? "Sin sistema"}</div>
            <div className="user-avatar">{initials}</div>
          </div>
        </header>

        <main className="page-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}