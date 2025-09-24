// 中文说明：
// 统一的 Axios 封装（支持统一响应ApiResponse、JWT、组织头、文件下载）。
// 注意：
// - 基础地址从环境变量 VITE_API_BASE_URL 读取；
// - Authorization 从本地存储读取并自动附加；
// - 统一响应：后端约定 code=0 表示成功；否则抛出错误；
// - 下载：提供 getBlob/postBlob 以获取二进制并返回文件名。

import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';

// 环境变量：后端 API 基础地址
const baseURL = (import.meta as any).env?.VITE_API_BASE_URL || '/';

// 本地存储键名
const TOKEN_KEY = 'HEYUANERP_TOKEN';
const ORG_KEY = 'HEYUANERP_ORG_ID';

// 自定义错误类型（包含业务码与 TraceId）
export class ApiError extends Error {
  code: number;
  traceId?: string;
  constructor(message: string, code = -1, traceId?: string) {
    super(message);
    this.name = 'ApiError';
    this.code = code;
    this.traceId = traceId;
  }
}

// 创建 axios 实例
const instance: AxiosInstance = axios.create({
  baseURL,
  timeout: 15000,
  withCredentials: false,
  headers: {
    'X-Requested-With': 'XMLHttpRequest',
  },
});

// 请求拦截：附加认证与组织标识
instance.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) {
    config.headers = config.headers || {};
    (config.headers as any).Authorization = `Bearer ${token}`;
  }
  const orgId = localStorage.getItem(ORG_KEY);
  if (orgId) {
    config.headers = config.headers || {};
    (config.headers as any)['X-Org-Id'] = orgId;
  }
  // 语言（可选）：简体中文
  config.headers = config.headers || {};
  (config.headers as any)['Accept-Language'] = 'zh-CN';
  return config;
});

// 响应拦截：统一错误处理（网络/超时/状态码）
instance.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response) {
      // 非 2xx 状态码
      const status = error.response.status;
      let msg = `请求失败（HTTP ${status}）`;
      if (status === 401) msg = '未认证或登录已过期，请重新登录';
      if (status === 403) msg = '无权限访问该资源';
      return Promise.reject(new ApiError(msg, status));
    }
    if (error.code === 'ECONNABORTED') {
      return Promise.reject(new ApiError('请求超时，请检查网络后重试'));
    }
    return Promise.reject(new ApiError(error.message || '网络异常'));
  }
);

// 统一解包函数：将 ApiResponse<T> => T
async function unwrapApi<T>(res: AxiosResponse<any>): Promise<T> {
  const payload = res.data as { code?: number; message?: string; data?: T; traceId?: string };
  // 文件/流响应不在此处理
  if (res.request && (res.request.responseType === 'blob' || (res.config as any).responseType === 'blob')) {
    throw new Error('请使用 getBlob/postBlob 获取二进制响应');
  }
  if (payload && typeof payload.code === 'number') {
    if (payload.code === 0) return payload.data as T;
    throw new ApiError(payload.message || '请求失败', payload.code, payload.traceId);
  }
  // 若后端未按约定返回统一响应，则直接返回 data
  return res.data as T;
}

// 解析 Content-Disposition 获取文件名
function parseFileNameFromDisposition(disposition?: string | null): string | null {
  if (!disposition) return null;
  // 示例：attachment; filename="xxx.csv"; filename*=UTF-8''xxx.csv
  const matchStar = /filename\*=UTF-8''([^;\n]+)/i.exec(disposition);
  if (matchStar && matchStar[1]) {
    try {
      return decodeURIComponent(matchStar[1]);
    } catch { /* 忽略解码错误 */ }
  }
  const match = /filename="?([^";]+)"?/i.exec(disposition);
  return match && match[1] ? match[1] : null;
}

// 对外导出的小工具方法
export const http = {
  // JSON GET（返回统一响应 data）
  async get<T>(url: string, params?: any, config?: AxiosRequestConfig): Promise<T> {
    const res = await instance.get(url, { params, ...(config || {}) });
    return unwrapApi<T>(res);
  },

  // JSON POST（返回统一响应 data）
  async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const res = await instance.post(url, data, config);
    return unwrapApi<T>(res);
  },

  // JSON PUT
  async put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const res = await instance.put(url, data, config);
    return unwrapApi<T>(res);
  },

  // JSON DELETE
  async del<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const res = await instance.delete(url, config);
    return unwrapApi<T>(res);
  },

  // 表单上传（multipart/form-data）
  async postForm<T>(url: string, form: FormData, config?: AxiosRequestConfig): Promise<T> {
    const res = await instance.post(url, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
      ...(config || {}),
    });
    return unwrapApi<T>(res);
  },

  // 获取二进制（下载）
  async getBlob(url: string, params?: any, config?: AxiosRequestConfig): Promise<{ blob: Blob; fileName: string | null; contentType: string | null }> {
    const res = await instance.get(url, { params, responseType: 'blob', ...(config || {}) });
    const contentType = (res.headers['content-type'] || res.headers['Content-Type'] || null) as string | null;
    const disposition = (res.headers['content-disposition'] || res.headers['Content-Disposition'] || null) as string | null;
    const fileName = parseFileNameFromDisposition(disposition);
    return { blob: res.data as Blob, fileName, contentType };
  },

  // 设置/清除令牌与组织标识
  setToken(token: string | null) {
    if (token) localStorage.setItem(TOKEN_KEY, token);
    else localStorage.removeItem(TOKEN_KEY);
  },
  setOrgId(orgId: string | null) {
    if (orgId) localStorage.setItem(ORG_KEY, orgId);
    else localStorage.removeItem(ORG_KEY);
  },

  // 直接暴露 axios 实例（少量高级用法）
  instance,
};

export default http;
