import { useEffect, useMemo, useState } from 'react';
import { api } from '../lib/api';
import { AgentCard, AgentStatus, DashboardSource } from '../lib/types';
import { usePollingQuery } from '../lib/usePollingQuery';
import { EmptyState, ErrorState, LoadingState } from '../components/States';
import { StatusBadge } from '../components/StatusBadge';
import { SourceList } from '../components/SourceList';

export function AgentsPage({ sources }: { sources: DashboardSource[] }) {
  const { data, loading, error } = usePollingQuery(() => api.agents(), 15000, []);
  const [status, setStatus] = useState<'all' | AgentStatus>('all');
  const [agent, setAgent] = useState('all');
  const [activeOnly, setActiveOnly] = useState(false);
  const [search, setSearch] = useState('');
  const [sort, setSort] = useState<'activity' | 'queue' | 'errors'>('activity');
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const filtered = useMemo(() => {
    if (!data) return [];
    return [...data.agents]
      .filter((item) => (status === 'all' || item.status === status) && (agent === 'all' || item.agentId === agent) && (!activeOnly || item.status === 'busy' || item.tasksInProgress > 0))
      .filter((item) => {
        const haystack = [item.name, item.agentId, item.primaryModel, ...item.fallbackModels].filter(Boolean).join(' ').toLowerCase();
        return haystack.includes(search.toLowerCase());
      })
      .sort((a, b) => sort === 'queue'
        ? b.queueCount - a.queueCount
        : sort === 'errors'
          ? b.failuresLast24h.length - a.failuresLast24h.length
          : +new Date(b.lastActivityAt ?? 0) - +new Date(a.lastActivityAt ?? 0));
  }, [data, status, agent, activeOnly, search, sort]);

  useEffect(() => {
    if (!filtered.length) {
      setSelectedId(null);
      return;
    }

    if (!selectedId || !filtered.some((item) => item.agentId === selectedId)) {
      setSelectedId(filtered[0].agentId);
    }
  }, [filtered, selectedId]);

  const selected = filtered.find((item) => item.agentId === selectedId) ?? null;

  if (loading && !data) return <LoadingState />;
  if (error && !data) return <ErrorState message={error} />;
  if (!data) return <EmptyState>No agent telemetry found.</EmptyState>;

  return (
    <div className="page-grid">
      <section className="page-header panel">
        <div>
          <p className="eyebrow">Agent operations</p>
          <h2>Roster, status, and recent work</h2>
          <p className="muted">Reframed as a control-room roster instead of a kanban board. Refreshes every {Math.round(data.refreshMs / 1000)}s.</p>
        </div>
        <div className="refresh-note">
          <span className="live-pill"><span className="live-dot" /> Live</span>
          <span className="muted compact">{filtered.length} agent{filtered.length === 1 ? '' : 's'} in view</span>
        </div>
      </section>

      <section className="filter-toolbar panel">
        <div className="filter-group">
          <label>Status</label>
          <select value={status} onChange={(e) => setStatus(e.target.value as 'all' | AgentStatus)}>
            <option value="all">All status</option>
            {data.filterOptions.statuses.map((item) => <option key={item}>{item}</option>)}
          </select>
        </div>
        <div className="filter-group">
          <label>Agent</label>
          <select value={agent} onChange={(e) => setAgent(e.target.value)}>
            <option value="all">All agents</option>
            {data.filterOptions.agents.map((item) => <option key={item}>{item}</option>)}
          </select>
        </div>
        <div className="filter-group">
          <label>Sort</label>
          <select value={sort} onChange={(e) => setSort(e.target.value as 'activity' | 'queue' | 'errors')}>
            <option value="activity">Recent activity</option>
            <option value="queue">Queue size</option>
            <option value="errors">Errors</option>
          </select>
        </div>
        <div className="filter-group filter-search">
          <label>Search</label>
          <input placeholder="Agent name, id, or model" value={search} onChange={(e) => setSearch(e.target.value)} />
        </div>
        <label className="toggle-pill">
          <input type="checkbox" checked={activeOnly} onChange={(e) => setActiveOnly(e.target.checked)} />
          <span>Active only</span>
        </label>
      </section>

      {!filtered.length ? (
        <EmptyState>No agents match the current filters.</EmptyState>
      ) : (
        <section className="dashboard-grid dashboard-grid-agents">
          <article className="panel roster-panel">
            <div className="section-head">
              <div>
                <p className="eyebrow">Roster</p>
                <h3>Configured agents</h3>
              </div>
            </div>
            <div className="roster-list">
              {filtered.map((agentCard) => (
                <button
                  key={agentCard.agentId}
                  type="button"
                  className={`agent-row ${selectedId === agentCard.agentId ? 'selected' : ''}`}
                  onClick={() => setSelectedId(agentCard.agentId)}
                >
                  <div className="agent-row-main">
                    <div className="agent-name-block">
                      <div className="agent-row-title">
                        <StatusBadge status={agentCard.status} dotOnly />
                        <strong>{agentCard.name}</strong>
                      </div>
                      <span className="muted compact">{agentCard.primaryModel ?? 'Model unavailable'}</span>
                    </div>
                    <StatusBadge status={agentCard.status} />
                  </div>
                  <div className="agent-row-stats">
                    <span><label>Queue</label><strong>{agentCard.queueCount}</strong></span>
                    <span><label>Running</label><strong>{agentCard.tasksInProgress}</strong></span>
                    <span><label>Done 24h</label><strong>{agentCard.completedLast24h}</strong></span>
                    <span><label>Requests</label><strong>{agentCard.tokenUsageSummary.requests}</strong></span>
                  </div>
                  <div className="agent-row-meta">
                    <span className="muted">{agentCard.agentId}</span>
                    <span className="muted">Last activity {agentCard.lastActivityAt ? formatRelative(agentCard.lastActivityAt) : 'unknown'}</span>
                  </div>
                </button>
              ))}
            </div>
          </article>

          {selected ? (
            <article className="panel detail-panel">
              <div className="section-head">
                <div>
                  <p className="eyebrow">Selected agent</p>
                  <h3>{selected.name}</h3>
                </div>
                <StatusBadge status={selected.status} />
              </div>

              <div className="detail-metrics">
                <div className="stat-cell"><span className="stat-label">Primary model</span><strong className="stat-value small">{selected.primaryModel ?? 'Unknown'}</strong></div>
                <div className="stat-cell"><span className="stat-label">Fallbacks</span><strong className="stat-value small">{selected.fallbackModels.join(', ') || 'None'}</strong></div>
                <div className="stat-cell"><span className="stat-label">Input / output</span><strong className="stat-value small">{selected.tokenUsageSummary.inputTokens.toLocaleString()} / {selected.tokenUsageSummary.outputTokens.toLocaleString()}</strong></div>
                <div className="stat-cell"><span className="stat-label">Requests (24h)</span><strong className="stat-value small">{selected.tokenUsageSummary.requests.toLocaleString()}</strong></div>
              </div>

              <div className="detail-sections">
                <TaskBlock title="Current tasks" tasks={selected.currentTasks} empty="No current tasks." />
                <TaskBlock title="Completed tasks" tasks={selected.completedTasks.slice(0, 10)} empty="No completed tasks found." />
                <TaskBlock title="Failures last 24h" tasks={selected.failuresLast24h} empty="No recent failures." />
              </div>

              <section className="panel-subsection">
                <h4>Agent notes</h4>
                <ul className="notes-list compact-notes">
                  {selected.detailNotes.map((note) => <li key={note}>{note}</li>)}
                </ul>
              </section>
            </article>
          ) : null}
        </section>
      )}

      <section className="panel notes-panel">
        <div className="section-head">
          <div>
            <p className="eyebrow">Interpretation</p>
            <h3>Agent notes</h3>
          </div>
        </div>
        <ul className="notes-list">
          {data.notes.map((note) => <li key={note}>{note}</li>)}
        </ul>
      </section>

      <SourceList sources={sources} />
    </div>
  );
}

