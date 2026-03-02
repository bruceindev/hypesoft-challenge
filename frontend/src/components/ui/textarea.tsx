import * as React from "react";
import { cn } from "@/lib/utils";

const Textarea = React.forwardRef<HTMLTextAreaElement, React.ComponentProps<"textarea">>(({ className, ...props }, ref) => {
  return <textarea ref={ref} className={cn("min-h-[100px] w-full rounded-md border bg-background px-3 py-2 text-sm", className)} {...props} />;
});
Textarea.displayName = "Textarea";

export { Textarea };
