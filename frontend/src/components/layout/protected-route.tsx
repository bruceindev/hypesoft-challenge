import { Navigate } from "react-router-dom";
import { APP_ROUTES } from "@/lib/constants";
import { useAuth } from "@/hooks/use-auth";
import { hasPermission } from "@/lib/roles";
import { PageSkeleton } from "@/components/layout/page-skeleton";

interface ProtectedRouteProps {
  permission?: string;
  children: React.ReactNode;
}

export const ProtectedRoute = ({ permission, children }: ProtectedRouteProps) => {
  const auth = useAuth();

  if (auth.isLoading) {
    return <PageSkeleton />;
  }

  if (!auth.isAuthenticated) {
    return <Navigate to={APP_ROUTES.login} replace />;
  }

  if (permission && !hasPermission(auth.user?.roles ?? [], permission)) {
    return <Navigate to={APP_ROUTES.forbidden} replace />;
  }

  return <>{children}</>;
};
