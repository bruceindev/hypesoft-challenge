import { useState } from "react";
import { Pencil, Trash2 } from "lucide-react";
import { CategoryForm } from "@/components/forms/category-form";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Skeleton } from "@/components/ui/skeleton";
import { useCategoryMutations, useCategories } from "@/hooks/use-categories";
import { notifyError, notifySuccess } from "@/lib/notify";
import { hasPermission } from "@/lib/roles";
import { useAuth } from "@/hooks/use-auth";
import type { Category } from "@/types/category";

export default function CategoriesPage() {
  const auth = useAuth();
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const { data = [], isLoading } = useCategories();
  const { createMutation, updateMutation, deleteMutation } = useCategoryMutations();

  const canManage = hasPermission(auth.user?.roles ?? [], "categories:manage");

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Categorias</CardTitle>
          {canManage && (
            <Button onClick={() => setIsCreateOpen(true)}>
              Nova Categoria
            </Button>
          )}
        </CardHeader>
        <CardContent className="space-y-4">
        {isLoading ? (
          <div className="space-y-3">
            <Skeleton className="h-10 w-full rounded-lg" />
            <Skeleton className="h-10 w-full rounded-lg" />
            <Skeleton className="h-10 w-full rounded-lg" />
          </div>
        ) : (
          <ul className="space-y-2">
            {data.map((category) => (
              <li key={category.id} className="flex items-center justify-between rounded-md border p-3">
                <span>{category.name}</span>
                {canManage && (
                  <div className="flex gap-2">
                    <Button variant="ghost" size="sm" onClick={() => setEditingCategory(category)}>
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() =>
                        deleteMutation.mutate(category.id, {
                          onSuccess: () => notifySuccess("Categoria removida"),
                          onError: () => notifyError("Erro ao remover categoria"),
                        })
                      }
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                )}
              </li>
            ))}
          </ul>
        )}
        </CardContent>
      </Card>

      <Dialog open={isCreateOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Nova Categoria</DialogTitle>
          </DialogHeader>
          <CategoryForm
            isLoading={createMutation.isPending}
            submitLabel="Adicionar"
            onSubmit={(values) => {
              createMutation.mutate(values, {
                onSuccess: () => {
                  notifySuccess("Categoria criada");
                  setIsCreateOpen(false);
                },
                onError: () => notifyError("Erro ao criar categoria"),
              });
            }}
          />
          <Button variant="outline" onClick={() => setIsCreateOpen(false)}>
            Fechar
          </Button>
        </DialogContent>
      </Dialog>

      <Dialog open={editingCategory !== null}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Editar categoria</DialogTitle>
          </DialogHeader>
          <CategoryForm
            initialValues={{
              name: editingCategory?.name,
              description: editingCategory?.description,
            }}
            submitLabel="Salvar"
            isLoading={updateMutation.isPending}
            onSubmit={(values) => {
              if (!editingCategory) {
                return;
              }

              updateMutation.mutate(
                { id: editingCategory.id, input: values },
                {
                  onSuccess: () => {
                    notifySuccess("Categoria atualizada");
                    setEditingCategory(null);
                  },
                  onError: () => notifyError("Erro ao atualizar categoria"),
                },
              );
            }}
          />
          <Button variant="outline" onClick={() => setEditingCategory(null)}>
            Fechar
          </Button>
        </DialogContent>
      </Dialog>
    </>
  );
}
