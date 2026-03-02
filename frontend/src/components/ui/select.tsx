import * as React from "react";
import { cn } from "@/lib/utils";

export const Select = React.forwardRef<HTMLSelectElement, React.SelectHTMLAttributes<HTMLSelectElement>>(
  ({ className, children, ...props }, ref) => (
    <select ref={ref} className={cn("h-10 w-full rounded-md border bg-background px-3 py-2 text-sm", className)} {...props}>
      {children}
    </select>
  ),
);
Select.displayName = "Select";
