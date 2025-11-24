export interface IResponsePaginated<T> {
  pageNumber: number;
  pageSize: number;
  totalRows: number;
  totalPages: number;
  result: T[];
}