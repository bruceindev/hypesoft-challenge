import { lazy, Suspense } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import { Layout } from "@/components/layout/layout";
import { PageSkeleton } from "@/components/layout/page-skeleton";
import { ProtectedRoute } from "@/components/layout/protected-route";
import { APP_ROUTES } from "@/lib/constants";

const LoginPage = lazy(() => import("@/pages/login-page"));
const DashboardPage = lazy(() => import("@/pages/dashboard-page"));
const ProductsPage = lazy(() => import("@/pages/products-page"));
const CategoriesPage = lazy(() => import("@/pages/categories-page"));
const ForbiddenPage = lazy(() => import("@/pages/forbidden-page"));

function App() {
  return (
    <Suspense fallback={<PageSkeleton />}>
      <Routes>
        <Route path={APP_ROUTES.login} element={<LoginPage />} />
        <Route path={APP_ROUTES.forbidden} element={<ForbiddenPage />} />

        <Route
          path="/"
          element={
            <ProtectedRoute>
              <Layout>
                <Navigate to={APP_ROUTES.dashboard} replace />
              </Layout>
            </ProtectedRoute>
          }
        />

        <Route
          path={APP_ROUTES.dashboard}
          element={
            <ProtectedRoute>
              <Layout>
                <DashboardPage />
              </Layout>
            </ProtectedRoute>
          }
        />
        <Route
          path={APP_ROUTES.products}
          element={
            <ProtectedRoute>
              <Layout>
                <ProductsPage />
              </Layout>
            </ProtectedRoute>
          }
        />
        <Route
          path={APP_ROUTES.categories}
          element={
            <ProtectedRoute permission="categories:manage">
              <Layout>
                <CategoriesPage />
              </Layout>
            </ProtectedRoute>
          }
        />
      </Routes>
    </Suspense>
  );
}

export default App;
