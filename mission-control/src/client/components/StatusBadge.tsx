export function StatusBadge({ status, dotOnly = false }: { status: string; dotOnly?: boolean }) {
  if (dotOnly) {
    return <span className={`status-dot status-dot-${status}`} aria-label={status} title={status} />;
  }

  return <span className={`badge badge-${status}`}>{status}</span>;
}
