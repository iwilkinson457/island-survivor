import { AgentsResponse, DashboardSnapshot, TimeRange, TokenUsageResponse } from './types';

async function getJson<T>(url: string): Promise<T> {
  const response = await fetch(url);
  if (!response.ok) throw new Error(`Request failed: ${response.status}`);
  return response.json() as Promise<T>;
}

export const api = {
  dashboard: (range: TimeRange) => getJson<DashboardSnapshot>(`/api/dashboard?range=${range}`),
  tokenUsage: (range: TimeRange) => getJson<TokenUsageResponse>(`/api/token-usage?range=${range}`),
  agents: () => getJson<AgentsResponse>('/api/agents')
};