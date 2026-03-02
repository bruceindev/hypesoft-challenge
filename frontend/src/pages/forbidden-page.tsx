import { Link } from "react-router-dom";
import { APP_ROUTES } from "@/lib/constants";

export default function ForbiddenPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4">
      <h1 className="text-2xl font-semibold">Acesso negado</h1>
      <p className="text-muted-foreground">Você não tem permissão para acessar este conteúdo.</p>
      <Link className="text-primary underline" to={APP_ROUTES.dashboard}>Voltar para dashboard</Link>
    </div>
  );
}
