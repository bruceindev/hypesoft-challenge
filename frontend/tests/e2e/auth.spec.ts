import { test, expect } from "@playwright/test";

const E2E_ENABLED = process.env.E2E_ENABLED === "true";

test("login screen renders", async ({ page }) => {
  test.skip(!E2E_ENABLED, "Set E2E_ENABLED=true to run browser scenarios");
  await page.goto("/login");
  await expect(page.getByTestId("login-keycloak-btn")).toBeVisible({ timeout: 15000 });
});

test("dashboard loads correctly after login", async ({ page }) => {
  test.skip(!E2E_ENABLED, "Set E2E_ENABLED=true to run browser scenarios");
  test.skip(!process.env.E2E_USER_EMAIL || !process.env.E2E_USER_PASSWORD, "E2E credentials not configured");

  await page.goto("/login");
  await page.getByTestId("login-keycloak-btn").click();

  await page.getByLabel(/username|email/i).fill(process.env.E2E_USER_EMAIL!);
  await page.getByLabel(/password/i).fill(process.env.E2E_USER_PASSWORD!);
  await page.getByRole("button", { name: /sign in|entrar|login/i }).click();

  await expect(page.getByTestId("dashboard-page")).toBeVisible();
});
