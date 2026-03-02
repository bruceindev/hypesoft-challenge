import { apiClient } from "@/services/api-client";
import type { PaginatedResult, PaginationParams } from "@/types/pagination";
import type { Product, ProductInput } from "@/types/product";

type ProductApiDto = {
  id: string;
  name: string;
  description: string;
  price: number;
  categoryId: string;
  stockQuantity: number;
  isLowStock?: boolean;
  createdAt?: string;
  updatedAt?: string;
};

const toProductModel = (dto: ProductApiDto): Product => {
  return {
    id: dto.id,
    name: dto.name,
    description: dto.description,
    price: dto.price,
    stock: dto.stockQuantity,
    categoryId: dto.categoryId,
    createdAt: dto.createdAt,
    updatedAt: dto.updatedAt,
  };
};

export const productService = {
  async list(params: PaginationParams): Promise<PaginatedResult<Product>> {
    const { data } = await apiClient.get<PaginatedResult<ProductApiDto>>("/products", {
      params: {
        pageNumber: params.pageNumber ?? 1,
        pageSize: params.pageSize ?? 10,
        searchTerm: params.searchTerm || undefined,
        categoryId: params.categoryId || undefined,
      },
    });

    return {
      ...data,
      items: data.items.map(toProductModel),
    };
  },

  async getLowStock(): Promise<Product[]> {
    const { data } = await apiClient.get<ProductApiDto[]>("/products/low-stock");
    return data.map(toProductModel);
  },

  async create(input: ProductInput): Promise<string> {
    const payload = {
      name: input.name,
      description: input.description,
      price: input.price,
      stockQuantity: input.stock,
      categoryId: input.categoryId,
    };

    const { data } = await apiClient.post<ProductApiDto>("/products", payload);
    return data.id;
  },

  async update(id: string, input: ProductInput): Promise<void> {
    await apiClient.put(`/products/${id}`, {
      id,
      name: input.name,
      description: input.description,
      price: input.price,
      stockQuantity: input.stock,
      categoryId: input.categoryId,
    });
  },

  async remove(id: string): Promise<void> {
    await apiClient.delete(`/products/${id}`);
  },

  async updateStock(id: string, stock: number): Promise<void> {
    await apiClient.patch(`/products/${id}/stock`, {
      stockQuantity: stock,
    });
  },
};
