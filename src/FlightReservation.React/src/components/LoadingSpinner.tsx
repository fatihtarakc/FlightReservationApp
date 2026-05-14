export default function LoadingSpinner({ text = 'Loading...' }: { text?: string }) {
  return (
    <div className="d-flex justify-content-center align-items-center py-5">
      <div className="spinner-border text-primary me-3" role="status" />
      <span className="text-muted">{text}</span>
    </div>
  );
}
