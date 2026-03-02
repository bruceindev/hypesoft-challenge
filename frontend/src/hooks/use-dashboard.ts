import { useQuery } from "@tanstack/react-query";
import { dashboardService } from "@/services/dashboard-service";

export const useDashboard = () => {
  return useQuery({
    queryKey: ["dashboard"],
    queryFn: () => dashboardService.getSummary(),
    staleTime: 60_000,
    gcTime: 300_000,
  });
};
