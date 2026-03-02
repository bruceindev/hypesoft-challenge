import { Bar, BarChart, Cell, ResponsiveContainer, XAxis, YAxis } from "recharts";

interface CountryChartPoint {
  code: string;
  value: number;
  active?: boolean;
}

interface CountriesChartProps {
  items: CountryChartPoint[];
}

export function CountriesChart({ items }: CountriesChartProps) {
  return (
    <section className="rounded-xl bg-white p-6 shadow-sm ring-1 ring-slate-100">
      <h3 className="mb-6 text-base font-semibold text-slate-900">Customer’s Countries</h3>

      <div className="h-52">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={items} margin={{ top: 0, right: 0, left: -28, bottom: 0 }}>
            <YAxis
              axisLine={false}
              tickLine={false}
              tick={{ fill: "#94A3B8", fontSize: 11 }}
              tickFormatter={(value) => `${Math.round(value / 1000)}k`}
            />
            <XAxis axisLine={false} tickLine={false} dataKey="code" tick={{ fill: "#94A3B8", fontSize: 11 }} />
            <Bar dataKey="value" radius={[6, 6, 0, 0]}>
              {items.map((entry) => (
                <Cell key={entry.code} fill={entry.active ? "#6366F1" : "#E2E8F0"} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </div>
    </section>
  );
}
