import { fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query';

// TODO: Configure a URL base da API
const baseUrl = import.meta.env.VITE_API_URL || 'http://localhost:3000/api';

const baseQuery = fetchBaseQuery({
  baseUrl,
  prepareHeaders: (headers) => {
    // TODO: Adicionar token de autenticação se necessário
    // const token = getAuthToken();
    // if (token) {
    //   headers.set('authorization', `Bearer ${token}`);
    // }
    return headers;
  },
});

export const baseQueryWithReauth: BaseQueryFn<
  string | FetchArgs,
  unknown,
  FetchBaseQueryError
> = async (args, api, extraOptions) => {
  let result = await baseQuery(args, api, extraOptions);
  
  // TODO: Implementar lógica de reautenticação se necessário
  // if (result?.error?.status === 401) {
  //   // Tentar refresh token
  //   const refreshResult = await baseQuery('/refresh', api, extraOptions);
  //   if (refreshResult?.data) {
  //     // Retry original query
  //     result = await baseQuery(args, api, extraOptions);
  //   }
  // }
  
  return result;
};

