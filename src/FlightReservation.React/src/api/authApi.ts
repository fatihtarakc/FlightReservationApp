import { api } from './apiClient';
import type { ApiResponse, SignInRequest, RegisterRequest, TokenModel, VerifyEmailRequest, ResetPasswordRequest } from '../types';

export const authApi = {
  signIn:               (data: SignInRequest)        => api.post<ApiResponse<TokenModel>>('account/sign-in',               data),
  register:             (data: RegisterRequest)      => api.post<ApiResponse<object>>    ('account/register',              data),
  verifyEmail:          (data: VerifyEmailRequest)   => api.post<ApiResponse<object>>    ('account/verify-code',           data),
  sendVerificationCode: (email: string)              => api.post<ApiResponse<object>>    (`account/send-verification-code?email=${encodeURIComponent(email)}&purpose=1&channel=1`, {}),
  resetPassword:        (data: ResetPasswordRequest) => api.post<ApiResponse<object>>    ('account/reset-password',        data),
};
