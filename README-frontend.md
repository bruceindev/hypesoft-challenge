# Frontend README (React 18 + Vite + TypeScript)

## Contexto do desafio
Este frontend foi construído com foco em qualidade técnica, integração real com backend e segurança básica de produção.

O prazo foi curto, então a prioridade foi:
- manter arquitetura organizada;
- garantir autenticação e autorização corretas;
- entregar fluxo de CRUD completo com feedback de erro;
- evitar complexidade desnecessária para o escopo do desafio.

## Decisões técnicas principais
- **`services/` + `hooks/`**: separa acesso à API da lógica de consumo em tela sem acoplamento forte.
- **`auth-context` com Keycloak**: centraliza estado de autenticação e papéis de usuário.
- **`api-client` com interceptor**: adiciona token, faz retry de `401` com refresh e redireciona quando sessão expira.
- **React Query**: cache e invalidação de dados de forma previsível, com política simples de retry.
- **Tipagem forte (`types/`)**: reduz erro de contrato entre frontend e backend.

## Simplificações aplicadas (para ficar coerente com junior avançado)
1. Fluxo de inicialização do `auth-context` foi deixado mais direto (menos camadas assíncronas).
2. `query-client` foi simplificado para manter robustez sem comportamento excessivamente sofisticado.
3. Hook não utilizado (`useLowStock`) foi removido para evitar abstração sem uso real.
4. Artefatos gerados localmente (`coverage/`, `test-results/`) foram isolados no `.gitignore`.

## O que foi mantido de propósito
- Segurança de autenticação/autorização com Keycloak.
- Interceptor com lock de refresh para evitar corrida de token.
- Estrutura de pastas por responsabilidade.
- Tratamento global de erro amigável para usuário.

## Limitações conhecidas (escopo/prazo)
- A suíte E2E existe, mas não foi expandida para todos os cenários de borda.
- O dashboard prioriza consumo real de API e legibilidade; não inclui camadas extras de analytics.

## Próximos passos naturais
- Aumentar cobertura de testes para fluxos de permissão por papel.
- Adicionar monitoramento de erro (ex.: Sentry) quando o projeto entrar em ambiente estável.
- Refinar componentes de UI reutilizáveis conforme surgirem novas telas.
