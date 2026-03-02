import { MoreHorizontal } from "lucide-react";

interface StatCardProps {
  label: string;
  value: string;
  growth: string;
  positive?: boolean;
}

export function StatCard({ label, value, growth, positive = true }: StatCardProps) {
  return (
    <article className="rounded-xl bg-white p-6 shadow-sm ring-1 ring-slate-100">
      <header className="mb-4 flex items-center justify-between">
        <h3 className="text-sm font-medium text-slate-700">{label}</h3>
        <button className="text-slate-400 transition hover:text-slate-500" aria-label={`More options ${label}`}>
          <MoreHorizontal className="h-4 w-4" />
        </button>
      </header>

      <p className="text-4xl font-semibold tracking-tight text-slate-900">{value}</p>
      <p className="mt-2 text-xs text-slate-400">
        <span className={positive ? "font-semibold text-emerald-500" : "font-semibold text-rose-500"}>{growth}</span>
        <span className="ml-1">from last month</span>
      </p>
    </article>
  );
}
