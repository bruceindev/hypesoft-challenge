import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { categorySchema, type CategorySchema } from "@/lib/schemas";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

interface CategoryFormProps {
  onSubmit: (values: CategorySchema) => void;
  isLoading?: boolean;
  initialValues?: Partial<CategorySchema>;
  submitLabel?: string;
}

export const CategoryForm = ({
  onSubmit,
  isLoading,
  initialValues,
  submitLabel = "Adicionar",
}: CategoryFormProps) => {
  const form = useForm<CategorySchema>({
    resolver: zodResolver(categorySchema),
    defaultValues: {
      name: initialValues?.name ?? "",
      description: initialValues?.description ?? "",
    },
    mode: "onChange",
  });

  return (
    <form className="space-y-3" onSubmit={form.handleSubmit(onSubmit)}>
      <div className="space-y-2">
        <Label>Nome da categoria</Label>
        <Input {...form.register("name")} />
        {form.formState.errors.name && <p className="text-xs text-destructive">{form.formState.errors.name.message}</p>}
      </div>

      <div className="space-y-2">
        <Label>Descrição</Label>
        <Textarea {...form.register("description")} />
        {form.formState.errors.description && <p className="text-xs text-destructive">{form.formState.errors.description.message}</p>}
      </div>

      <Button disabled={isLoading || !form.formState.isValid} type="submit">
        {submitLabel}
      </Button>
    </form>
  );
};
