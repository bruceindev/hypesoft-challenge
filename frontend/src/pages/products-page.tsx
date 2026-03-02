import { useMemo, useState } from "react";
import { Pencil, Plus, Search, Trash2 } from "lucide-react";
import { ProductForm } from "@/components/forms/product-form";
import { StockUpdateForm } from "@/components/forms/stock-update-form";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { useAuth } from "@/hooks/use-auth";
import { useCategories } from "@/hooks/use-categories";
import { useProductMutations, useProducts } from "@/hooks/use-products";
import { hasPermission } from "@/lib/roles";
import { notifyError, notifySuccess } from "@/lib/notify";
import type { Product } from "@/types/product";
import { currency } from "@/utils/currency";

export default function ProductsPage() {
  const auth = useAuth();
  const [searchTerm, setSearchTerm] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);

  const params = useMemo(() => ({ pageNumber, pageSize: 8, searchTerm, categoryId: categoryId || undefined }), [pageNumber, searchTerm, categoryId]);

  const { data, isLoading } = useProducts(params);
  const { data: categories = [] } = useCategories();
  const { createMutation, updateMutation, deleteMutation, updateStockMutation } = useProductMutations();

  const canCreate = hasPermission(auth.user?.roles ?? [], "products:create");
  const canUpdate = hasPermission(auth.user?.roles ?? [], "products:update");
  const canDelete = hasPermission(auth.user?.roles ?? [], "products:delete");
  const canUpdateStock = hasPermission(auth.user?.roles ?? [], "stock:update");

  return (
    <div className="space-y-4">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Produtos</CardTitle>
          {canCreate && (
            <Button data-testid="create-product-btn" onClick={() => { setEditingProduct(null); setIsModalOpen(true); }}>
              <Plus className="mr-2 h-4 w-4" /> Novo produto
            </Button>
          )}
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-[1fr_220px]">
            <div className="relative">
              <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
              <Input data-testid="products-search" className="pl-9" placeholder="Buscar por nome" value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} />
            </div>
            <Select data-testid="products-category-filter" value={categoryId} onChange={(e) => setCategoryId(e.target.value)}>
              <option value="">Todas categorias</option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>{category.name}</option>
              ))}
            </Select>
          </div>

          {isLoading ? (
            <div className="space-y-3">
              <Skeleton className="h-10 w-full rounded-lg" />
              <Skeleton className="h-10 w-full rounded-lg" />
              <Skeleton className="h-10 w-full rounded-lg" />
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Nome</TableHead>
                  <TableHead>Categoria</TableHead>
                  <TableHead>Preço</TableHead>
                  <TableHead>Estoque</TableHead>
                  <TableHead>Ações</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {data?.items.map((product) => (
                  <TableRow key={product.id}>
                    <TableCell>{product.name}</TableCell>
                    <TableCell>{categories.find((c) => c.id === product.categoryId)?.name ?? "-"}</TableCell>
                    <TableCell>{currency(product.price)}</TableCell>
                    <TableCell>
                      {product.stock < 10 ? <Badge variant="destructive">Baixo ({product.stock})</Badge> : <Badge variant="secondary">{product.stock}</Badge>}
                    </TableCell>
                    <TableCell className="space-y-2">
                      {canUpdateStock && (
                        <StockUpdateForm
                          stock={product.stock}
                          onSubmit={(stock) =>
                            updateStockMutation.mutate(
                              { id: product.id, stock },
                              { onSuccess: () => notifySuccess("Estoque atualizado"), onError: () => notifyError("Erro ao atualizar estoque") },
                            )
                          }
                        />
                      )}
                      <div className="flex gap-2">
                        {canUpdate && (
                          <Button data-testid={`edit-product-${product.id}`} variant="ghost" size="sm" onClick={() => { setEditingProduct(product); setIsModalOpen(true); }}>
                            <Pencil className="h-4 w-4" />
                          </Button>
                        )}
                        {canDelete && (
                          <Button
                            data-testid={`delete-product-${product.id}`}
                            variant="ghost"
                            size="sm"
                            onClick={() =>
                              deleteMutation.mutate(product.id, {
                                onSuccess: () => notifySuccess("Produto removido"),
                                onError: () => notifyError("Erro ao remover produto"),
                              })
                            }
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        )}
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}

          <div className="flex items-center justify-between">
            <Button variant="outline" disabled={!data?.hasPreviousPage} onClick={() => setPageNumber((prev) => Math.max(1, prev - 1))}>Anterior</Button>
            <span className="text-sm text-muted-foreground">Página {data?.pageNumber ?? 1} de {data?.totalPages ?? 1}</span>
            <Button variant="outline" disabled={!data?.hasNextPage} onClick={() => setPageNumber((prev) => prev + 1)}>Próxima</Button>
          </div>
        </CardContent>
      </Card>

      <Dialog open={isModalOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingProduct ? "Editar produto" : "Novo produto"}</DialogTitle>
          </DialogHeader>
          <ProductForm
            categories={categories}
            initialData={editingProduct ?? undefined}
            isLoading={createMutation.isPending || updateMutation.isPending}
            onSubmit={(values) => {
              const operation = editingProduct
                ? updateMutation.mutate({ id: editingProduct.id, input: values }, {
                    onSuccess: () => { notifySuccess("Produto atualizado"); setIsModalOpen(false); },
                    onError: () => notifyError("Erro ao atualizar produto"),
                  })
                : createMutation.mutate(values, {
                    onSuccess: () => { notifySuccess("Produto criado"); setIsModalOpen(false); },
                    onError: () => notifyError("Erro ao criar produto"),
                  });
              return operation;
            }}
          />
          <Button variant="outline" onClick={() => setIsModalOpen(false)}>Fechar</Button>
        </DialogContent>
      </Dialog>
    </div>
  );
}
