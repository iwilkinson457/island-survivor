import { useMemo, useState } from 'react';
import { Bar, BarChart, CartesianGrid, Cell, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { api } from '../lib/api';
import { DashboardSource, TimeRange, TokenUsageRecord } from '../lib/types';
import { usePollingQuery } from '../lib/usePollingQuery';
import { EmptyState, ErrorState, LoadingState } from '../components/States';
import { StatusBadge } from '../components/StatusBadge';
import { SourceList } from '../components/SourceList';

const CHART_COLORS = ['#53b3ff', '#6f86ff', '#2dd4bf', '#f59e0b', '#f472b6', '#34d399'];

export function TokenUsagePage({ sources }: { sources: DashboardSource[] }) {
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

  const models = useMemo(() => {
    if (!data) return [] as string[];
    const seen = new Set(filteredRecords.map((item) => item.model));
    return data.filterOptions.models.filter((item) => seen.size === 0 || seen.has(item));
  }, [data, filteredRecords]);

  const chartData = useMemo(() => {
    if (!data) return [];
    const allowedModels = new Set(filteredRecords.map((item) => item.model));
    return data.buckets.map((bucket) => {
      const row: Record<string, string | number> = {
        label: bucket.label,
        requests: bucket.requests
      };

      for (const [modelName, total] of Object.entries(bucket.totalsByModel)) {
        if (!allowedModels.size || allowedModels.has(modelName)) row[modelName] = total;
      }

      return row;
    });
  }, [data, filteredRecords]);

  const visibleBreakdown = useMemo(() => {
    return (data?.breakdown ?? []).filter((row) =>
      (provider === 'all' || row.provider === provider) &&
      (model === 'all' || row.model === model)
    );
  }, [data, provider, model]);

  const perfData = useMemo(() => {
    return filteredRecords
      .filter((item) => item.tokensPerSecond)
      .slice(0, 60)
      .reverse()
      .map((item) => ({
        time: shortTime(item.startedAt),
        model: item.model,
        tokensPerSecond: item.tokensPerSecond!
      }));
  }, [filteredRecords]);

  const totals = useMemo(() => summarize(filteredRecords), [filteredRecords]);

  if (loading && !data) return <LoadingState />;
  if (error && !data) return <ErrorState message={error} />;
  if (!data) return <EmptyState>No token telemetry found.</EmptyState>;

  return (
    <div className="page-grid">
      <section className="page-header panel">
        <div>
          <p className="eyebrow">Token telemetry</p>
          <h2>Provider, model, and agent usage</h2>
          <p className="muted">Same live OpenClaw data, redesigned into a denser operational console. Refreshes every {Math.round(data.refreshMs / 1000)}s.</p>
        </div>
        <div className="header-actions">
          <div className="segmented-control">
            {(['24h', '7d', '30d'] as TimeRange[]).map((option) => (
              <button key={option} type="button" className={option === range ? 'active' : ''} onClick={() => setRange(option)}>{option}</button>
            ))}
          </div>
          <div className="refresh-note">
            <span className="live-pill"><span className="live-dot" /> Live</span>
            <span className="muted compact">Updated {formatRelative(data.generatedAt)}</span>
          </div>
        </div>
      </section>

      <section className="stats-strip panel">
        {data.summaryCards.map((card) => (
          <div key={card.label} className="stat-cell">
            <span className="stat-label">{card.label}</span>
            <strong className="stat-value">{card.value}</strong>
            {card.helpText ? <span className="muted compact">{card.helpText}</span> : null}
          </div>
        ))}
      </section>

      <section className="filter-toolbar panel">
        <div className="filter-group">
          <label>Agent</label>
          <select value={agent} onChange={(e) => setAgent(e.target.value)}>
            <option value="all">All agents</option>
            {data.filterOptions.agents.map((item) => <option key={item}>{item}</option>)}
          </select>
        </div>
        <div className="filter-group">
          <label>Provider</label>
          <select value={provider} onChange={(e) => setProvider(e.target.value)}>
            <option value="all">All providers</option>
            {data.filterOptions.providers.map((item) => <option key={item}>{item}</option>)}
          </select>
        </div>
        <div className="filter-group">
          <label>Model</label>
          <select value={model} onChange={(e) => setModel(e.target.value)}>
            <option value="all">All models</option>
            {data.filterOptions.models.map((item) => <option key={item}>{item}</option>)}
          </select>
        </div>
        <div className="filter-group">
          <label>Outcome</label>
          <select value={status} onChange={(e) => setStatus(e.target.value)}>
            <option value="all">All outcomes</option>
            {data.filterOptions.statuses.map((item) => <option key={item}>{item}</option>)}
          </select>
        </div>
        <div className="toolbar-summary">
          <span className="muted">Filtered requests</span>
          <strong>{filteredRecords.length.toLocaleString()}</strong>
        </div>
      </section>

      <section className="dashboard-grid dashboard-grid-token">
        <article className="panel panel-emphasis">
          <div className="section-head">
            <div>
              <p className="eyebrow">Volume</p>
              <h3>Token volume by interval</h3>
            </div>
            <span className="muted compact">Stacked by model</span>
          </div>
          <div className="chart-shell tall-chart">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={chartData}>
                <CartesianGrid stroke="#1f3148" vertical={false} />
                <XAxis dataKey="label" stroke="#7f93ad" />
                <YAxis stroke="#7f93ad" />
                <Tooltip contentStyle={tooltipStyle} labelStyle={tooltipLabelStyle} />
                {models.slice(0, 6).map((modelName, index) => (
                  <Bar key={modelName} dataKey={modelName} stackId="usage" radius={[3, 3, 0, 0]}>
                    {chartData.map((_, rowIndex) => (
                      <Cell key={`${modelName}-${rowIndex}`} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                    ))}
                  </Bar>
                ))}
              </BarChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="panel side-panel">
          <div className="section-head">
            <div>
              <p className="eyebrow">Breakdown</p>
              <h3>Model usage detail</h3>
            </div>
            <span className="muted compact">{visibleBreakdown.length} rows</span>
          </div>
          <div className="table-wrap compact-table-wrap">
            <table className="table data-table">
              <thead>
                <tr>
                  <th>Model</th>
                  <th>Provider</th>
                  <th>Req</th>
                  <th>Input</th>
                  <th>Output</th>
                  <th>Tok/s</th>
                  <th>Latency</th>
                  <th>Err</th>
                </tr>
              </thead>
              <tbody>
                {visibleBreakdown.map((row) => (
                  <tr key={`${row.provider}:${row.model}`}>
                    <td>{row.model}</td>
                    <td>{row.provider}</td>
                    <td>{row.requests}</td>
                    <td>{row.inputTokens.toLocaleString()}</td>
                    <td>{row.outputTokens.toLocaleString()}</td>
                    <td>{row.avgTokensPerSecond?.toFixed(1) ?? '—'}</td>
                    <td>{row.avgLatencyMs ? `${Math.round(row.avgLatencyMs)} ms` : '—'}</td>
                    <td>{row.errorCount}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </article>
      </section>

      <section className="dashboard-grid dashboard-grid-secondary">
        <article className="panel">
          <div className="section-head">
            <div>
              <p className="eyebrow">Performance</p>
              <h3>Response speed</h3>
            </div>
            <span className="muted compact">Recent tokens/sec observations</span>
          </div>
          <div className="chart-shell medium-chart">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={perfData}>
                <CartesianGrid stroke="#1f3148" vertical={false} />
                <XAxis dataKey="time" stroke="#7f93ad" />
                <YAxis stroke="#7f93ad" />
                <Tooltip contentStyle={tooltipStyle} labelStyle={tooltipLabelStyle} />
                <Line type="monotone" dataKey="tokensPerSecond" stroke="#2dd4bf" strokeWidth={2} dot={false} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="panel">
          <div className="section-head">
            <div>
              <p className="eyebrow">Current slice</p>
              <h3>Filtered totals</h3>
            </div>
          </div>
          <div className="metric-stack">
            <div className="metric-row-line"><span className="muted">Input tokens</span><strong>{totals.input.toLocaleString()}</strong></div>
            <div className="metric-row-line"><span className="muted">Output tokens</span><strong>{totals.output.toLocaleString()}</strong></div>
            <div className="metric-row-line"><span className="muted">Total tokens</span><strong>{totals.total.toLocaleString()}</strong></div>
            <div className="metric-row-line"><span className="muted">Cache read</span><strong>{totals.cacheRead.toLocaleString()}</strong></div>
            <div className="metric-row-line"><span className="muted">Avg tokens/request</span><strong>{totals.avgPerRequest.toFixed(0)}</strong></div>
            <div className="metric-row-line"><span className="muted">Unique models</span><strong>{new Set(filteredRecords.map((item) => item.model)).size}</strong></div>
          </div>
        </article>

        <article className="panel">
          <div className="section-head">
            <div>
              <p className="eyebrow">Tracked windows</p>
              <h3>Usage windows</h3>
            </div>
          </div>
          <div className="window-stack">
            {data.limits.windows.map((window) => (
              <article key={window.window} className="window-card">
                <strong>{window.window}</strong>
                <h4>{window.trackedTokens.toLocaleString()} tokens</h4>
                <p className="muted">{window.trackedRequests} tracked requests</p>
                <p className="muted compact">{window.note}</p>
              </article>
            ))}
          </div>
        </article>
      </section>

      <section className="panel notes-panel">
        <div className="section-head">
          <div>
            <p className="eyebrow">Interpretation</p>
            <h3>Operator notes</h3>
          </div>
        </div>
        <ul className="notes-list">
          {data.notes.map((note) => <li key={note}>{note}</li>)}
        </ul>
      </section>

      <section className="panel">
        <div className="section-head">
          <div>
            <p className="eyebrow">Recent activity</p>
            <h3>Request log</h3>
          </div>
          <span className="muted compact">Showing {Math.min(filteredRecords.length, 30)} of {filteredRecords.length}</span>
        </div>
        <div className="table-wrap">
          <table className="table data-table dense-table">
            <thead>
              <tr>
                <th>Status</th>
                <th>Agent</th>
                <th>Provider / model</th>
                <th>Input</th>
                <th>Output</th>
                <th>Latency</th>
                <th>Started</th>
              </tr>
            </thead>
            <tbody>
              {filteredRecords.slice(0, 30).map((record) => (
                <RequestRow key={record.id} record={record} />
              ))}
            </tbody>
          </table>
        </div>
      </section>

      <SourceList sources={sources} />
    </div>
  );
}

function RequestRow({ record }: { record: TokenUsageRecord }) {
  return (
    <tr>
      <td><StatusBadge status={record.status} dotOnly /></td>
      <td>{record.agentId}</td>
      <td>
        <div className="table-primary">{record.provider}</div>
        <div className="table-secondary">{record.model}</div>
      </td>
      <td>{record.inputTokens.toLocaleString()}</td>
      <td>{record.outputTokens.toLocaleString()}</td>
      <td>{record.latencyMs ? `${Math.round(record.latencyMs)} ms` : '—'}</td>
      <td>
        <div className="table-primary">{formatRelative(record.startedAt)}</div>
        <div className="table-secondary">{new Date(record.startedAt).toLocaleString()}</div>
      </td>
    </tr>
  );
}

function summarize(records: TokenUsageRecord[]) {
  const input = records.reduce((sum, item) => sum + item.inputTokens, 0);
  const output = records.reduce((sum, item) => sum + item.outputTokens, 0);
  const total = records.reduce((sum, item) => sum + item.totalTokens, 0);
  const cacheRead = records.reduce((sum, item) => sum + (item.cacheReadTokens ?? 0), 0);
  const avgPerRequest = records.length ? total / records.length : 0;
  return { input, output, total, cacheRead, avgPerRequest };
}

function shortTime(value: string) {
  return new Date(value).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
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

const tooltipStyle = {
  background: '#09111d',
  border: '1px solid #203149',
  borderRadius: 12,
  color: '#e8f0fb'
};

const tooltipLabelStyle = {
  color: '#9cb0c8'
};
