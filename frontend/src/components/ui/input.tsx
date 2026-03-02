import * as React from "react";
import { cn } from "@/lib/utils";

const Input = React.forwardRef<HTMLInputElement, React.ComponentProps<"input">>(({ className, ...props }, ref) => {
  return <input ref={ref} className={cn("h-10 w-full rounded-md border bg-background px-3 py-2 text-sm", className)} {...props} />;
});
Input.displayName = "Input";

export { Input };
