import { Filter, ChevronDown } from "lucide-react";
import { CartesianGrid, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

export interface SalesChartPoint {
  label: string;
  primary: number;
  secondary: number;
}

interface SalesChartProps {
  points: SalesChartPoint[];
  totalLabel: string;
  growthLabel: string;
}

interface ChartTooltipProps {
  active?: boolean;
  label?: string;
  payload?: Array<{ name: string; value: number }>;
}

function ChartTooltip({ active, label, payload }: ChartTooltipProps) {
  if (!active || !payload || payload.length === 0) {
    return null;
  }

  return (
    <div className="rounded-xl bg-slate-900 px-3 py-2 text-xs text-slate-100 shadow-lg">
      <p className="mb-1 font-semibold">{label}</p>
      {payload.map((entry) => (
        <p key={entry.name} className="flex items-center justify-between gap-4 text-[11px] text-slate-200">
          <span>{entry.name}:</span>
          <span>${entry.value.toFixed(2)}</span>
        </p>
      ))}
    </div>
  );
}

export function SalesChart({ points, totalLabel, growthLabel }: SalesChartProps) {
  return (
    <section className="rounded-xl bg-white p-6 shadow-sm ring-1 ring-slate-100">
      <header className="mb-4 flex flex-wrap items-center justify-between gap-3">
        <div>
          <h3 className="text-base font-semibold text-slate-900">Sales Insight</h3>
          <p className="mt-1 text-4xl font-semibold tracking-tight text-slate-900">{totalLabel}</p>
          <p className="mt-1 text-xs text-slate-400">
            <span className="font-semibold text-emerald-500">{growthLabel}</span>
            <span className="ml-1">from last month</span>
          </p>
        </div>

        <div className="flex items-center gap-2">
          <button className="inline-flex items-center gap-2 rounded-lg border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600">
            Weekly
            <ChevronDown className="h-3.5 w-3.5" />
          </button>
          <button className="inline-flex items-center gap-2 rounded-lg border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600">
            <Filter className="h-3.5 w-3.5" />
            Filter
          </button>
        </div>
      </header>

      <div className="mb-2 flex items-center justify-end gap-4 text-xs text-slate-500">
        <span className="inline-flex items-center gap-1">
          <span className="h-2 w-2 rounded-full bg-indigo-500" />
          Shopee
        </span>
        <span className="inline-flex items-center gap-1">
          <span className="h-2 w-2 rounded-full bg-violet-500" />
          TikTok Shop
        </span>
      </div>

      <div className="h-52">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={points} margin={{ top: 10, right: 8, left: -18, bottom: 0 }}>
            <CartesianGrid vertical={false} strokeDasharray="2 10" stroke="#EEF2FF" />
            <XAxis dataKey="label" tickLine={false} axisLine={false} tick={{ fill: "#94A3B8", fontSize: 12 }} />
            <YAxis tickLine={false} axisLine={false} tick={{ fill: "#CBD5E1", fontSize: 11 }} />
            <Tooltip cursor={false} content={<ChartTooltip />} />
            <Line type="monotone" dataKey="primary" stroke="#6366F1" strokeWidth={3} dot={false} activeDot={{ r: 4 }} />
            <Line type="monotone" dataKey="secondary" stroke="#A855F7" strokeWidth={3} dot={false} activeDot={{ r: 4 }} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </section>
  );
}
