import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import ProductsPage from "@/pages/products-page";
import type { Product } from "@/types/product";

const useProductsMock = vi.fn();

vi.mock("@/hooks/use-auth", () => ({
  useAuth: () => ({ user: { roles: ["Admin"] } }),
}));

vi.mock("@/hooks/use-categories", () => ({
  useCategories: () => ({
    data: [{ id: "cat-1", name: "Eletrônicos", description: "Produtos eletrônicos" }],
  }),
}));

vi.mock("@/hooks/use-products", () => ({
  useProducts: (params: unknown) => useProductsMock(params),
  useProductMutations: () => ({
    createMutation: { mutate: vi.fn(), isPending: false },
    updateMutation: { mutate: vi.fn(), isPending: false },
    deleteMutation: { mutate: vi.fn() },
    updateStockMutation: { mutate: vi.fn() },
  }),
}));

const products: Product[] = [
  {
    id: "prod-1",
    name: "Notebook Pro",
    description: "Notebook de alta performance",
    price: 5000,
    stock: 12,
    categoryId: "cat-1",
  },
  {
    id: "prod-2",
    name: "Mouse Gamer",
    description: "Mouse com sensor óptico",
    price: 250,
    stock: 20,
    categoryId: "cat-1",
  },
];

describe("ProductsPage", () => {
  beforeEach(() => {
    useProductsMock.mockImplementation((params: { searchTerm?: string }) => {
      const term = (params.searchTerm ?? "").toLowerCase();
      const filtered = products.filter((product) => product.name.toLowerCase().includes(term));

      return {
        data: {
          items: filtered,
          pageNumber: 1,
          pageSize: 8,
          totalPages: 1,
          totalCount: filtered.length,
          hasPreviousPage: false,
          hasNextPage: false,
        },
        isLoading: false,
      };
    });
  });

  it("renders products table with rows", () => {
    render(<ProductsPage />);

    expect(screen.getByText("Notebook Pro")).toBeInTheDocument();
    expect(screen.getByText("Mouse Gamer")).toBeInTheDocument();
  });

  it("filters products by name", () => {
    render(<ProductsPage />);

    fireEvent.change(screen.getByTestId("products-search"), {
      target: { value: "mouse" },
    });

    expect(screen.queryByText("Notebook Pro")).not.toBeInTheDocument();
    expect(screen.getByText("Mouse Gamer")).toBeInTheDocument();
  });
});
