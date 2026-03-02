export interface DashboardLowStockProduct {
  id: string;
  name: string;
  categoryId: string;
  categoryName?: string;
  price: number;
  stock: number;
}

export interface DashboardCategoryStat {
  categoryId: string;
  name: string;
  total: number;
}

export interface DashboardSummary {
  totalProducts: number;
  totalStockValue: number;
  lowStockCount: number;
  lowStockProducts: DashboardLowStockProduct[];
  productsByCategory: DashboardCategoryStat[];
}
