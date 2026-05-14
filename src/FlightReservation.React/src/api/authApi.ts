import { api } from './apiClient';
import type { ApiResponse, SignInDto, RegisterDto, TokenDto, VerifyEmailDto, ResetPasswordDto } from '../types';

export const authApi = {
  signIn:               (data: SignInDto)        => api.post<ApiResponse<TokenDto>>('account/sign-in',               data),
  register:             (data: RegisterDto)      => api.post<ApiResponse<object>>  ('account/register',              data),
  verifyEmail:          (data: VerifyEmailDto)   => api.post<ApiResponse<object>>  ('account/verify-code',           data),
  sendVerificationCode: (email: string)          => api.post<ApiResponse<object>>  (`account/send-verification-code?email=${encodeURIComponent(email)}&purpose=1&channel=1`, {}),
  resetPassword:        (data: ResetPasswordDto) => api.post<ApiResponse<object>>  ('account/reset-password',        data),
};
