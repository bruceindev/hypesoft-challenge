import axios from "axios";
import keycloak from "@/lib/keycloak";
import { getFriendlyErrorMessage, notifyError } from "@/lib/notify";

type RetriableRequestConfig = {
  _retry?: boolean;
} & import("axios").InternalAxiosRequestConfig;

// Evita múltiplos refresh simultâneos quando várias requisições retornam 401.
let refreshPromise: Promise<string | null> | null = null;

const refreshAccessToken = async (): Promise<string | null> => {
  if (refreshPromise) {
    return refreshPromise;
  }

  refreshPromise = (async () => {
    if (!keycloak.authenticated) {
      return null;
    }

    await keycloak.updateToken(30);
    return keycloak.token ?? null;
  })().finally(() => {
    refreshPromise = null;
  });

  return refreshPromise;
};

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api",
  timeout: 10_000,
});

apiClient.interceptors.request.use(async (config) => {
  const token = await refreshAccessToken();

  if (!token) {
    window.dispatchEvent(new CustomEvent("auth:unauthorized"));
    return Promise.reject(new axios.AxiosError("Missing auth token", "ERR_AUTH_TOKEN_MISSING", config));
  }

  config.headers.Authorization = `Bearer ${token}`;
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error?.config as RetriableRequestConfig | undefined;
    const status = error?.response?.status as number | undefined;

    if (status === 401 && originalRequest && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const token = await refreshAccessToken();

        if (token) {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return apiClient(originalRequest);
        }
      } catch {
        // Se o refresh falhar, segue para fluxo padrão de sessão expirada.
      }

      notifyError(getFriendlyErrorMessage(401));
      window.dispatchEvent(new CustomEvent("auth:unauthorized"));
    }

    if (status === 403) {
      window.dispatchEvent(new CustomEvent("auth:forbidden"));
    }

    if (status && status >= 500) {
      notifyError(getFriendlyErrorMessage(status));
    }

    return Promise.reject(error);
  },
);
