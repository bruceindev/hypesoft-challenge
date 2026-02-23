# Versionamento Base

Este projeto segue duas regras de versionamento:

1. **Conventional Commits** para mensagens de commit.
2. **SemVer** para tags de release (`MAJOR.MINOR.PATCH`).

## Padrão de Commits

Exemplos válidos:

- `feat(products): create product crud endpoints`
- `fix(stock): correct low-stock threshold`
- `docs(readme): improve setup instructions`
- `test(application): add create product handler tests`

## Estratégia de Tags

- `v0.1.0` → primeira base funcional (scaffold + estrutura inicial)
- `v0.2.0` → CRUD de categorias e produtos pronto
- `v0.3.0` → dashboard + estoque baixo + filtros
- `v0.4.0` → autenticação Keycloak integrada
- `v1.0.0` → entrega final estável do desafio

## Fluxo recomendado

1. Criar branch de feature: `feature/<escopo>`
2. Commits pequenos e semânticos (Conventional Commits)
3. Merge para `main`
4. Criar tag anotada para marcos importantes

## Comandos úteis

```bash
# criar tag anotada
git tag -a v0.1.0 -m "chore(init): base project scaffold"

# enviar commits + tags para remoto
git push origin main --follow-tags

# listar tags
git tag -n
```
