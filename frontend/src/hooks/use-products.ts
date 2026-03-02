import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { productService } from "@/services/product-service";
import type { PaginationParams } from "@/types/pagination";
import type { ProductInput } from "@/types/product";

export const useProducts = (params: PaginationParams) => {
  return useQuery({
    queryKey: ["products", params],
    queryFn: () => productService.list(params),
  });
};

export const useProductMutations = () => {
  const queryClient = useQueryClient();

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ["products"] });
    queryClient.invalidateQueries({ queryKey: ["dashboard"] });
  };

  const createMutation = useMutation({
    mutationFn: (input: ProductInput) => productService.create(input),
    onSuccess: invalidate,
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, input }: { id: string; input: ProductInput }) => productService.update(id, input),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => productService.remove(id),
    onSuccess: invalidate,
  });

  const updateStockMutation = useMutation({
    mutationFn: ({ id, stock }: { id: string; stock: number }) => productService.updateStock(id, stock),
    onSuccess: invalidate,
  });

  return { createMutation, updateMutation, deleteMutation, updateStockMutation };
};
