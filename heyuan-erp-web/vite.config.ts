import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'node:path';

// Vite 配置：
// - 使用 Vue 插件
// - 设置别名 '@' -> src
// - Dev Server 端口与 /api 代理到后端（目标从环境变量 VITE_API_BASE 读取）
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd());
  const apiBase = env.VITE_API_BASE || 'http://localhost:5180';

  return {
    plugins: [vue()],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src'),
      },
    },
    server: {
      port: 5173,
      proxy: {
        '/api': {
          target: apiBase,
          changeOrigin: true,
          // 若后端非以 /api 为前缀，可在此重写路径
          // rewrite: (p) => p.replace(/^\/api/, ''),
        },
      },
    },
    preview: {
      port: 5173,
    },
  };
});
