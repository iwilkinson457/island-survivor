import { useEffect, useState } from 'react';

export function usePollingQuery<T>(loader: () => Promise<T>, refreshMs: number, deps: unknown[] = []) {
  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    let timer: number | undefined;

    const run = async () => {
      try {
        setLoading((prev) => !data || prev);
        const next = await loader();
        if (!cancelled) {
          setData(next);
          setError(null);
          setLoading(false);
        }
      } catch (err) {
        if (!cancelled) {
          setError(err instanceof Error ? err.message : 'Unknown error');
          setLoading(false);
        }
      } finally {
        if (!cancelled) timer = window.setTimeout(run, refreshMs);
      }
    };

    run();
    return () => {
      cancelled = true;
      if (timer) window.clearTimeout(timer);
    };
  }, deps);

  return { data, error, loading };
}