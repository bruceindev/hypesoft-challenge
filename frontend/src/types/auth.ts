export type UserRole = "Admin" | "Manager" | "User";

export interface AuthUser {
  id: string;
  name: string;
  email?: string;
  roles: UserRole[];
}

export interface AuthState {
  isAuthenticated: boolean;
  isLoading: boolean;
  token?: string;
  user?: AuthUser;
}
