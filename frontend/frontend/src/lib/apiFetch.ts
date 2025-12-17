import { getAccessToken, saveAccessToken } from "@/utils/auth";
import { logout } from "@/utils/auth";
const API_BASE = "http://localhost:5051";

export async function apiFetch(input: RequestInfo, init?: RequestInit) {
  const accessToken = getAccessToken();

  // make request - with url given and other parameters and include for cookies
  const res = await fetch(input, {
    credentials: "include",
    ...init,
    headers: {
      ...init?.headers,
      ...(accessToken && {
        Authorization: `Bearer ${accessToken}`,
      }),
    },
  });

  if (res.status === 401) {
    const refresh = await fetch(`${API_BASE}/api/users/refresh`, {
      method: "POST",
      credentials: "include",
    });

    // refresh expires or invalid --> logout
    if (!refresh.ok) {
      logout();
      alert("Session expired. Please log in again.");
      throw new Error("Session expired");
    }
    // save new access token
    const { accessToken } = await refresh.json();
    saveAccessToken(accessToken);

    // retry same request with new access token
    return fetch(input, {
      credentials: "include",
      ...init,
      headers: {
        ...init?.headers,
        Authorization: `Bearer ${accessToken}`,
      },
    });
  }

  return res;
}
