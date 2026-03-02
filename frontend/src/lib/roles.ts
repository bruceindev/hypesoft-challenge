import type { UserRole } from "@/types/auth";

export const ROLE_PERMISSIONS: Record<UserRole, string[]> = {
  Admin: ["products:create", "products:update", "products:delete", "categories:manage", "stock:update"],
  Manager: ["products:create", "products:update", "stock:update"],
  User: [],
};

export const hasPermission = (roles: UserRole[], permission: string): boolean => {
  return roles.some((role) => ROLE_PERMISSIONS[role].includes(permission));
};
