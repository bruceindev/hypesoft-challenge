import { ChevronDown, Filter } from "lucide-react";

interface TopProductViewModel {
  id: string;
  name: string;
  category: string;
  soldLabel: string;
  revenueLabel: string;
  initials: string;
}

interface TopProductsProps {
  items: TopProductViewModel[];
}

export function TopProducts({ items }: TopProductsProps) {
  return (
    <section className="rounded-xl bg-white p-6 shadow-sm ring-1 ring-slate-100">
      <header className="mb-5 flex flex-wrap items-center justify-between gap-3">
        <h3 className="text-base font-semibold text-slate-900">Top Products</h3>
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

      <div className="space-y-3">
        <div className="grid grid-cols-[1.6fr_0.8fr_0.8fr] text-[11px] text-slate-400">
          <span>Product</span>
          <span>Sold</span>
          <span className="text-right">Revenue</span>
        </div>

        {items.map((item) => (
          <article key={item.id} className="grid grid-cols-[1.6fr_0.8fr_0.8fr] items-center gap-2">
            <div className="flex items-center gap-3">
              <div className="grid h-10 w-10 place-items-center rounded-md bg-indigo-100 text-xs font-semibold text-indigo-600">
                {item.initials}
              </div>
              <div>
                <p className="text-sm font-medium text-slate-800">{item.name}</p>
                <p className="text-xs text-slate-400">{item.category}</p>
              </div>
            </div>

            <span className="w-fit rounded-md bg-emerald-50 px-2 py-1 text-xs font-medium text-emerald-600">{item.soldLabel}</span>
            <span className="text-right text-sm font-semibold text-slate-800">{item.revenueLabel}</span>
          </article>
        ))}
      </div>
    </section>
  );
}
