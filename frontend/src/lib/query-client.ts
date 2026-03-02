import { MutationCache, QueryCache, QueryClient } from "@tanstack/react-query";
import { AxiosError } from "axios";
import { getFriendlyErrorMessage, notifyError } from "@/lib/notify";

const getErrorStatus = (error: unknown): number | undefined => {
  if (error instanceof AxiosError) {
    return error.response?.status;
  }

  return undefined;
};

const shouldShowGlobalError = (status?: number) => {
  if (!status) {
    return true;
  }

  return status >= 500;
};

export const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error) => {
      const status = getErrorStatus(error);

      if (!shouldShowGlobalError(status)) {
        return;
      }

      notifyError(getFriendlyErrorMessage(status));
    },
  }),
  mutationCache: new MutationCache({
    onError: (error) => {
      const status = getErrorStatus(error);
      notifyError(getFriendlyErrorMessage(status));
    },
  }),
  defaultOptions: {
    queries: {
      staleTime: 60_000,
      gcTime: 300_000,
      retry: (failureCount, error) => {
        const status = getErrorStatus(error);

        if (status === 401 || status === 403) {
          return false;
        }

        return failureCount < 2;
      },
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: 0,
    },
  },
});
