import React, { useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { CssBaseline } from '@mui/material';
import { Provider } from 'react-redux';
import { store } from './store';
import { useAppDispatch, useAppSelector } from './utils/hooks';
import { initializeAuth } from './store/authSlice';
import { UserRole } from './types/auth.types';

// Components
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import CreatePlanetPage from './pages/CreatePlanetPage';
import UpdatePlanetPage from './pages/UpdatePlanetPage';
import PrivateRoute from './components/auth/PrivateRoute';
import RoleGuard from './components/auth/RoleGuard';
import Layout from './components/common/Layout';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
  typography: {
    h4: {
      fontWeight: 600,
    },
    h6: {
      fontWeight: 600,
    },
  },
});

const AppContent: React.FC = () => {
  const dispatch = useAppDispatch();
  const { isAuthenticated } = useAppSelector((state) => state.auth);

  useEffect(() => {
    dispatch(initializeAuth());
  }, [dispatch]);

  return (
    <Router>
      <Routes>
        <Route
          path="/login"
          element={
            isAuthenticated ? <Navigate to="/dashboard" replace /> : <LoginPage />
          }
        />
        <Route
          path="/dashboard"
          element={
            <PrivateRoute>
              <Layout>
                <DashboardPage />
              </Layout>
            </PrivateRoute>
          }
        />
        <Route
          path="/create-planet"
          element={
            <PrivateRoute>
              <RoleGuard allowedRoles={[UserRole.SuperAdmin]}>
                <Layout>
                  <CreatePlanetPage />
                </Layout>
              </RoleGuard>
            </PrivateRoute>
          }
        />
        <Route
          path="/update-planet/:id"
          element={
            <PrivateRoute>
              <RoleGuard allowedRoles={[UserRole.SuperAdmin, UserRole.PlanetAdmin]}>
                <Layout>
                  <UpdatePlanetPage />
                </Layout>
              </RoleGuard>
            </PrivateRoute>
          }
        />
        <Route
          path="/"
          element={
            <Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />
          }
        />
        <Route
          path="*"
          element={
            <Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />
          }
        />
      </Routes>
    </Router>
  );
};

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <AppContent />
      </ThemeProvider>
    </Provider>
  );
}

export default App;
