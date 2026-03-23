import { createBrowserRouter } from "react-router";
import { ProtectedRoute } from "../auth/ProtectedRoute";
import AppLayout from "../layouts/AppLayout";
import DashboardPage from "../pages/DashboardPage";
import LoginPage from "../pages/LoginPage";
import NodesPage from "../pages/NodesPage";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <AppLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <DashboardPage />,
      },
      {
        path: "nodes",
        element: <NodesPage />,
      },
    ],
  },
]);