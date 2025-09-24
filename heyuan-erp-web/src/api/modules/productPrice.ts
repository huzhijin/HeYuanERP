// 产品与价格 API（最小可用列表）
import { http } from '../../lib/http'

export interface ProductCategory {
  id: number
  name: string
  code: string
  description?: string
  parentId?: number | null
  isActive: boolean
}

export interface ProductItem {
  id: string
  code: string
  name: string
  categoryId?: number
  unit?: string
  active?: boolean
  safetyStock?: number
}

export interface PriceStrategy {
  id: number
  name: string
  type: string
  effectiveDate?: string
  expiryDate?: string
  isActive: boolean
}

export interface Quotation {
  id: number
  quotationNumber: string
  quotationTitle?: string
  accountId: number
  quoteDate: string
  validUntil?: string
  status: string
  totalAmount: number
}

export interface Pagination<T> { items: T[]; page: number; size: number; total: number }

const BASE = '/api/ProductPrice'

// 分类
export async function listCategories(): Promise<ProductCategory[]> {
  return await http.get(`${BASE}/categories`)
}

export async function createCategory(body: Partial<ProductCategory>): Promise<any> {
  return await http.post(`${BASE}/categories`, body)
}

export async function updateCategory(id: number, body: Partial<ProductCategory>): Promise<any> {
  return await http.put(`${BASE}/categories/${id}`, body)
}

export async function deleteCategory(id: number): Promise<any> {
  return await http.del(`${BASE}/categories/${id}`)
}

// 产品（分页）
export async function listProducts(page = 1, pageSize = 50): Promise<Pagination<ProductItem>> {
  return await http.get(`${BASE}/products`, { pageNumber: page, pageSize })
}

// 产品搜索（按代码/名称模糊）
export async function searchProducts(searchTerm: string): Promise<ProductItem[]> {
  return await http.get(`${BASE}/products/search`, { searchTerm })
}

export async function createProduct(body: Partial<ProductItem>): Promise<any> {
  return await http.post(`${BASE}/products`, body)
}

export async function updateProduct(id: string, body: Partial<ProductItem>): Promise<any> {
  return await http.put(`${BASE}/products/${id}`, body)
}

export async function deleteProduct(id: string): Promise<any> {
  return await http.del(`${BASE}/products/${id}`)
}

export async function updateStock(id: string, newStock: number): Promise<any> {
  return await http.put(`${BASE}/products/${id}/stock`, newStock)
}

// 价格策略
export async function listStrategies(): Promise<PriceStrategy[]> {
  return await http.get(`${BASE}/strategies`)
}

export async function createStrategy(body: Partial<PriceStrategy>): Promise<any> {
  return await http.post(`${BASE}/strategies`, body)
}

export async function updateStrategy(id: number, body: Partial<PriceStrategy>): Promise<any> {
  return await http.put(`${BASE}/strategies/${id}`, body)
}

export async function deleteStrategy(id: number): Promise<any> {
  return await http.del(`${BASE}/strategies/${id}`)
}

// 报价单（按编号查询）
export async function getQuotationByNumber(no: string): Promise<Quotation | null> {
  return await http.get(`${BASE}/quotations/by-number/${encodeURIComponent(no)}`)
}

export async function createQuotation(body: Partial<Quotation>): Promise<any> {
  return await http.post(`${BASE}/quotations`, body)
}

export async function updateQuotation(id: number, body: Partial<Quotation>): Promise<any> {
  return await http.put(`${BASE}/quotations/${id}`, body)
}

export async function deleteQuotation(id: number): Promise<any> {
  return await http.del(`${BASE}/quotations/${id}`)
}
