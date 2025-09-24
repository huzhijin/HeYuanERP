// 说明：Axios 封装（统一响应/错误处理，中文注释）
// - 从环境变量读取 API 基地址：VITE_API_BASE_URL
// - 自动附加 JWT（Authorization: Bearer <token>）
// - 解析后端统一 Envelope：{ code, message, data, traceId }

import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios'

// 统一响应 Envelope 类型
export interface ApiEnvelope<T = any> {
  code: string
  message: string
  data: T
  traceId?: string
}

// 从环境变量读取 API 基地址
const baseURL = (import.meta as any).env?.VITE_API_BASE_URL || '/'

// 创建 Axios 实例
const http: AxiosInstance = axios.create({
  baseURL,
  timeout: 30000,
  withCredentials: false,
  headers: {
    'Content-Type': 'application/json',
    'Accept-Language': 'zh-CN,zh;q=0.9'
  }
})

// 本地存储的 JWT 键名
const TOKEN_KEY = 'HEYUANERP_TOKEN'

// 读取当前 token
function getToken(): string | null {
  try {
    return localStorage.getItem(TOKEN_KEY)
  } catch {
    return null
  }
}

// 设置/清除 token（同时更新默认请求头）
export function setAuthToken(token: string | null) {
  try {
    if (token) localStorage.setItem(TOKEN_KEY, token)
    else localStorage.removeItem(TOKEN_KEY)
  } catch {
    // 忽略本地存储异常
  }
  const t = token ?? getToken()
  if (t) http.defaults.headers.common['Authorization'] = `Bearer ${t}`
  else delete http.defaults.headers.common['Authorization']
}

// 初始化默认 token 头
setAuthToken(getToken())

// 请求拦截器：附加 Authorization/Trace-Id
http.interceptors.request.use((config) => {
  const token = getToken()
  if (token) {
    config.headers = config.headers || {}
    ;(config.headers as any)['Authorization'] = `Bearer ${token}`
  }
  return config
})

// 响应拦截器：统一 Envelope 处理
http.interceptors.response.use(
  (resp: AxiosResponse<ApiEnvelope>) => {
    const env = resp.data as any
    if (env && typeof env === 'object' && 'code' in env) {
      if (env.code === 'OK') return env.data
      const err: any = new Error(env.message || '请求失败')
      err.code = env.code
      err.traceId = env.traceId
      throw err
    }
    // 不符合 Envelope，则直接返回原始 data
    return resp.data as any
  },
  (error: AxiosError) => {
    // 统一错误转义
    const resp = error.response as AxiosResponse<ApiEnvelope> | undefined
    if (resp && resp.data && (resp.data as any).message) {
      const env = resp.data
      const err: any = new Error(env.message || '请求失败')
      err.code = env.code || resp.status
      err.traceId = env.traceId
      return Promise.reject(err)
    }
    return Promise.reject(error)
  }
)

// 便捷方法：GET
export async function get<T = any>(url: string, params?: any, config?: AxiosRequestConfig): Promise<T> {
  const resp = await http.get<any, T>(url, { params, ...(config || {}) })
  return resp
}

// 便捷方法：POST
export async function post<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
  const resp = await http.post<any, T>(url, data, config)
  return resp
}

// 导出默认实例（用于特殊需求）
export default http

