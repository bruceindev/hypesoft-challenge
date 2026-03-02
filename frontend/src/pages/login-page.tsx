import { Navigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuth } from "@/hooks/use-auth";
import { APP_ROUTES } from "@/lib/constants";

export default function LoginPage() {
  const auth = useAuth();

  if (auth.isAuthenticated) {
    return <Navigate to={APP_ROUTES.dashboard} replace />;
  }

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <Card className="w-full max-w-md" data-testid="login-card">
        <CardHeader>
          <CardTitle>Hypesoft</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-muted-foreground">Entre com sua conta Keycloak para acessar o painel.</p>
          <Button className="w-full" onClick={auth.login} data-testid="login-keycloak-btn">Entrar com Keycloak</Button>
        </CardContent>
      </Card>
    </div>
  );
}
