import { DashboardSource } from '../lib/types';

export function SourceList({ sources }: { sources: DashboardSource[] }) {
  return (
    <section className="panel">
      <div className="panel-header"><h3>Data Sources</h3></div>
      <div className="source-list">
        {sources.map((source) => (
          <article key={source.path} className="source-item">
            <div>
              <strong>{source.name}</strong>
              <p className="muted">{source.purpose}</p>
            </div>
            <div className="source-meta">
              <span className={`badge ${source.live ? 'badge-success' : 'badge-offline'}`}>{source.live ? 'live' : 'missing'}</span>
              <code>{source.path}</code>
            </div>
          </article>
        ))}
      </div>
    </section>
  );
}