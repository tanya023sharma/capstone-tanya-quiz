import {
  HttpErrorResponse,
  HttpInterceptorFn,
} from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiError } from '../../shared/models/api-error.models';

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const apiError: ApiError = error.error?.code
        ? error.error
        : {
            code: 'NETWORK_ERROR',
            message: 'The service is unavailable. Try again shortly.',
          };
      return throwError(() => apiError);
    }),
  );
