export type RequestStatus = 'idle' | 'loading' | 'success' | 'error';

export interface RequestState {
  status: RequestStatus;
  message?: string;
}

export const idleRequest: RequestState = { status: 'idle' };
