const BASE_URL = '/api/';

function getToken(): string | null {
  return localStorage.getItem('jwt_token');
}

async function request<T>(method: string, path: string, body?: unknown): Promise<{ data: T }> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' };
  const token = getToken();
  if (token) headers['Authorization'] = `Bearer ${token}`;

  const res = await fetch(BASE_URL + path, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });

  if (res.status === 401) {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('jwt_expiration');
    window.location.href = '/login';
    throw new Error('Unauthorized');
  }

  const json = await res.json() as Record<string, unknown>;

  // Normalize isSuccess → success (API uses isSuccess, types use success)
  if ('isSuccess' in json) {
    json['success'] = json['isSuccess'];
  }

  return { data: json as T };
}

export const api = {
  get:    <T>(path: string)                => request<T>('GET',    path),
  post:   <T>(path: string, body: unknown) => request<T>('POST',   path, body),
  put:    <T>(path: string, body: unknown) => request<T>('PUT',    path, body),
  delete: <T>(path: string)               => request<T>('DELETE', path),
};
