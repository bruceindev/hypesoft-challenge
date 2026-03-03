import { apiClient } from "@/services/api-client";
import type { Category } from "@/types/category";

type CategoryApiDto = {
  id: string;
  name: string;
  description: string;
  createdAt?: string;
};

const toCategoryModel = (dto: CategoryApiDto): Category => {
  return {
    id: dto.id,
    name: dto.name,
    description: dto.description,
    createdAt: dto.createdAt,
  };
};

export const categoryService = {
  async list(): Promise<Category[]> {
    const { data } = await apiClient.get<CategoryApiDto[]>("/categories");
    return data.map(toCategoryModel);
  },

  async create(input: { name: string; description?: string }): Promise<string> {
    const { data } = await apiClient.post<CategoryApiDto>("/categories", {
      name: input.name,
      description: input.description ?? "",
    });
    return data.id;
  },

  async update(id: string, input: { name: string; description?: string }): Promise<void> {
    await apiClient.put(`/categories/${id}`, {
      id,
      name: input.name,
      description: input.description ?? "",
    });
  },

  async remove(id: string): Promise<void> {
    await apiClient.delete(`/categories/${id}`);
  },
};
