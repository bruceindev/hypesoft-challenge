import { Component, type ReactNode } from "react";

interface GlobalErrorBoundaryState {
  hasError: boolean;
}

interface GlobalErrorBoundaryProps {
  children: ReactNode;
}

export class GlobalErrorBoundary extends Component<GlobalErrorBoundaryProps, GlobalErrorBoundaryState> {
  constructor(props: GlobalErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(): GlobalErrorBoundaryState {
    return { hasError: true };
  }

  componentDidCatch() {
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
          <div className="w-full max-w-md rounded-xl border border-slate-200 bg-white p-6 text-center shadow-sm">
            <h1 className="text-xl font-semibold text-slate-900">Algo deu errado</h1>
            <p className="mt-2 text-sm text-slate-500">Ocorreu um erro inesperado na aplicação. Tente recarregar a página.</p>
            <button
              onClick={() => window.location.reload()}
              className="mt-4 rounded-lg bg-indigo-500 px-4 py-2 text-sm font-semibold text-white hover:bg-indigo-600"
            >
              Recarregar
            </button>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}
