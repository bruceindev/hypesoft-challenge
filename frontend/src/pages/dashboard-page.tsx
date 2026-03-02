import { CountriesChart } from "@/components/dashboard/CountriesChart";
import { SalesChart } from "@/components/dashboard/SalesChart";
import { StatCard } from "@/components/dashboard/StatCard";
import { TopProducts } from "@/components/dashboard/TopProducts";
import { TransactionsTable } from "@/components/dashboard/TransactionsTable";
import { PageSkeleton } from "@/components/layout/page-skeleton";
import { useDashboard } from "@/hooks/use-dashboard";
import { currency } from "@/utils/currency";

const fallbackSalesPoints = [
  { label: "Mon", primary: 0, secondary: 0 },
  { label: "Tue", primary: 0, secondary: 0 },
  { label: "Wed", primary: 0, secondary: 0 },
  { label: "Thu", primary: 0, secondary: 0 },
  { label: "Fri", primary: 0, secondary: 0 },
];

export default function DashboardPage() {
  const { data, isLoading, isError } = useDashboard();

  if (isLoading) {
    return <PageSkeleton />;
  }

  if (isError || !data) {
    return (
      <div className="rounded-xl border border-rose-200 bg-rose-50 p-4 text-sm text-rose-600">
        Não foi possível carregar o dashboard agora. Tente novamente em instantes.
      </div>
    );
  }

  const salesPoints =
    data.productsByCategory.length > 0
      ? data.productsByCategory.slice(0, 7).map((category, index) => ({
          label: category.name.slice(0, 3).toUpperCase(),
          primary: category.total * 120,
          secondary: Math.max(category.total * 80 - index * 10, 0),
        }))
      : fallbackSalesPoints;

  const topProductsItems = data.lowStockProducts.slice(0, 4).map((product) => ({
    id: product.id,
    name: product.name,
    category: product.categoryName ?? "Category",
    soldLabel: `${product.stock} pcs sold`,
    revenueLabel: currency(product.price * product.stock),
    initials: product.name
      .split(" ")
      .slice(0, 2)
      .map((part) => part[0])
      .join("")
      .toUpperCase(),
  }));

  const transactionsItems = data.lowStockProducts.slice(0, 5).map((product) => ({
    id: product.id.slice(0, 12),
    customer: product.name,
    product: product.name,
    category: product.categoryName ?? "Category",
    quantity: `${product.stock} pcs`,
    total: currency(product.price * product.stock),
    initials: product.name
      .split(" ")
      .slice(0, 2)
      .map((part) => part[0])
      .join("")
      .toUpperCase(),
  }));

  const countriesItems = data.productsByCategory.slice(0, 6).map((item, index) => ({
    code: item.name.slice(0, 2).toUpperCase() || `C${index + 1}`,
    value: Math.max(item.total * 1000, 1000),
    active: index === 0,
  }));

  return (
    <div className="space-y-6" data-testid="dashboard-page">
      <h1 className="text-3xl font-semibold tracking-tight text-slate-900">Dashboard</h1>

      <section className="grid grid-cols-1 gap-6 xl:grid-cols-3">
        <StatCard label="Total Sales" value={currency(data.totalStockValue)} growth="+8%" positive />
        <StatCard label="Customers" value={String(data.totalProducts)} growth="+20%" positive />
        <StatCard label="Page Views" value={String(data.lowStockProducts.length)} growth="-15%" positive={false} />
      </section>

      <section className="grid grid-cols-1 gap-6 xl:grid-cols-[2fr_1.2fr]">
        <SalesChart points={salesPoints} totalLabel={currency(data.totalStockValue)} growthLabel="+12%" />
        <TopProducts items={topProductsItems} />
      </section>

      <section className="grid grid-cols-1 gap-6 xl:grid-cols-[2fr_1fr]">
        <TransactionsTable items={transactionsItems} />
        <CountriesChart items={countriesItems} />
      </section>
    </div>
  );
}
