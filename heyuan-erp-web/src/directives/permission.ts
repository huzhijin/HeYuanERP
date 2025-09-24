// 权限指令 v-permission：根据权限字符串（或数组）显示/隐藏元素
// 用法：
//   <button v-permission="'orders.create'">新建订单</button>
//   <button v-permission="['admin', 'orders.approve']">审批</button>
import type { Directive } from 'vue';
import { useAuthStore } from '@/store/auth';

function hasPermission(required: string | string[]): boolean {
  const auth = useAuthStore();
  const userPerms = auth.user?.permissions || [];
  const roles = auth.roles || [];
  if (roles.includes('Admin')) return true;
  const reqList = Array.isArray(required) ? required : [required];
  return reqList.some(p => userPerms.includes(p));
}

export const permission: Directive = {
  mounted(el, binding) {
    const ok = hasPermission(binding.value);
    if (!ok) {
      // 默认策略：隐藏元素
      el.parentNode && el.parentNode.removeChild(el);
    }
  },
};

export default permission;

