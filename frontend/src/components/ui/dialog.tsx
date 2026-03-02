import * as React from "react";

export const Dialog = ({ open, children }: { open: boolean; children: React.ReactNode }) => (open ? <>{children}</> : null);

export const DialogContent = ({ children }: { children: React.ReactNode }) => (
  <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
    <div className="w-full max-w-lg rounded-xl border bg-background p-4 shadow-lg">{children}</div>
  </div>
);

export const DialogHeader = ({ children }: { children: React.ReactNode }) => <div className="mb-3">{children}</div>;
export const DialogTitle = ({ children }: { children: React.ReactNode }) => <h2 className="text-lg font-semibold">{children}</h2>;
