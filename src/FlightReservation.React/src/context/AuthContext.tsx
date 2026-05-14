import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  sub?: string;
  email?: string;
  unique_name?: string;
  role?: string | string[];
  exp?: number;
}

interface AuthUser {
  email: string;
  name: string;
  roles: string[];
  sub: string;
}

interface AuthContextType {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (token: string, expiration: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

function parseToken(token: string): AuthUser | null {
  try {
    const decoded = jwtDecode<JwtPayload>(token);
    const roles = Array.isArray(decoded.role)
      ? decoded.role
      : decoded.role
      ? [decoded.role]
      : [];
    return {
      email: decoded.email ?? '',
      name: decoded.unique_name ?? decoded.email ?? '',
      roles,
      sub: decoded.sub ?? '',
    };
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('jwt_token'));
  const [user, setUser] = useState<AuthUser | null>(() => {
    const t = localStorage.getItem('jwt_token');
    return t ? parseToken(t) : null;
  });

  useEffect(() => {
    const expStr = localStorage.getItem('jwt_expiration');
    if (expStr) {
      const exp = new Date(expStr);
      if (exp < new Date()) {
        logout();
      }
    }
  }, []);

  const login = (newToken: string, expiration: string) => {
    localStorage.setItem('jwt_token', newToken);
    localStorage.setItem('jwt_expiration', expiration);
    setToken(newToken);
    setUser(parseToken(newToken));
  };

  const logout = () => {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('jwt_expiration');
    setToken(null);
    setUser(null);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token,
        isAdmin: user?.roles.includes('Admin') ?? false,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
}
