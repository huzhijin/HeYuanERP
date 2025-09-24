#!/usr/bin/env bash
set -euo pipefail

# 安装 Playwright 所需浏览器（Chromium）
# 用法：bash scripts/install-playwright-browsers.sh [chromium|firefox|webkit|all]

BROWSERS="${1:-chromium}"

echo "[打印] 检测 dotnet 与 playwright CLI..."
if ! command -v dotnet >/dev/null 2>&1; then
  echo "[错误] 未检测到 dotnet，请先安装 .NET SDK 8.0+" >&2
  exit 1
fi

# 若 playwright 不存在，尝试安装 .NET 全局工具（Microsoft.Playwright.CLI）
if ! command -v playwright >/dev/null 2>&1; then
  echo "[打印] 未检测到 playwright CLI，尝试安装全局工具 Microsoft.Playwright.CLI..."
  dotnet tool update --global Microsoft.Playwright.CLI >/dev/null 2>&1 || \
  dotnet tool install --global Microsoft.Playwright.CLI
  export PATH="$HOME/.dotnet/tools:$PATH"
fi

if command -v playwright >/dev/null 2>&1; then
  echo "[打印] 使用 playwright CLI 安装浏览器: $BROWSERS"
  playwright install "$BROWSERS"
  echo "[打印] 浏览器安装完成。"
  exit 0
fi

# 兜底：尝试使用构建产物中的 playwright 脚本
echo "[打印] playwright CLI 不可用，尝试编译项目后使用脚本安装..."
dotnet build -c Release || true

PLAYWRIGHT_PS1=$(find . -path "*/bin/*/playwright.ps1" -print -quit || true)
PLAYWRIGHT_SH=$(find . -path "*/bin/*/playwright.sh" -print -quit || true)

if [[ -n "$PLAYWRIGHT_PS1" ]]; then
  echo "[打印] 通过 $PLAYWRIGHT_PS1 安装浏览器: $BROWSERS"
  pwsh -File "$PLAYWRIGHT_PS1" install "$BROWSERS"
  exit 0
fi

if [[ -n "$PLAYWRIGHT_SH" ]]; then
  echo "[打印] 通过 $PLAYWRIGHT_SH 安装浏览器: $BROWSERS"
  bash "$PLAYWRIGHT_SH" install "$BROWSERS"
  exit 0
fi

echo "[错误] 未能找到 playwright 安装脚本，请确认已引用 Microsoft.Playwright 包并完成一次构建。" >&2
exit 2

