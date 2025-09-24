// 中文说明：
// 浏览器端通用下载工具：支持 Blob/ArrayBuffer 保存为本地文件。

/**
 * 将 Blob/ArrayBuffer 触发浏览器下载。
 * @param data Blob 或 ArrayBuffer
 * @param fileName 文件名
 * @param contentType MIME 类型（可选）
 */
export function downloadBlob(data: Blob | ArrayBuffer, fileName: string, contentType?: string) {
  let blob: Blob;
  if (data instanceof Blob) {
    blob = data;
  } else {
    blob = new Blob([data], { type: contentType || 'application/octet-stream' });
  }

  // IE 兼容分支（可选）
  const navAny = navigator as any;
  if (navAny.msSaveOrOpenBlob) {
    navAny.msSaveOrOpenBlob(blob, fileName);
    return;
  }

  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.style.display = 'none';
  a.href = url;
  a.download = fileName || `download_${Date.now()}`;
  document.body.appendChild(a);
  a.click();
  window.URL.revokeObjectURL(url);
  a.remove();
}

