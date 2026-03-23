import { ReactNode } from 'react';

export const LoadingState = () => (
  <div className="panel state-panel">
    <div className="state-stack">
      <span className="live-dot" />
      <strong>Loading dashboard data</strong>
      <p className="muted">Collecting live operational telemetry from OpenClaw sources…</p>
    </div>
  </div>
);

export const ErrorState = ({ message }: { message: string }) => (
  <div className="panel state-panel state-error">
    <div className="state-stack">
      <strong>Dashboard load failed</strong>
      <p className="error-text">{message}</p>
    </div>
  </div>
);

export const EmptyState = ({ children }: { children: ReactNode }) => (
  <div className="panel state-panel">
    <div className="state-stack">{children}</div>
  </div>
);
