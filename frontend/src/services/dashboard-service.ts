import { apiClient } from "@/services/api-client";
import type { DashboardSummary } from "@/types/dashboard";

export const dashboardService = {
  async getSummary(): Promise<DashboardSummary> {
    const { data } = await apiClient.get<DashboardSummary>("/dashboard/stats");
    return data;
  },
};
