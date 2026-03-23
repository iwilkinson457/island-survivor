import { Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { TokenUsagePage } from './pages/TokenUsagePage';
import { AgentsPage } from './pages/AgentsPage';
import { api } from './lib/api';
import { usePollingQuery } from './lib/usePollingQuery';
import { LoadingState } from './components/States';
import { SourceList } from './components/SourceList';

export function App() {
  const { data } = usePollingQuery(() => api.dashboard('24h'), 30000, []);

  return (
    <Layout>
      {!data ? <LoadingState /> : <SourceList sources={data.dataSources} />}
      <Routes>
        <Route path="/" element={<TokenUsagePage />} />
        <Route path="/agents" element={<AgentsPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  );
}