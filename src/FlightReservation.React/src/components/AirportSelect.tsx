import { AIRPORTS } from '../data/airports';

interface Props {
  value: string;
  onChange: (iata: string) => void;
  label: string;
  exclude?: string;
  required?: boolean;
}

export default function AirportSelect({ value, onChange, label, exclude, required }: Props) {
  const options = exclude ? AIRPORTS.filter(a => a.iata !== exclude) : AIRPORTS;

  return (
    <div>
      <label className="form-label fw-semibold">{label}</label>
      <select
        className="form-select"
        value={value}
        onChange={e => onChange(e.target.value)}
        required={required}
      >
        <option value="">Select airport...</option>
        {options.map(a => (
          <option key={a.iata} value={a.iata}>
            {a.flag} {a.city} — {a.name}
          </option>
        ))}
      </select>
    </div>
  );
}
