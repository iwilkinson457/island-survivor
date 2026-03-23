import { NavLink } from 'react-router-dom';
import { ReactNode } from 'react';

export function Layout({ children }: { children: ReactNode }) {
  return (
    <div className="shell">
      <aside className="sidebar">
        <div>
          <p className="eyebrow">OpenClaw</p>
          <h1>Mission Control</h1>
          <p className="muted">Operational visibility for agents, tokens, and task flow.</p>
        </div>
        <nav className="nav">
          <NavLink to="/" end>Token Usage</NavLink>
          <NavLink to="/agents">Agents</NavLink>
        </nav>
      </aside>
      <main className="content">{children}</main>
    </div>
  );
}