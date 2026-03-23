export function SummaryCard({ label, value, helpText }: { label: string; value: string; helpText?: string }) {
  return (
    <section className="panel summary-card">
      <p className="muted">{label}</p>
      <h3>{value}</h3>
      {helpText ? <small className="muted">{helpText}</small> : null}
    </section>
  );
}