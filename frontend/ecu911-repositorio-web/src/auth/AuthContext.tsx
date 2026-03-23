import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { loginRequest, meRequest } from "../api/authApi";
import type { LoginRequest, UserMe } from "../types/auth";

type AuthContextType = {
  token: string | null;
  user: UserMe | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (payload: LoginRequest) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const TOKEN_KEY = "ecu911_token";
const USER_KEY = "ecu911_user";

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(localStorage.getItem(TOKEN_KEY));
  const [user, setUser] = useState<UserMe | null>(() => {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) as UserMe : null;
  });
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    async function bootstrap() {
      if (!token) return;

      try {
        setIsLoading(true);
        const currentUser = await meRequest(token);
        setUser(currentUser);
        localStorage.setItem(USER_KEY, JSON.stringify(currentUser));
      } catch {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USER_KEY);
        setToken(null);
        setUser(null);
      } finally {
        setIsLoading(false);
      }
    }

    bootstrap();
  }, [token]);

  async function login(payload: LoginRequest) {
    setIsLoading(true);
    try {
      const result = await loginRequest(payload);
      setToken(result.token);
      localStorage.setItem(TOKEN_KEY, result.token);

      const currentUser = await meRequest(result.token);
      setUser(currentUser);
      localStorage.setItem(USER_KEY, JSON.stringify(currentUser));
    } finally {
      setIsLoading(false);
    }
  }

  function logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setToken(null);
    setUser(null);
  }

  const value = useMemo<AuthContextType>(() => ({
    token,
    user,
    isAuthenticated: !!token && !!user,
    isLoading,
    login,
    logout,
  }), [token, user, isLoading]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth debe usarse dentro de AuthProvider.");
  }

  return context;
}