import express from 'express';
import cors from 'cors';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { buildAgents, buildSnapshot, buildTokenUsage } from './providers/openclawProvider.js';

const app = express();
const port = Number(process.env.PORT ?? 4187);
const __dirname = path.dirname(fileURLToPath(import.meta.url));
const distPath = path.resolve(__dirname, '../../dist');

app.use(cors());
app.use(express.json());

app.get('/api/health', (_req, res) => res.json({ ok: true, at: new Date().toISOString() }));
app.get('/api/dashboard', (req, res) => res.json(buildSnapshot((req.query.range as any) ?? '24h')));
app.get('/api/token-usage', (req, res) => res.json(buildTokenUsage((req.query.range as any) ?? '24h')));
app.get('/api/agents', (_req, res) => res.json(buildAgents()));

app.use(express.static(distPath));
app.get('*', (_req, res) => {
  res.sendFile(path.join(distPath, 'index.html'));
});

app.listen(port, () => {
  console.log(`Mission Control server listening on http://127.0.0.1:${port}`);
});