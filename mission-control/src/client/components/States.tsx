import { ReactNode } from 'react';

export const LoadingState = () => <div className="panel state">Loading dashboard data…</div>;
export const ErrorState = ({ message }: { message: string }) => <div className="panel state error">{message}</div>;
export const EmptyState = ({ children }: { children: ReactNode }) => <div className="panel state">{children}</div>;