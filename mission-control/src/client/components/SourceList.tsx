import { useState } from 'react';
import { DashboardSource } from '../lib/types';

export function SourceList({ sources }: { sources: DashboardSource[] }) {
  const [open, setOpen] = useState(false);

  return (
    <section className="panel sources-panel">
      <div className="section-head">
        <div>
          <p className="eyebrow">Traceability</p>
          <h3>Data sources</h3>
        </div>
        <button type="button" className="ghost-button" onClick={() => setOpen((value) => !value)}>
          {open ? 'Hide sources' : `Show sources (${sources.length})`}
        </button>
      </div>

      {open ? (
        <div className="source-grid">
          {sources.map((source) => (
            <article key={source.path} className="source-card">
              <div className="source-head">
                <strong>{source.name}</strong>
                <span className={`badge ${source.live ? 'badge-success' : 'badge-offline'}`}>{source.live ? 'live' : 'missing'}</span>
              </div>
              <p className="muted">{source.purpose}</p>
              <code>{source.path}</code>
            </article>
          ))}
        </div>
      ) : (
        <p className="muted">Source details are collapsed by default to keep the console focused. Expand when you need to trace where a metric came from.</p>
      )}
    </section>
  );
}
