import { createBrowserRouter } from 'react-router-dom'
import App from "../App.tsx";

export const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
  },
  {
    path: '/admin/login',
    element: <App />,
  },
  {
    path: '/admin/menu',
    element: <App />,
  },
  {
    path: '/admin/order',
    element: <App />,
  },
  {
    path: '/admin',
    element: <App />,
  }
])