// axios 封装：统一基址、鉴权、响应 Envelope 解包与错误处理
import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';

// 开发环境：通过 Vite 代理转发 /api → 后端，避免跨域；
// 优先使用显式的 VITE_API_BASE_URL（开发/生产一致），其次使用 VITE_API_BASE；
// 若均未提供且为开发模式，退回相对路径（走 dev proxy）。
const API_BASE = (import.meta as any).env?.VITE_API_BASE_URL
  || (import.meta as any).env?.VITE_API_BASE
  || (import.meta.env.DEV ? '' : '');
const TOKEN_KEY = 'HY_TOKEN'; // 与 store 中保持一致

function getToken(): string | null {
  try {
    return localStorage.getItem(TOKEN_KEY);
  } catch {
    return null;
  }
}

const instance: AxiosInstance = axios.create({
  baseURL: API_BASE,
  timeout: 15000,
});

// 请求拦截：加 Authorization 头
instance.interceptors.request.use((config: AxiosRequestConfig) => {
  const token = getToken();
  if (token) {
    config.headers = config.headers || {};
    (config.headers as any)['Authorization'] = `Bearer ${token}`;
  }
  return config;
});

// 响应拦截：
// - Content-Type 为 PDF/二进制则直接返回 data
// - JSON 且为统一 Envelope：code!==OK -> 抛错；否则返回 data
instance.interceptors.response.use(
  (resp: AxiosResponse) => {
    const ct = resp.headers['content-type'] || '';
    if (ct.includes('application/pdf') || ct.includes('octet-stream')) {
      return resp.data;
    }
    const body = resp.data;
    if (body && typeof body === 'object' && 'code' in body) {
      if (body.code !== 'OK') {
        const err = new Error(body.message || '请求失败');
        (err as any).traceId = body.traceId;
        (err as any).response = body;
        throw err;
      }
      return body.data;
    }
    return body;
  },
  (error: AxiosError) => {
    const status = error.response?.status;
    // 未授权：清理本地并跳转登录
    if (status === 401 || status === 403) {
      try { localStorage.removeItem(TOKEN_KEY); } catch {}
      const redirect = encodeURIComponent(location.pathname + location.search);
      if (!location.pathname.startsWith('/login')) {
        location.href = `/login?redirect=${redirect}`;
      }
    }
    return Promise.reject(error);
  }
);

export default instance;

export const http = {
  get<T = any>(url: string, config?: AxiosRequestConfig) {
    return instance.get<T>(url, config).then(r => r as any as T);
  },
  post<T = any>(url: string, data?: any, config?: AxiosRequestConfig) {
    return instance.post<T>(url, data, config).then(r => r as any as T);
  },
  put<T = any>(url: string, data?: any, config?: AxiosRequestConfig) {
    return instance.put<T>(url, data, config).then(r => r as any as T);
  },
  delete<T = any>(url: string, config?: AxiosRequestConfig) {
    return instance.delete<T>(url, config).then(r => r as any as T);
  },
};
