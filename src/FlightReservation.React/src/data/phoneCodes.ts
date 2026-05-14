export interface CountryPhone {
  flag: string;
  name: string;
  code: string;
  pattern: string; // hint pattern for local number
}

export const PHONE_CODES: CountryPhone[] = [
  { flag: '🇹🇷', name: 'Turkey',       code: '+90',  pattern: '5XX XXX XX XX' },
  { flag: '🇺🇸', name: 'USA/Canada',   code: '+1',   pattern: 'XXX XXX XXXX' },
  { flag: '🇬🇧', name: 'UK',           code: '+44',  pattern: 'XXXX XXXXXX' },
  { flag: '🇩🇪', name: 'Germany',      code: '+49',  pattern: 'XXX XXXXXXX' },
  { flag: '🇫🇷', name: 'France',       code: '+33',  pattern: 'X XX XX XX XX' },
  { flag: '🇦🇪', name: 'UAE',          code: '+971', pattern: '5X XXX XXXX' },
  { flag: '🇮🇹', name: 'Italy',        code: '+39',  pattern: 'XXX XXX XXXX' },
  { flag: '🇪🇸', name: 'Spain',        code: '+34',  pattern: 'XXX XXX XXX' },
  { flag: '🇳🇱', name: 'Netherlands',  code: '+31',  pattern: 'X XX XX XX XX' },
  { flag: '🇧🇪', name: 'Belgium',      code: '+32',  pattern: 'XXX XX XX XX' },
  { flag: '🇨🇭', name: 'Switzerland',  code: '+41',  pattern: 'XX XXX XX XX' },
  { flag: '🇦🇹', name: 'Austria',      code: '+43',  pattern: 'XXX XXXXXXX' },
  { flag: '🇵🇱', name: 'Poland',       code: '+48',  pattern: 'XXX XXX XXX' },
  { flag: '🇸🇪', name: 'Sweden',       code: '+46',  pattern: 'XX XXX XX XX' },
  { flag: '🇳🇴', name: 'Norway',       code: '+47',  pattern: 'XXX XX XXX' },
  { flag: '🇩🇰', name: 'Denmark',      code: '+45',  pattern: 'XX XX XX XX' },
  { flag: '🇬🇷', name: 'Greece',       code: '+30',  pattern: 'XXX XXX XXXX' },
  { flag: '🇵🇹', name: 'Portugal',     code: '+351', pattern: 'XXX XXX XXX' },
  { flag: '🇷🇺', name: 'Russia',       code: '+7',   pattern: 'XXX XXX XX XX' },
  { flag: '🇯🇵', name: 'Japan',        code: '+81',  pattern: 'XX XXXX XXXX' },
  { flag: '🇨🇳', name: 'China',        code: '+86',  pattern: 'XXX XXXX XXXX' },
  { flag: '🇮🇳', name: 'India',        code: '+91',  pattern: 'XXXXX XXXXX' },
  { flag: '🇧🇷', name: 'Brazil',       code: '+55',  pattern: 'XX XXXXX XXXX' },
  { flag: '🇦🇺', name: 'Australia',    code: '+61',  pattern: 'XXX XXX XXX' },
];
