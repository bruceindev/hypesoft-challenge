import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { productSchema, type ProductSchema } from "@/lib/schemas";
import type { Category } from "@/types/category";
import type { Product } from "@/types/product";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";

interface ProductFormProps {
  categories: Category[];
  initialData?: Product;
  onSubmit: (values: ProductSchema) => void;
  isLoading?: boolean;
}

export const ProductForm = ({ categories, initialData, onSubmit, isLoading }: ProductFormProps) => {
  const form = useForm<ProductSchema>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      name: initialData?.name ?? "",
      description: initialData?.description ?? "",
      price: initialData?.price ?? 0,
      stock: initialData?.stock ?? 0,
      categoryId: initialData?.categoryId ?? "",
    },
    mode: "onChange",
  });

  const errors = form.formState.errors;

  return (
    <form className="space-y-4" onSubmit={form.handleSubmit(onSubmit)}>
      <div className="space-y-2">
        <Label>Nome</Label>
        <Input {...form.register("name")} />
        {errors.name && <p className="text-xs text-destructive">{errors.name.message}</p>}
      </div>

      <div className="space-y-2">
        <Label>Descrição</Label>
        <Textarea {...form.register("description")} />
        {errors.description && <p className="text-xs text-destructive">{errors.description.message}</p>}
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-2">
          <Label>Preço</Label>
          <Input type="number" step="0.01" {...form.register("price", { valueAsNumber: true })} />
          {errors.price && <p className="text-xs text-destructive">{errors.price.message}</p>}
        </div>

        <div className="space-y-2">
          <Label>Estoque</Label>
          <Input type="number" {...form.register("stock", { valueAsNumber: true })} />
          {errors.stock && <p className="text-xs text-destructive">{errors.stock.message}</p>}
        </div>
      </div>

      <div className="space-y-2">
        <Label>Categoria</Label>
        <Select {...form.register("categoryId")}>
          <option value="">Selecione</option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </Select>
        {errors.categoryId && <p className="text-xs text-destructive">{errors.categoryId.message}</p>}
      </div>

      <Button disabled={isLoading || !form.formState.isValid} type="submit">
        Salvar
      </Button>
    </form>
  );
};
