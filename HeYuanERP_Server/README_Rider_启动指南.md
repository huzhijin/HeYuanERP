# Rider 启动指南（SQLite 开发环境）

## 1. 打开解决方案
- 路径：`HeYuanERP_Server/HeYuanERP.sln`

## 2. 使用共享运行配置
- Rider 菜单：Run → Run Configurations → 选择 `HeYuanERP.Api (SQLite Dev)`
- 该配置已包含：
  - Working Directory：`src/HeYuanERP.Api`
  - 环境变量：
    - `ASPNETCORE_ENVIRONMENT=Development`
    - `ASPNETCORE_URLS=http://localhost:5180`
    - `CONNECTION_STRING=Data Source=heyuanerp_dev.db`
    - `JWT__ISSUER=heyuanerp`
    - `JWT__AUDIENCE=heyuanerp.web`
    - `JWT__SECRET=LocalDev_ChangeMe_123456`
    - `CORS__ALLOWEDORIGINS=http://localhost:5173`

## 3. 启动与验证
- 点击 ▶ 运行后端
- 打开 `http://localhost:5180/swagger` 查看接口
- 登录接口：`POST /api/auth/login`，默认账号：`admin / CHANGE_ME`

## 4. 注意
- 若要修改端口或连接串，直接在运行配置里改环境变量，或在 `src/HeYuanERP.Api/appsettings.json` 中调整默认值即可。
- 首次运行会自动创建 `heyuanerp_dev.db` 并播种开发账号与权限。

