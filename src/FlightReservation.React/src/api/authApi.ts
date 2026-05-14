import apiClient from './apiClient';
import type { ApiResponse, SignInDto, RegisterDto, TokenDto, VerifyEmailDto, ResetPasswordDto } from '../types';

export const authApi = {
  signIn: (data: SignInDto) =>
    apiClient.post<ApiResponse<TokenDto>>('account/sign-in', data),

  register: (data: RegisterDto) =>
    apiClient.post<ApiResponse<object>>('account/register', data),

  verifyEmail: (data: VerifyEmailDto) =>
    apiClient.post<ApiResponse<object>>('account/verify-email', data),

  sendVerificationCode: (email: string) =>
    apiClient.post<ApiResponse<object>>('account/send-verification-code', { email }),

  resetPassword: (data: ResetPasswordDto) =>
    apiClient.post<ApiResponse<object>>('account/reset-password', data),
};
