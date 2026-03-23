import type { LoginRequest, LoginResponse, UserMe } from "../types/auth";

const AUTH_BASE_URL = "https://localhost:7104/api/Auth";

export async function loginRequest(payload: LoginRequest): Promise<LoginResponse> {
  const response = await fetch(`${AUTH_BASE_URL}/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Credenciales inválidas o acceso no permitido.");
  }

  return response.json();
}

export async function meRequest(token: string): Promise<UserMe> {
  const response = await fetch(`${AUTH_BASE_URL}/me`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "No se pudo obtener la sesión actual.");
  }

  return response.json();
}