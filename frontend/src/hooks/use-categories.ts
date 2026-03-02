import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { categoryService } from "@/services/category-service";
import type { CategorySchema } from "@/lib/schemas";

export const useCategories = () => {
  return useQuery({
    queryKey: ["categories"],
    queryFn: () => categoryService.list(),
  });
};

export const useCategoryMutations = () => {
  const queryClient = useQueryClient();

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ["categories"] });
    queryClient.invalidateQueries({ queryKey: ["products"] });
    queryClient.invalidateQueries({ queryKey: ["dashboard"] });
  };

  const createMutation = useMutation({
    mutationFn: (input: CategorySchema) => categoryService.create(input),
    onSuccess: invalidate,
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, input }: { id: string; input: CategorySchema }) => categoryService.update(id, input),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => categoryService.remove(id),
    onSuccess: invalidate,
  });

  return { createMutation, updateMutation, deleteMutation };
};
