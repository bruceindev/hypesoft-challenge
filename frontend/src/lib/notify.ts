import { toast } from "sonner";

export const notifySuccess = (message: string) => {
  toast.success(message);
};

export const notifyError = (message: string) => {
  toast.error(message);
};

export const getFriendlyErrorMessage = (status?: number) => {
  if (!status) {
    return "Não foi possível concluir a operação. Verifique sua conexão.";
  }

  if (status === 400) {
    return "Dados inválidos. Revise os campos e tente novamente.";
  }

  if (status === 401) {
    return "Sua sessão expirou. Faça login novamente.";
  }

  if (status === 403) {
    return "Você não tem permissão para esta ação.";
  }

  if (status >= 500) {
    return "Erro interno do servidor. Tente novamente em instantes.";
  }

  return "Ocorreu um erro inesperado.";
};
