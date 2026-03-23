import { useMemo, useState } from 'react';
import { api } from '../lib/api';
import { AgentCard, AgentStatus } from '../lib/types';
import { usePollingQuery } from '../lib/usePollingQuery';
import { EmptyState, ErrorState, LoadingState } from '../components/States';
import { StatusBadge } from '../components/StatusBadge';

export function AgentsPage() {
  const { data, loading, error } = usePollingQuery(() => api.agents(), 15000, []);
  const [status, setStatus] = useState<'all' | AgentStatus>('all');
  const [agent, setAgent] = useState('all');
  const [activeOnly, setActiveOnly] = useState(false);
  const [search, setSearch] = useState('');
  const [sort, setSort] = useState<'activity' | 'queue' | 'errors'>('activity');
  const [selected, setSelected] = useState<AgentCard | null>(null);

  const filtered = useMemo(() => {
    if (!data) return [];
    return [...data.agents]
      .filter((item) => (status === 'all' || item.status === status) && (agent === 'all' || item.agentId === agent) && (!activeOnly || item.status === 'busy' || item.tasksInProgress > 0))
      .map((item) => ({
        ...item,
        outstandingTasks: item.outstandingTasks.filter((task) => task.title.toLowerCase().includes(search.toLowerCase())),
        currentTasks: item.currentTasks.filter((task) => task.title.toLowerCase().includes(search.toLowerCase())),
        completedTasks: item.completedTasks.filter((task) => task.title.toLowerCase().includes(search.toLowerCase()))
      }))
      .sort((a, b) => sort === 'queue' ? b.queueCount - a.queueCount : sort === 'errors' ? b.failuresLast24h.length - a.failuresLast24h.length : +new Date(b.lastActivityAt ?? 0) - +new Date(a.lastActivityAt ?? 0));
  }, [data, status, agent, activeOnly, search, sort]);

  if (loading && !data) return <LoadingState />;
  if (error && !data) return <ErrorState message={error} />;
  if (!data) return <EmptyState>No agent data found.</EmptyState>;

  return (
    <div className="page-grid">
      <section className="hero panel">
        <div>
          <p className="eyebrow">Agent mission board</p>
          <h2>Kanban view of OpenClaw agent activity</h2>
          <p className="muted">Combines configured agents, recent session runs, and subagent task metadata. Refreshes every {Math.round(data.refreshMs / 1000)}s.</p>
        </div>
      </section>

      <section className="filters panel">
        <select value={status} onChange={(e) => setStatus(e.target.value as any)}><option value="all">All status</option>{data.filterOptions.statuses.map((item) => <option key={item}>{item}</option>)}</select>
        <select value={agent} onChange={(e) => setAgent(e.target.value)}><option value="all">All agents</option>{data.filterOptions.agents.map((item) => <option key={item}>{item}</option>)}</select>
        <select value={sort} onChange={(e) => setSort(e.target.value as any)}><option value="activity">Sort: activity</option><option value="queue">Sort: queue size</option><option value="errors">Sort: errors</option></select>
        <label className="toggle"><input type="checkbox" checked={activeOnly} onChange={(e) => setActiveOnly(e.target.checked)} /> Show active only</label>
        <input placeholder="Search tasks" value={search} onChange={(e) => setSearch(e.target.value)} />
      </section>

      <section className="kanban-board">
        {filtered.map((agentCard) => (
          <article key={agentCard.agentId} className="kanban-column panel" onClick={() => setSelected(agentCard)}>
            <div className="panel-header column-head">
              <div>
                <h3>{agentCard.name}</h3>
                <p className="muted">{agentCard.primaryModel ?? 'Model unavailable'}</p>
              </div>
              <StatusBadge status={agentCard.status} />
            </div>
            <div className="metric-row">
              <span>Queue {agentCard.queueCount}</span><span>In progress {agentCard.tasksInProgress}</span><span>Done 24h {agentCard.completedLast24h}</span>
            </div>
            <p className="muted">Last activity: {agentCard.lastActivityAt ? new Date(agentCard.lastActivityAt).toLocaleString() : 'Unknown'}</p>
            <TaskSection title="Outstanding Tasks" tasks={agentCard.outstandingTasks} />
            <TaskSection title="Current Tasks" tasks={agentCard.currentTasks} />
            <TaskSection title="Completed Tasks (24h)" tasks={agentCard.completedTasks.slice(0, 6)} />
          </article>
        ))}
      </section>

      {selected ? (
        <aside className="drawer panel">
          <div className="panel-header"><h3>{selected.name} details</h3><button onClick={() => setSelected(null)}>Close</button></div>
          <p><StatusBadge status={selected.status} /></p>
          <p className="muted">Primary: {selected.primaryModel ?? 'Unknown'}</p>
          <p className="muted">Fallbacks: {selected.fallbackModels.join(', ') || 'None'}</p>
          <p className="muted">Token summary (24h): {selected.tokenUsageSummary.inputTokens.toLocaleString()} in / {selected.tokenUsageSummary.outputTokens.toLocaleString()} out / {selected.tokenUsageSummary.requests} requests</p>
          <TaskSection title="Recent runs" tasks={[...selected.currentTasks, ...selected.completedTasks].slice(0, 10)} />
          <TaskSection title="Failures last 24h" tasks={selected.failuresLast24h} />
          {selected.detailNotes.map((note) => <p key={note} className="muted">• {note}</p>)}
        </aside>
      ) : null}

      <section className="panel">
        {data.notes.map((note) => <p key={note} className="muted">• {note}</p>)}
      </section>
    </div>
  );
}

function TaskSection({ title, tasks }: { title: string; tasks: AgentCard['currentTasks'] }) {
  return (
    <section className="task-section">
      <h4>{title}</h4>
      {!tasks.length ? <p className="muted">No items</p> : tasks.map((task) => (
        <article key={task.id} className="task-card">
          <div className="task-head"><strong>{task.title}</strong><StatusBadge status={task.status} /></div>
          <p className="muted">{task.id.slice(0, 12)} · {task.provider ?? 'provider ?'} · {task.model ?? 'model ?'}</p>
          <p className="muted">{task.startedAt ? new Date(task.startedAt).toLocaleString() : 'No start'} {task.durationMs ? `· ${Math.round(task.durationMs / 1000)}s` : ''}</p>
          {(task.tokensIn || task.tokensOut) ? <p className="muted">{task.tokensIn ?? 0} in / {task.tokensOut ?? 0} out</p> : null}
          {task.failureReason ? <p className="muted error-text">{task.failureReason}</p> : null}
        </article>
      ))}
    </section>
  );
}