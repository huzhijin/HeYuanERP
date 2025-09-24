// 认证状态仓库：管理 token、用户信息、角色与权限
import { defineStore } from 'pinia';

export interface UserInfo {
  id: string;
  loginId: string;
  name: string;
  permissions: string[];
}

export interface AuthState {
  token: string;
  user: UserInfo | null;
  roles: string[];
}

const TOKEN_KEY = 'HY_TOKEN';

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: localStorage.getItem(TOKEN_KEY) || '',
    user: null,
    roles: [],
  }),
  getters: {
    isAuthenticated: (s) => !!s.token,
  },
  actions: {
    setToken(token: string) {
      this.token = token;
      localStorage.setItem(TOKEN_KEY, token);
    },
    clear() {
      this.token = '';
      this.user = null;
      this.roles = [];
      localStorage.removeItem(TOKEN_KEY);
    },
    setUser(user: UserInfo, roles: string[] = []) {
      this.user = user;
      this.roles = roles;
    },
  },
});

