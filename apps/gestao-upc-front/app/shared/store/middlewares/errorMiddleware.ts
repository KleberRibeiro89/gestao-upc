import { isRejectedWithValue, type Middleware } from '@reduxjs/toolkit';

export const errorMiddleware: Middleware = () => (next) => (action) => {
  if (isRejectedWithValue(action)) {
    let errorMessage = 'Ocorreu um erro inesperado, tente novamente mais tarde';
    let errorTitle = 'Ocorreu um Erro';
    const status = (action.payload as any)?.status || (action.error as any)?.status;

    const extractApiMessage = (data: any): string | null => {
      if (!data || typeof data !== 'object') return null;

      if (data.success === false) {
        if (data.errors && Array.isArray(data.errors) && data.errors.length > 0) {
          const errorMessages = data.errors
            .map((error: any) => error.message || error.field)
            .filter(Boolean);

          if (errorMessages.length > 0) {
            return errorMessages.join(', ');
          }
        }

        if (data.message) {
          return data.message;
        }
      }

      return null;
    };

    switch (status) {
      case 400: {
        let apiMessage = null;

        if (action.payload && typeof action.payload === 'object') {
          if ('data' in action.payload && action.payload.data) {
            apiMessage = extractApiMessage(action.payload.data);
          }
        }

        errorMessage = apiMessage || 'Dados inválidos. Verifique as informações e tente novamente.';
        errorTitle = 'Erro de Validação';
        break;
      }

      case 401:
        errorMessage = 'Sua sessão expirou. Por favor, faça login novamente.';
        errorTitle = 'Sessão Expirada';
        break;

      case 403:
        errorMessage = 'Você não tem permissão para realizar esta ação.';
        errorTitle = 'Acesso Negado';
        break;

      case 404:
        errorMessage = 'O recurso solicitado não foi encontrado.';
        errorTitle = 'Não Encontrado';
        break;

      case 500:
        errorMessage = 'Erro no servidor. Por favor, tente novamente mais tarde.';
        errorTitle = 'Erro do Servidor';
        break;

      case 'FETCH_ERROR':
        errorMessage = 'Erro de conexão. Verifique sua internet e tente novamente.';
        errorTitle = 'Erro de Conexão';
        break;

      case 'TIMEOUT_ERROR':
        // Timeout
        errorMessage = 'A requisição demorou muito tempo. Tente novamente.';
        errorTitle = 'Tempo Esgotado';
        break;

      default:
        // Erro genérico ou desconhecido
        errorMessage = 'Ocorreu um erro inesperado. Tente novamente mais tarde.';
        errorTitle = 'Ocorreu um Erro';
    }

    console.error('errorMiddleware caught an error:', {
      status,
      error: action.error,
      payload: action.payload,
      errorTitle: errorTitle,
      errorMessage: errorMessage,
    });

    // notification.error({
    //   message: errorTitle,
    //   description: errorMessage,
    //   duration: 5,
    // });
  }

  return next(action);
};