import { z } from "zod";

export const productSchema = z.object({
  name: z.string().min(2, "Nome obrigatório"),
  description: z.string().min(5, "Descrição obrigatória"),
  price: z.number().min(0.01, "Preço deve ser maior que zero"),
  stock: z.number().min(0, "Estoque não pode ser negativo"),
  categoryId: z.string().min(1, "Categoria obrigatória"),
});

export const categorySchema = z.object({
  name: z.string().min(2, "Nome obrigatório").max(100, "Máximo 100 caracteres"),
  description: z.string().min(5, "Descrição obrigatória").max(500, "Máximo 500 caracteres"),
});

export const stockSchema = z.object({
  stock: z.number().min(0, "Estoque não pode ser negativo"),
});

export type ProductSchema = z.infer<typeof productSchema>;
export type CategorySchema = z.infer<typeof categorySchema>;
export type StockSchema = z.infer<typeof stockSchema>;
