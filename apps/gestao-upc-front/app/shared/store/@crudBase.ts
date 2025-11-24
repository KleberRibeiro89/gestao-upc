import type { FilterCondition } from '../models/api/FilterConditions';
import type { IResponsePaginated } from '../models/api/IResponsePaginated';
import { createApi } from '@reduxjs/toolkit/query/react';
import { baseQueryWithReauth } from './baseQuery';

export interface IPaginationParams<TModel> {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: ISortParameter<TModel>[];
  filter?: IFilterParameter<TModel>[];
  operator?: FilterCondition;
}

export interface IFilterParameter<TModel> {
  name: keyof TModel;
  condition: number;
  value: string;
}

export interface ISortParameter<TModel> {
  name: keyof TModel;
  order: string;
}

export const createCrudBaseApi = <TModel>(path: string) => {
  const api = createApi({
    baseQuery: baseQueryWithReauth,
    reducerPath: `crud-${path}`,
    // refetchOnMountOrArgChange: true,
    endpoints: (build) => ({
      getPaginated: build.query<IResponsePaginated<TModel>, IPaginationParams<TModel> | void>({
        query: (params) => {
          const defaultParams = {
            pageNumber: 0,
            pageSize: 15,
            orderBy: [],
            filter: [],
            operator: null,
          };

          const finalParams = { ...defaultParams, ...params };

          const queryParams = new URLSearchParams({
            pageNumber: finalParams.pageNumber.toString(),
            pageSize: finalParams.pageSize.toString(),
          });

          if (finalParams.orderBy.length > 0) {
            queryParams.append('orderByString', JSON.stringify(finalParams.orderBy));
          }

          if (finalParams.filter.length > 0) {
            queryParams.append('filterString', JSON.stringify(finalParams.filter));
          }

          if (finalParams.operator) {
            queryParams.append('operator', finalParams.operator.toString());
          }

          return `${path}/paginated?${queryParams.toString()}`;
        },
      }),
      getById: build.query<TModel, string>({
        query: (id) => `${path}/${id}`,
      }),
      post: build.mutation<TModel, Partial<TModel>>({
        query: (data) => ({
          url: path,
          method: 'POST',
          body: data,
        }),
      }),
      put: build.mutation<TModel, Partial<TModel>>({
        query: (data) => ({
          url: path,
          method: 'PUT',
          body: data,
        }),
      }),
      delete: build.mutation<void, string>({
        query: (id) => ({
          url: `${path}/${id}`,
          method: 'DELETE',
        }),
      }),
    }),
  });
  const {
    useGetPaginatedQuery,
    useGetByIdQuery,
    usePostMutation,
    usePutMutation,
    useDeleteMutation,
  } = api;
  return {
    api: api,
    useGetPaginatedQuery,
    useGetByIdQuery,
    usePostMutation,
    usePutMutation,
    useDeleteMutation,
  };
};
