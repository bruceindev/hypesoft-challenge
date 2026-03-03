import { test } from "@playwright/test";
import { expect } from "@playwright/test";

const E2E_ENABLED = process.env.E2E_ENABLED === "true";

test("create product", async ({ page }) => {
  test.skip(!E2E_ENABLED, "Set E2E_ENABLED=true to run browser scenarios");
  test.skip(!process.env.E2E_ADMIN_EMAIL || !process.env.E2E_ADMIN_PASSWORD, "Admin credentials not configured");

  await page.goto("/login");
  await page.getByTestId("login-keycloak-btn").click();

  await page.getByLabel(/username|email/i).fill(process.env.E2E_ADMIN_EMAIL!);
  await page.getByLabel(/password/i).fill(process.env.E2E_ADMIN_PASSWORD!);
  await page.getByRole("button", { name: /sign in|entrar|login/i }).click();

  await page.goto("/products");
  await page.getByTestId("create-product-btn").click();

  await page.getByLabel("Nome").fill(`Produto E2E ${Date.now()}`);
  await page.getByLabel("Descrição").fill("Produto criado por teste E2E");
  await page.getByLabel("Preço").fill("50");
  await page.getByLabel("Estoque").fill("15");
  await page.getByLabel("Categoria").selectOption({ index: 1 });
  await page.getByRole("button", { name: "Salvar" }).click();

  await expect(page.getByText("Produto criado")).toBeVisible();
});

test("user blocked from delete", async ({ page }) => {
  test.skip(!E2E_ENABLED, "Set E2E_ENABLED=true to run browser scenarios");
  test.skip(!process.env.E2E_USER_EMAIL || !process.env.E2E_USER_PASSWORD, "User credentials not configured");

  await page.goto("/login");
  await page.getByTestId("login-keycloak-btn").click();

  await page.getByLabel(/username|email/i).fill(process.env.E2E_USER_EMAIL!);
  await page.getByLabel(/password/i).fill(process.env.E2E_USER_PASSWORD!);
  await page.getByRole("button", { name: /sign in|entrar|login/i }).click();

  await page.goto("/products");
  await expect(page.getByTestId("create-product-btn")).toHaveCount(0);
  await expect(page.locator('[data-testid^="delete-product-"]')).toHaveCount(0);
});
