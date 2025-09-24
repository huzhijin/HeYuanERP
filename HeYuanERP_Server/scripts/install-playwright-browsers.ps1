Param(
  [ValidateSet('chromium','firefox','webkit','all')]
  [string]$Browsers = 'chromium'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "[打印] 检测 dotnet 与 playwright CLI..."
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
  Write-Error "未检测到 dotnet，请先安装 .NET SDK 8.0+"
}

# 若 playwright 不存在，尝试安装 .NET 全局工具（Microsoft.Playwright.CLI）
if (-not (Get-Command playwright -ErrorAction SilentlyContinue)) {
  Write-Host "[打印] 未检测到 playwright CLI，尝试安装全局工具 Microsoft.Playwright.CLI..."
  try {
    dotnet tool update --global Microsoft.Playwright.CLI | Out-Null
  } catch {
    dotnet tool install --global Microsoft.Playwright.CLI | Out-Null
  }
  $env:PATH = "$HOME/.dotnet/tools;$env:PATH"
}

if (Get-Command playwright -ErrorAction SilentlyContinue) {
  Write-Host "[打印] 使用 playwright CLI 安装浏览器: $Browsers"
  playwright install $Browsers
  Write-Host "[打印] 浏览器安装完成。"
  exit 0
}

# 兜底：尝试使用构建产物中的 playwright 脚本
Write-Host "[打印] playwright CLI 不可用，尝试编译项目后使用脚本安装..."
dotnet build -c Release | Out-Null

$ps1 = Get-ChildItem -Path . -Recurse -Filter playwright.ps1 -ErrorAction SilentlyContinue | Select-Object -First 1
if ($ps1) {
  Write-Host "[打印] 通过 $($ps1.FullName) 安装浏览器: $Browsers"
  pwsh -File $ps1.FullName install $Browsers
  exit 0
}

$sh = Get-ChildItem -Path . -Recurse -Filter playwright.sh -ErrorAction SilentlyContinue | Select-Object -First 1
if ($sh) {
  Write-Host "[打印] 通过 $($sh.FullName) 安装浏览器: $Browsers"
  bash $sh.FullName install $Browsers
  exit 0
}

Write-Error "未能找到 playwright 安装脚本，请确认已引用 Microsoft.Playwright 包并完成一次构建。"
exit 2