function TaskBlock({ title, tasks, empty }: { title: string; tasks: AgentCard['currentTasks']; empty: string }) {
  return (
    <section className="panel-subsection">
      <h4>{title}</h4>
      {!tasks.length ? <p className="muted">{empty}</p> : (
        <div className="task-list">
          {tasks.map((task) => (
            <article key={task.id} className="task-row">
              <div className="task-row-head">
                <strong>{task.title}</strong>
                <StatusBadge status={task.status} />
              </div>
              <div className="task-row-meta">
                <span>{task.provider ?? 'provider ?'}</span>
                <span>{task.model ?? 'model ?'}</span>
                <span>{task.startedAt ? new Date(task.startedAt).toLocaleString() : 'No start'}</span>
                <span>{task.durationMs ? `${Math.round(task.durationMs / 1000)}s` : '—'}</span>
              </div>
              {(task.tokensIn || task.tokensOut) ? <p className="muted compact">{task.tokensIn ?? 0} in / {task.tokensOut ?? 0} out</p> : null}
              {task.failureReason ? <p className="error-text compact">{task.failureReason}</p> : null}
            </article>
          ))}
        </div>
      )}
    </section>
  );
}

function formatRelative(value: string) {
  const deltaMs = Date.now() - new Date(value).getTime();
  const minutes = Math.max(0, Math.round(deltaMs / 60000));
  if (minutes < 1) return 'just now';
  if (minutes < 60) return `${minutes}m ago`;
  const hours = Math.round(minutes / 60);
  if (hours < 24) return `${hours}h ago`;
  const days = Math.round(hours / 24);
  return `${days}d ago`;
}
