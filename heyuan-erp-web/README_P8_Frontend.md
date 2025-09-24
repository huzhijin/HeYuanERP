# 前端（P8 发票）运行与说明

## 依赖与启动
```bash
cd heyuan-erp-web
npm i
npm run dev
# 默认 http://localhost:5173
```

确保 `.env.development` 或环境变量 `VITE_API_BASE` 指向后端（默认 `http://localhost:5080`）。

## 路由与页面
- `/invoices` 发票列表（筛选状态、分页、打印）
- `/invoices/new` 开票表单（来源选择：订单/交货 → 行编辑 → 提交）
- `/invoices/:id` 发票详情（头/行展示、打印）

## 交互说明
- 列表页调用 `src/services/invoices.ts`，适配统一响应 Envelope；
- 开票表单为控制批次文件数量，直接用 `http.post('/api/Invoices', payload)` 提交；后续可抽取 `createInvoice` 至服务层；
- 打印按钮调用 `/api/invoices/{id}/print/pdf` 并在新窗口打开 PDF。

