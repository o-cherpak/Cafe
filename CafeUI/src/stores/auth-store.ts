import type {UserRole} from "../types/enums/enums.ts";
import type {AuthResponseDto} from "../types/auth.ts";
import {createStore} from "zustand/vanilla";
import {persist} from "zustand/middleware/persist";

interface AuthState {
  token: string | null,
  email: string | null,
  role: typeof UserRole | null,
  customerId: number | null,

  login: (data: AuthResponseDto) => void,
  logout: () => void,
  isAuthenticated: () => boolean,
}

export const useAuthStore = createStore<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      email: null,
      role: null,
      customerId: null,

      login: (data) => set({
        token: data.token,
        email: data.email,
        role: data.role,
        customerId: data.customerId ?? null,
      }),

      logout: () => set({
        token: null,
        email: null,
        role: null,
        customerId: null,
      }),

      isAuthenticated: () => get().token !== null
    }), {
      name: "auth",
    })
);

