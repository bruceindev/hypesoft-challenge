import { Bell, Menu, Moon, Search } from "lucide-react";

interface TopbarProps {
  onOpenSidebar: () => void;
  userName?: string;
  userRole?: string;
}

export function Topbar({ onOpenSidebar, userName = "Miguel Santos", userRole = "Shop Admin" }: TopbarProps) {
  return (
    <header className="flex items-center justify-between gap-4 border-b border-slate-200 bg-white/70 px-4 py-4 backdrop-blur-sm sm:px-6">
      <div className="flex w-full max-w-2xl items-center gap-3 lg:mx-auto">
        <button onClick={onOpenSidebar} className="rounded-lg border border-slate-200 bg-white p-2 text-slate-600 lg:hidden" aria-label="Open sidebar">
          <Menu className="h-4 w-4" />
        </button>

        <div className="flex w-full items-center gap-2 rounded-xl border border-slate-200 bg-white px-4 py-2.5 shadow-sm">
          <Search className="h-4 w-4 text-slate-400" />
          <input
            type="text"
            placeholder="Search"
            className="w-full border-none bg-transparent text-sm text-slate-700 outline-none placeholder:text-slate-400"
          />
          <span className="hidden rounded-md bg-slate-100 px-2 py-1 text-[10px] font-semibold text-slate-500 sm:inline">⌘ S</span>
        </div>
      </div>

      <div className="hidden items-center gap-3 md:flex">
        <button className="rounded-lg p-2 text-slate-500 hover:bg-slate-100" aria-label="Theme">
          <Moon className="h-4 w-4" />
        </button>

        <button className="rounded-lg p-2 text-slate-500 hover:bg-slate-100" aria-label="Notifications">
          <Bell className="h-4 w-4" />
        </button>

        <div className="flex items-center gap-2 rounded-xl border border-slate-200 bg-white px-2 py-1.5">
          <div className="grid h-8 w-8 place-items-center rounded-full bg-indigo-100 text-xs font-semibold text-indigo-600">
            {userName
              .split(" ")
              .slice(0, 2)
              .map((part) => part[0])
              .join("")}
          </div>
          <div className="pr-1">
            <p className="text-sm font-semibold text-slate-800">{userName}</p>
            <p className="text-[11px] text-slate-400">{userRole}</p>
          </div>
        </div>
      </div>
    </header>
  );
}
