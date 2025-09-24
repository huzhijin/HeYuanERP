// 对账差异 API（最小可用）
import { http } from '../../lib/http'

export async function listUnresolvedDifferences(): Promise<any[]> {
  const res:any = await http.get('/api/Reconciliation/differences/unresolved')
  return (res?.data || res?.items || res) as any[]
}

export async function resolveDifference(id: string, resolution: string, handledBy: string): Promise<any> {
  return await http.post(`/api/Reconciliation/differences/${id}/resolve`, { resolution, handledBy })
}

