/* eslint-disable react-refresh/only-export-components */
import { createContext, useContext, useEffect, useMemo, useState } from "react";
import keycloak from "@/lib/keycloak";
import type { AuthState, UserRole } from "@/types/auth";

type AuthContextValue = AuthState & {
  login: () => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

const parseRoles = (tokenParsed: Record<string, unknown> | undefined): UserRole[] => {
  const resourceAccess = tokenParsed?.resource_access as { [key: string]: { roles?: string[] } } | undefined;
  const realmAccess = tokenParsed?.realm_access as { roles?: string[] } | undefined;
  const clientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "hypesoft-frontend";
  const clientRoles = resourceAccess?.[clientId]?.roles ?? [];
  const realmRoles = realmAccess?.roles ?? [];
  const roles = [...clientRoles, ...realmRoles];

  return Array.from(new Set(roles))
    .map((role) => role.toLowerCase())
    .filter((role) => ["admin", "manager", "user"].includes(role))
    .map((role) => role.charAt(0).toUpperCase() + role.slice(1) as UserRole);
};

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [state, setState] = useState<AuthState>({
    isAuthenticated: false,
    isLoading: true,
  });

  useEffect(() => {
    let isMounted = true;

    const initializeAuth = async () => {
      try {
        const authenticated = await keycloak.init({
          onLoad: "check-sso",
          pkceMethod: "S256",
          checkLoginIframe: false,
        });

        if (!isMounted) {
          return;
        }

        if (!authenticated) {
          setState({ isAuthenticated: false, isLoading: false });
          return;
        }

        const tokenParsed = keycloak.tokenParsed as Record<string, unknown> | undefined;

        setState({
          isAuthenticated: true,
          isLoading: false,
          token: keycloak.token,
          user: {
            id: String(tokenParsed?.sub ?? ""),
            name: String(tokenParsed?.name ?? "User"),
            email: tokenParsed?.email ? String(tokenParsed.email) : undefined,
            roles: parseRoles(tokenParsed),
          },
        });
      } catch {
        if (isMounted) {
          setState({ isAuthenticated: false, isLoading: false });
        }
      }
    };

    initializeAuth();

    return () => {
      isMounted = false;
    };
  }, []);

  useEffect(() => {
    // Mantém o redirecionamento de autenticação concentrado em um único lugar.
    const onUnauthorized = () => {
      setState((previousState) => ({ ...previousState, isAuthenticated: false }));
      window.location.href = "/login";
    };

    const onForbidden = () => {
      window.location.href = "/forbidden";
    };

    window.addEventListener("auth:unauthorized", onUnauthorized);
    window.addEventListener("auth:forbidden", onForbidden);

    return () => {
      window.removeEventListener("auth:unauthorized", onUnauthorized);
      window.removeEventListener("auth:forbidden", onForbidden);
    };
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      ...state,
      login: () => keycloak.login(),
      logout: () => keycloak.logout({ redirectUri: window.location.origin + "/login" }),
    }),
    [state],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuthContext = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuthContext must be used within AuthProvider");
  }
  return context;
};
