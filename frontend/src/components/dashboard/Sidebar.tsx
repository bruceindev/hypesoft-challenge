import type { ComponentType } from "react";
import { NavLink } from "react-router-dom";
import {
  Box,
  ChartNoAxesCombined,
  HelpCircle,
  Home,
  MessageCircle,
  Settings,
  ShoppingBag,
  Users,
  X,
  FileText,
  Sparkles,
} from "lucide-react";
import { APP_ROUTES } from "@/lib/constants";

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

interface NavItem {
  label: string;
  icon: ComponentType<{ className?: string }>;
  to: string;
  active?: boolean;
  badge?: string;
}

interface NavSection {
  title: string;
  items: NavItem[];
}

const navSections: NavSection[] = [
  {
    title: "GENERAL",
    items: [
      { label: "Dashboard", icon: Home, to: APP_ROUTES.dashboard, active: true },
      { label: "Statistics", icon: ChartNoAxesCombined, to: "#" },
    ],
  },
  {
    title: "SHOP",
    items: [
      { label: "My Shop", icon: ShoppingBag, to: "#" },
      { label: "Products", icon: Box, to: APP_ROUTES.products },
      { label: "Customers", icon: Users, to: "#" },
      { label: "Invoice", icon: FileText, to: "#" },
      { label: "Message", icon: MessageCircle, to: "#", badge: "4" },
    ],
  },
  {
    title: "SUPPORT",
    items: [
      { label: "Settings", icon: Settings, to: "#" },
      { label: "Help", icon: HelpCircle, to: "#" },
    ],
  },
];

const sidebarBaseClass =
  "fixed inset-y-0 left-0 z-40 w-64 border-r border-slate-200 bg-white p-5 transition-transform duration-300 lg:translate-x-0";

export function Sidebar({ isOpen, onClose }: SidebarProps) {
  return (
    <>
      <div className={isOpen ? "fixed inset-0 z-30 bg-slate-900/40 lg:hidden" : "hidden"} onClick={onClose} />

      <aside className={`${sidebarBaseClass} ${isOpen ? "translate-x-0" : "-translate-x-full"}`}>
        <div className="mb-7 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="grid h-8 w-8 place-items-center rounded-md bg-indigo-500 text-white">
              <Sparkles className="h-4 w-4" />
            </div>
            <span className="text-xl font-semibold text-slate-900">ShopSense</span>
          </div>

          <button onClick={onClose} className="rounded-md p-1 text-slate-500 lg:hidden" aria-label="Close sidebar">
            <X className="h-4 w-4" />
          </button>
        </div>

        <nav className="space-y-7">
          {navSections.map((section) => (
            <div key={section.title}>
              <h4 className="mb-3 px-2 text-[11px] font-semibold tracking-[0.16em] text-slate-400">{section.title}</h4>

              <ul className="space-y-1">
                {section.items.map((item) => (
                  <li key={item.label}>
                    {item.to.startsWith("/") ? (
                      <NavLink
                        to={item.to}
                        className={({ isActive }) =>
                          `flex items-center justify-between rounded-xl px-3 py-2.5 text-sm font-medium transition ${
                            isActive || item.active
                              ? "bg-slate-100 text-slate-900"
                              : "text-slate-600 hover:bg-slate-50 hover:text-slate-900"
                          }`
                        }
                      >
                        <span className="inline-flex items-center gap-3">
                          <item.icon className="h-4 w-4" />
                          {item.label}
                        </span>
                        {item.badge ? <span className="rounded-full bg-indigo-100 px-1.5 text-[10px] text-indigo-600">{item.badge}</span> : null}
                      </NavLink>
                    ) : (
                      <button className="flex w-full items-center justify-between rounded-xl px-3 py-2.5 text-sm font-medium text-slate-600 transition hover:bg-slate-50 hover:text-slate-900">
                        <span className="inline-flex items-center gap-3">
                          <item.icon className="h-4 w-4" />
                          {item.label}
                        </span>
                        {item.badge ? <span className="rounded-full bg-indigo-100 px-1.5 text-[10px] text-indigo-600">{item.badge}</span> : null}
                      </button>
                    )}
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </nav>

        <div className="mt-8 rounded-xl bg-slate-50 p-4">
          <h5 className="text-sm font-semibold text-slate-900">Try ShopSense Pro</h5>
          <p className="mt-1 text-xs text-slate-500">Get Pro and enjoy 20+ features to enhance your sales.</p>
          <button className="mt-4 w-full rounded-xl bg-gradient-to-r from-indigo-500 to-violet-500 py-2.5 text-sm font-semibold text-white shadow-sm">
            Upgrade Plan
          </button>
        </div>
      </aside>
    </>
  );
}
