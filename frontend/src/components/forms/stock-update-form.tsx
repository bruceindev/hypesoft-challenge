import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { stockSchema, type StockSchema } from "@/lib/schemas";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export const StockUpdateForm = ({ stock, onSubmit }: { stock: number; onSubmit: (value: number) => void }) => {
  const form = useForm<StockSchema>({
    resolver: zodResolver(stockSchema),
    defaultValues: { stock },
    mode: "onChange",
  });

  return (
    <form className="flex items-center gap-2" onSubmit={form.handleSubmit((values) => onSubmit(values.stock))}>
      <Input className="w-24" type="number" {...form.register("stock", { valueAsNumber: true })} />
      <Button size="sm" type="submit" disabled={!form.formState.isValid}>Atualizar</Button>
    </form>
  );
};
