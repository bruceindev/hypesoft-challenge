import { Filter } from "lucide-react";

interface TransactionViewModel {
  id: string;
  customer: string;
  product: string;
  category: string;
  quantity: string;
  total: string;
  initials: string;
}

interface TransactionsTableProps {
  items: TransactionViewModel[];
}

export function TransactionsTable({ items }: TransactionsTableProps) {
  return (
    <section className="rounded-xl bg-white p-6 shadow-sm ring-1 ring-slate-100">
      <header className="mb-4 flex items-center justify-between">
        <h3 className="text-base font-semibold text-slate-900">Latest Transaction</h3>
        <button className="inline-flex items-center gap-2 rounded-lg border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600">
          <Filter className="h-3.5 w-3.5" />
          Filter
        </button>
      </header>

      <div className="overflow-x-auto">
        <table className="w-full min-w-[640px] text-left">
          <thead>
            <tr className="text-[11px] text-slate-400">
              <th className="pb-3 font-medium">Transaction ID</th>
              <th className="pb-3 font-medium">Customer</th>
              <th className="pb-3 font-medium">Product</th>
              <th className="pb-3 font-medium">Category</th>
              <th className="pb-3 font-medium">Qty</th>
              <th className="pb-3 text-right font-medium">Total</th>
            </tr>
          </thead>
          <tbody className="text-sm text-slate-700">
            {items.map((item) => (
              <tr key={item.id} className="border-t border-slate-100">
                <td className="py-3 text-xs text-slate-500">{item.id}</td>
                <td className="py-3">
                  <div className="flex items-center gap-2">
                    <div className="grid h-6 w-6 place-items-center rounded-full bg-indigo-100 text-[10px] font-semibold text-indigo-600">
                      {item.initials}
                    </div>
                    <span className="text-sm">{item.customer}</span>
                  </div>
                </td>
                <td className="py-3">{item.product}</td>
                <td className="py-3 text-slate-500">{item.category}</td>
                <td className="py-3 text-slate-500">{item.quantity}</td>
                <td className="py-3 text-right font-semibold text-slate-800">{item.total}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
