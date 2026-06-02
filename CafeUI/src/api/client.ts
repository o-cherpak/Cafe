import * as axios from "axios";
import {useAuthStore} from "../stores/auth-store.ts";

const client = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

client.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default client