import { useState } from "react";
import { useNavigate } from "react-router";
import { useAuth } from "../auth/AuthContext";

export default function LoginPage() {
  const navigate = useNavigate();
  const { login, isLoading } = useAuth();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [systemCode, setSystemCode] = useState("REPOSITORIO");
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      await login({
        username,
        password,
        systemCode,
      });

      navigate("/");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al iniciar sesión.");
    }
  }

  return (
    <div className="login-shell">
      <section className="login-hero">
        <div className="login-hero-card">
          <div className="login-logo">EC</div>

          <h1>Repositorio Digital ECU 911</h1>
          <p>
            Plataforma institucional para la gestión, consulta, carga y resguardo
            de documentación oficial, organizada por nodos, direcciones y estructura
            documental autorizada.
          </p>

          <div className="login-badges">
            <span className="login-badge">Seguridad por roles</span>
            <span className="login-badge">Acceso por sistema</span>
            <span className="login-badge">Control documental</span>
          </div>
        </div>
      </section>

      <section className="login-panel">
        <div className="login-card">
          <h2>Iniciar sesión</h2>
          <p>Ingresa con tu usuario institucional para acceder al repositorio.</p>

          <form onSubmit={handleSubmit}>
            <div style={{ marginBottom: 14 }}>
              <label className="field-label">Usuario</label>
              <input
                className="input"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>

            <div style={{ marginBottom: 14 }}>
              <label className="field-label">Contraseña</label>
              <input
                className="input"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>

            <div style={{ marginBottom: 18 }}>
              <label className="field-label">Sistema</label>
              <select
                className="select"
                value={systemCode}
                onChange={(e) => setSystemCode(e.target.value)}
              >
                <option value="REPOSITORIO">REPOSITORIO</option>
                <option value="BIBLIOTECA">BIBLIOTECA</option>
              </select>
            </div>

            {error && <div className="error-box">{error}</div>}

            <button type="submit" className="primary-button" disabled={isLoading} style={{ width: "100%" }}>
              {isLoading ? "Ingresando..." : "Ingresar al sistema"}
            </button>
          </form>

          <div className="login-footer-links">
            <span>Cambio de contraseña</span>
            <span>¿Olvidaste tu acceso?</span>
          </div>
        </div>
      </section>
    </div>
  );
}