# heyuan-erp-web - P9 前端说明（收款页/对账单导出）

> 本文档描述 P9 增量前端实现：收款列表与创建、对账单导出页面、Axios 封装与路由。所有文案与注释均为中文。

## 目录与模块

- 核心封装
  - `src/lib/http.ts` 统一 Axios 封装（JWT、组织头、统一响应、Blob 下载）
- 类型与 API
  - `src/types/payments.ts` 前端类型定义
  - `src/api/payments.ts` 收款/对账 API 封装
- 状态
  - `src/store/payments.ts` Pinia 仓库（查询、创建、导出）
- 页面与组件
  - `src/pages/payments/PaymentList.vue` 收款列表/筛选/分页
  - `src/pages/payments/PaymentCreate.vue` 收款创建（multipart 上传）
  - `src/pages/reconciliation/ReconciliationExport.vue` 对账单导出
  - `src/components/UploadAttachment.vue` 通用附件选择组件
  - `src/router/modules/payments.ts` 路由模块
- 工具
  - `src/utils/download.ts` 浏览器下载工具

## 环境变量

- `VITE_API_BASE_URL` 后端 API 基础地址（示例：`http://localhost:5000`）
- 认证：本项目默认从 `localStorage` 读取 `HEYUANERP_TOKEN` 并自动附加 `Authorization: Bearer <token>`
- 组织：从 `localStorage` 读取 `HEYUANERP_ORG_ID` 并附加 `X-Org-Id`

注意：Vite 仅会读取项目根的 `.env*` 文件；仓库根的 `.env` 示例仅供参考，请在 `heyuan-erp-web` 目录下另行创建 `.env.local`。

## 本地运行

```bash
cd heyuan-erp-web
npm ci
echo "VITE_API_BASE_URL=http://localhost:5000" > .env.local
npm run dev
# 打开 http://localhost:5173 （端口以实际输出为准）
```

## 使用说明

- 收款列表：导航到 `/payments`，可按方式/金额/日期/关键字筛选并分页；支持导出对账单
- 新增收款：导航到 `/payments/create`，填写方式/金额/日期/备注，选择多个附件后提交
- 对账导出：导航到 `/reconciliation/export`，选择日期范围与方式后导出 CSV

## 验证步骤

1) 在浏览器 `localStorage` 设置令牌（如后端暂未接入认证，可填任意非空值）：

   - `localStorage.setItem('HEYUANERP_TOKEN','dev-token')`
   - 可选：`localStorage.setItem('HEYUANERP_ORG_ID','demo-org')`

2) 列表页应能加载数据；创建成功后自动刷新列表；导出后浏览器自动下载 `reconciliation_*.csv`

## 接口契约与错误处理

- 统一响应：`{ code: 0, message: string, data: any, traceId?: string }`
- Axios 封装在收到 `code != 0` 时会抛出 `ApiError`，组件捕获后统一弹出提示
- 下载：`http.getBlob` 会解析 `Content-Disposition` 获取文件名

