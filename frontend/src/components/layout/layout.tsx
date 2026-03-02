import { useMemo, useState } from "react";
import { Sidebar } from "@/components/dashboard/Sidebar";
import { Topbar } from "@/components/dashboard/Topbar";
import { useAuth } from "@/hooks/use-auth";

interface LayoutProps {
  children: React.ReactNode;
}

export function Layout({ children }: LayoutProps) {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const auth = useAuth();

  const primaryRole = useMemo(() => {
    return auth.user?.roles?.[0] ?? "Shop Admin";
  }, [auth.user?.roles]);

  return (
    <div className="min-h-screen bg-slate-100 text-slate-900">
      <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />

      <div className="lg:pl-64">
        <Topbar onOpenSidebar={() => setSidebarOpen(true)} userName={auth.user?.name} userRole={primaryRole} />
        <main className="p-4 sm:p-6">{children}</main>
      </div>
    </div>
  );
}
