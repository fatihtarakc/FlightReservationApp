import { useState, useEffect } from 'react';
import { PHONE_CODES } from '../data/phoneCodes';

interface Props {
  value: string;
  onChange: (fullNumber: string) => void;
  error?: string;
  required?: boolean;
}

export default function PhoneInput({ value, onChange, error, required }: Props) {
  const [code, setCode] = useState('+90');
  const [local, setLocal] = useState('');

  useEffect(() => {
    if (value && value.startsWith('+')) {
      const match = PHONE_CODES.find(c => value.startsWith(c.code));
      if (match) {
        setCode(match.code);
        setLocal(value.slice(match.code.length));
      }
    }
  }, []);

  const handleCodeChange = (newCode: string) => {
    setCode(newCode);
    const digits = local.replace(/\D/g, '');
    onChange(digits ? newCode + digits : '');
  };

  const handleLocalChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const digits = e.target.value.replace(/\D/g, '').slice(0, 12);
    setLocal(digits);
    onChange(digits ? code + digits : '');
  };

  const selected = PHONE_CODES.find(c => c.code === code) ?? PHONE_CODES[0];

  return (
    <div>
      <div className={`input-group ${error ? 'is-invalid' : ''}`}>
        <select
          className="form-select flex-shrink-0"
          style={{ maxWidth: '160px' }}
          value={code}
          onChange={e => handleCodeChange(e.target.value)}
        >
          {PHONE_CODES.map(c => (
            <option key={c.code + c.name} value={c.code}>
              {c.flag} {c.code} {c.name}
            </option>
          ))}
        </select>
        <input
          type="tel"
          className={`form-control ${error ? 'is-invalid' : ''}`}
          placeholder={selected.pattern}
          value={local}
          onChange={handleLocalChange}
          required={required}
          minLength={6}
          maxLength={12}
          inputMode="tel"
        />
      </div>
      {error && <div className="invalid-feedback d-block">{error}</div>}
      {!error && local.length > 0 && (
        <small className="text-muted">{code}{local}</small>
      )}
    </div>
  );
}
