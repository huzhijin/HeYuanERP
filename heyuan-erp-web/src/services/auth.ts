// 认证服务封装：登录获取 JWT 与用户信息
// 使用 utils/request 的 http 实例，自动解包统一响应

import { http } from '@/utils/request';

export interface LoginRequest {
  loginId: string;
  password: string;
}

export interface UserDto {
  id: string;
  loginId: string;
  name: string;
  permissions: string[];
}

export interface LoginResultDto {
  token: string;
  user: UserDto;
  roles: string[];
}

export function login(body: LoginRequest) {
  return http.post<LoginResultDto>('/api/auth/login', body);
}

