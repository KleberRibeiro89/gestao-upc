import type { User } from "~/shared/models/IUser";
import { createCrudBaseApi } from "../@crudBase";

export const userApi = createCrudBaseApi<User>('user');

export const {
  useGetPaginatedQuery: useGetPaginatedUser,
  useGetByIdQuery: useGetByIdUser,
  usePostMutation: usePostUser,
  usePutMutation: usePutUser,
  useDeleteMutation: useDeleteUser,
} = userApi;

