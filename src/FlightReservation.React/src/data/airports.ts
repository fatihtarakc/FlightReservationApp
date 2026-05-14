export interface Airport {
  iata: string;
  city: string;
  name: string;
  flag: string;
  country: string;
}

export const AIRPORTS: Airport[] = [
  { iata: 'IST', city: 'Istanbul',   name: 'Istanbul Airport',   flag: '🇹🇷', country: 'Turkey' },
  { iata: 'SAW', city: 'Istanbul',   name: 'Sabiha Gökçen',      flag: '🇹🇷', country: 'Turkey' },
  { iata: 'ESB', city: 'Ankara',     name: 'Esenboğa',           flag: '🇹🇷', country: 'Turkey' },
  { iata: 'ADB', city: 'İzmir',      name: 'Adnan Menderes',     flag: '🇹🇷', country: 'Turkey' },
  { iata: 'AYT', city: 'Antalya',    name: 'Antalya Airport',    flag: '🇹🇷', country: 'Turkey' },
  { iata: 'LHR', city: 'London',     name: 'Heathrow',           flag: '🇬🇧', country: 'UK' },
  { iata: 'DXB', city: 'Dubai',      name: 'International',      flag: '🇦🇪', country: 'UAE' },
  { iata: 'JFK', city: 'New York',   name: 'JFK',                flag: '🇺🇸', country: 'USA' },
  { iata: 'FRA', city: 'Frankfurt',  name: 'Frankfurt Airport',  flag: '🇩🇪', country: 'Germany' },
  { iata: 'CDG', city: 'Paris',      name: 'Charles de Gaulle',  flag: '🇫🇷', country: 'France' },
];

export function airportLabel(iata: string): string {
  const a = AIRPORTS.find(x => x.iata === iata);
  return a ? `${a.flag} ${a.city}` : iata;
}
