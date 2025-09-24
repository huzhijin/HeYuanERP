// 高级报表分析 API
import { http } from '../../lib/http'

export async function fetchProductSalesStats(params: { startDate: string; endDate: string }): Promise<any> {
  return await http.get('/api/ProductPrice/analytics/product-sales', params)
}

export async function fetchQuotationStats(params: { startDate: string; endDate: string }): Promise<any> {
  return await http.get('/api/ProductPrice/analytics/quotation-stats', params)
}

