export interface ApiFieldError {
  field: string;
  message: string;
}

export interface ApiError {
  code: string;
  message: string;
  traceId?: string;
  fieldErrors?: ApiFieldError[];
}
