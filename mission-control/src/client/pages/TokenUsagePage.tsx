import { useMemo, useState } from 'react';
import { Bar, BarChart, CartesianGrid, Legend, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { api } from '../lib/api';
import { TimeRange } from '../lib/types';
import { usePollingQuery } from '../lib/usePollingQuery';
import { EmptyState, ErrorState, LoadingState } from '../components/States';
import { SummaryCard } from '../components/SummaryCard';
import { StatusBadge } from '../components/StatusBadge';

export function TokenUsagePage() {
  const [range, setRange] = useState<TimeRange>('24h');
  const [agent, setAgent] = useState('all');
  const [provider, setProvider] = useState('all');
  const [model, setModel] = useState('all');
  const [status, setStatus] = useState('all');
  const { data, loading, error } = usePollingQuery(() => api.tokenUsage(range), 30000, [range]);

  const filteredRecords = useMemo(() => {
    if (!data) return [];
    return data.records.filter((record) =>
      (agent === 'all' || record.agentId === agent) &&
      (provider === 'all' || record.provider === provider) &&
      (model === 'all' || record.model === model) &&
      (status === 'all' || record.status === status)
    );
  }, [data, agent, provider, model, status]);

  const chartData = useMemo(() => {
    if (!data) return [];
    const allowedModels = new Set(filteredRecords.map((item) => item.model));
    return data.buckets.map((bucket) => {
      const row: Record<string, string | number> = { label: bucket.label };
      for (const [modelName, total] of Object.entries(bucket.totalsByModel)) {
        if (!allowedModels.size || allowedModels.has(modelName)) row[modelName] = total;
      }
      return row;
    });
  }, [data, filteredRecords]);

  const perfData = useMemo(() => filteredRecords.filter((item) => item.tokensPerSecond).map((item) => ({ time: new Date(item.startedAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }), model: item.model, tokensPerSecond: item.tokensPerSecond! })), [filteredRecords]);
  const models = data?.filterOptions.models ?? [];

  if (loading && !data) return <LoadingState />;
  if (error && !data) return <ErrorState message={error} />;
  if (!data) return <EmptyState>No token usage data available.</EmptyState>;

  return (
    <div className="page-grid">
      <section className="hero panel">
        <div>
          <p className="eyebrow">Token telemetry</p>
          <h2>Provider and model usage across OpenClaw</h2>
          <p className="muted">Live from OpenClaw session metadata and JSONL transcripts. Refreshes every {Math.round(data.refreshMs / 1000)}s.</p>
        </div>
        <div className="toolbar">
          {(['24h', '7d', '30d'] as TimeRange[]).map((option) => (
            <button key={option} className={option === range ? 'active' : ''} onClick={() => setRange(option)}>{option}</button>
          ))}
        </div>
      </section>

      <section className="filters panel">
        <select value={agent} onChange={(e) => setAgent(e.target.value)}><option value="all">All agents</option>{data.filterOptions.agents.map((item) => <option key={item}>{item}</option>)}</select>
        <select value={provider} onChange={(e) => setProvider(e.target.value)}><option value="all">All providers</option>{data.filterOptions.providers.map((item) => <option key={item}>{item}</option>)}</select>
        <select value={model} onChange={(e) => setModel(e.target.value)}><option value="all">All models</option>{models.map((item) => <option key={item}>{item}</option>)}</select>
        <select value={status} onChange={(e) => setStatus(e.target.value)}><option value="all">All outcomes</option>{data.filterOptions.statuses.map((item) => <option key={item}>{item}</option>)}</select>
      </section>

      <div className="summary-grid">{data.summaryCards.map((card) => <SummaryCard key={card.label} {...card} />)}</div>

      <section className="panel chart-panel">
        <div className="panel-header"><h3>Token volume by interval</h3></div>
        <ResponsiveContainer width="100%" height={320}>
          <BarChart data={chartData}>
            <CartesianGrid stroke="#243043" vertical={false} />
            <XAxis dataKey="label" stroke="#94a3b8" />
            <YAxis stroke="#94a3b8" />
            <Tooltip />
            <Legend />
            {models.slice(0, 6).map((modelName, index) => <Bar key={modelName} dataKey={modelName} stackId="usage" fill={[ '#60a5fa', '#818cf8', '#22c55e', '#f59e0b', '#ec4899', '#14b8a6' ][index % 6]} />)}
          </BarChart>
        </ResponsiveContainer>
      </section>

      <section className="panel chart-panel">
        <div className="panel-header"><h3>Model performance (tokens/sec)</h3></div>
        <ResponsiveContainer width="100%" height={320}>
          <LineChart data={perfData}>
            <CartesianGrid stroke="#243043" vertical={false} />
            <XAxis dataKey="time" stroke="#94a3b8" />
            <YAxis stroke="#94a3b8" />
            <Tooltip />
            <Line type="monotone" dataKey="tokensPerSecond" stroke="#22c55e" dot={false} />
          </LineChart>
        </ResponsiveContainer>
      </section>

      <section className="panel">
        <div className="panel-header"><h3>Model usage breakdown</h3></div>
        <table className="table">
          <thead><tr><th>Model</th><th>Provider</th><th>Requests</th><th>Input</th><th>Output</th><th>Avg tok/s</th><th>Avg latency</th><th>Errors</th></tr></thead>
          <tbody>
            {data.breakdown.map((row) => (
              <tr key={`${row.provider}:${row.model}`}>
                <td>{row.model}</td><td>{row.provider}</td><td>{row.requests}</td><td>{row.inputTokens.toLocaleString()}</td><td>{row.outputTokens.toLocaleString()}</td><td>{row.avgTokensPerSecond?.toFixed(1) ?? '—'}</td><td>{row.avgLatencyMs ? `${Math.round(row.avgLatencyMs)} ms` : '—'}</td><td>{row.errorCount}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      <section className="panel">
        <div className="panel-header"><h3>Usage windows</h3></div>
        <div className="window-grid">
          {data.limits.windows.map((window) => (
            <article key={window.window} className="mini-panel">
              <strong>{window.window}</strong>
              <h3>{window.trackedTokens.toLocaleString()} tokens</h3>
              <p className="muted">{window.trackedRequests} tracked requests</p>
              <p className="muted">{window.note}</p>
            </article>
          ))}
        </div>
      </section>

      <section className="panel">
        <div className="panel-header"><h3>Recent request records</h3></div>
        <div className="record-list">
          {filteredRecords.slice(0, 30).map((record) => (
            <article key={record.id} className="record-item">
              <div>
                <strong>{record.agentId}</strong>
                <p className="muted">{record.provider} / {record.model}</p>
              </div>
              <div>
                <StatusBadge status={record.status} />
                <p className="muted">{new Date(record.startedAt).toLocaleString()}</p>
              </div>
              <div><strong>{record.inputTokens.toLocaleString()} in</strong><p className="muted">{record.outputTokens.toLocaleString()} out</p></div>
            </article>
          ))}
        </div>
        {data.notes.map((note) => <p key={note} className="muted">• {note}</p>)}
      </section>
    </div>
  );
}