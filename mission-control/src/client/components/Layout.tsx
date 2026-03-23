import { NavLink } from 'react-router-dom';
import { ReactNode } from 'react';

export function Layout({ children }: { children: ReactNode }) {
  return (
    <div className="app-shell">
      <header className="topbar">
        <div className="brand-block">
          <div className="brand-mark">OC</div>
          <div>
            <p className="eyebrow">OpenClaw</p>
            <h1>Mission Control</h1>
          </div>
        </div>

        <nav className="topnav">
          <NavLink to="/" end>Token Usage</NavLink>
          <NavLink to="/agents">Agents</NavLink>
        </nav>

        <div className="topbar-status">
          <span className="live-pill"><span className="live-dot" /> Live telemetry</span>
          <p className="muted compact">Dark-mode operational dashboard</p>
        </div>
      </header>

      <main className="app-content">{children}</main>
    </div>
  );
}
