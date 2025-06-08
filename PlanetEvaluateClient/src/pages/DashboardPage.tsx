import React, { useEffect, useState } from 'react';
import {
  Container,
  Typography,
  Box,
  Paper,
  Button,
  Card,
  CardContent,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  Alert,
} from '@mui/material';
import { useAppDispatch, useAppSelector } from '../utils/hooks';
import { logout, initializeAuth } from '../store/authSlice';
import { fetchPlanets, deletePlanet } from '../store/planetsSlice';
import { useNavigate, useLocation } from 'react-router-dom';
import PlanetsGrid from '../components/planets/PlanetsGrid';
import { Planet } from '../types/planet.types';
import { canCreatePlanet, canEditPlanet, canDeletePlanet } from '../utils/permissions';

const DashboardPage: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAppSelector((state) => state.auth);
  const { planets, isLoading, error } = useAppSelector((state) => state.planets);
  
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [planetToDelete, setPlanetToDelete] = useState<Planet | null>(null);
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [snackbarSeverity, setSnackbarSeverity] = useState<'success' | 'error' | 'warning' | 'info'>('info');
  // Check if user has permission to create planets
  const canUserCreatePlanet = canCreatePlanet(user?.role);
  const canUserEditPlanet = canEditPlanet(user?.role);
  const canUserDeletePlanet = canDeletePlanet(user?.role);

  // Handle unauthorized access message from navigation state
  useEffect(() => {
    if (location.state?.message) {
      setSnackbarMessage(location.state.message);
      setSnackbarSeverity('warning');
      setSnackbarOpen(true);
      // Clear the state to prevent showing the message again
      navigate(location.pathname, { replace: true, state: {} });
    }  }, [location.state, navigate, location.pathname]);

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => {
    console.log('=== DASHBOARD MOUNTED ===');
    console.log('User object:', user);
    console.log('User exists:', !!user);    console.log('Token in localStorage:', {
      exists: !!localStorage.getItem('authToken'), // Changed from 'token' to 'authToken'
      length: localStorage.getItem('authToken')?.length || 0,
      preview: localStorage.getItem('authToken')?.substring(0, 50) + '...'
    });
    console.log('Current planets state:', {
      count: planets.length,
      isLoading,
      error,
      planets: planets.slice(0, 2) // Show first 2 planets for debugging
    });
      // Only fetch planets if user is authenticated and we have a token
    if (user && localStorage.getItem('authToken')) { // Changed from 'token' to 'authToken'
      console.log('✅ Conditions met for fetching planets - dispatching fetchPlanets...');
      dispatch(fetchPlanets())
        .unwrap()
        .then((result) => {
          console.log('✅ fetchPlanets fulfilled with result:', result);
        })
        .catch((error) => {
          console.error('❌ fetchPlanets rejected with error:', error);
        });
    } else {
      console.log('❌ NOT fetching planets. Reasons:', {
        userExists: !!user,
        tokenExists: !!localStorage.getItem('authToken'), // Changed from 'token' to 'authToken'
        userDetails: user ? { id: user.id, username: user.username } : null      });
    }
  }, [dispatch, user]);
  
  useEffect(() => {
    console.log('Planets state updated:', { planets: planets.length, isLoading, error });
  }, [planets, isLoading, error]);

  // Debug effect to track auth state changes
  useEffect(() => {
    console.log('=== AUTH STATE CHANGED ===');
    console.log('New user state:', user);
    console.log('Is authenticated:', !!user);
    if (user) {
      console.log('User details:', {
        id: user.id,
        username: user.username,
        email: user.email,
        role: user.role
      });
    }
  }, [user]);
  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  const handleEditPlanet = (planet: Planet) => {
    if (!canUserEditPlanet) {
      setSnackbarMessage('You do not have permission to edit planets. Only SuperAdmin and PlanetAdmin users can edit planets.');
      setSnackbarSeverity('warning');
      setSnackbarOpen(true);
      return;
    }
    
    // Navigate to planet edit page
    navigate(`/update-planet/${planet.id}`);
  };
  const handleDeletePlanet = (planet: Planet) => {
    if (!canUserDeletePlanet) {
      setSnackbarMessage('You do not have permission to delete planets. Only SuperAdmin users can delete planets.');
      setSnackbarSeverity('warning');
      setSnackbarOpen(true);
      return;
    }
    
    setPlanetToDelete(planet);
    setDeleteDialogOpen(true);
  };
  const confirmDeletePlanet = async () => {
    if (planetToDelete) {
      try {
        await dispatch(deletePlanet(planetToDelete.id)).unwrap();
        setSnackbarMessage(`${planetToDelete.name} has been deleted`);
        setSnackbarSeverity('success');
        setSnackbarOpen(true);
      } catch (error) {
        setSnackbarMessage(`Failed to delete ${planetToDelete.name}`);
        setSnackbarSeverity('error');
        setSnackbarOpen(true);
      }
    }
    setDeleteDialogOpen(false);
    setPlanetToDelete(null);
  };const handleAddPlanet = () => {
    if (canUserCreatePlanet) {
      navigate('/create-planet');
    } else {
      setSnackbarMessage('You do not have permission to create planets. Only SuperAdmin users can create planets.');
      setSnackbarSeverity('warning');
      setSnackbarOpen(true);
    }
  };

  const handleCloseSnackbar = () => {
    setSnackbarOpen(false);
  };
  return (
    <Container maxWidth="lg">
      <Box sx={{ mt: 4, mb: 4 }}>
        {/* Header Section */}
        <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <Typography variant="h4" component="h1">
              Planet Evaluation Dashboard
            </Typography>
            <Button variant="outlined" color="secondary" onClick={handleLogout}>
              Logout
            </Button>
          </Box>
          
          {/* User Info Cards */}
          <Box 
            sx={{ 
              display: 'flex', 
              flexDirection: { xs: 'column', md: 'row' }, 
              gap: 3 
            }}
          >            <Box sx={{ flex: 1 }}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Welcome back, {user?.username}!
                  </Typography>
                  <Typography variant="body1" color="text.secondary">
                    Email: {user?.email}
                  </Typography>
                  <Typography variant="body1" color="text.secondary">
                    Role: {user?.role}
                  </Typography>
                  {user?.role === 'SuperAdmin' && (
                    <Typography variant="body2" color="primary" sx={{ mt: 1, fontWeight: 'bold' }}>
                      ✨ Full system access - Can create, edit, and delete planets
                    </Typography>
                  )}
                  {user?.role === 'PlanetAdmin' && (
                    <Typography variant="body2" color="info.main" sx={{ mt: 1, fontWeight: 'bold' }}>
                      🌍 Planet Administrator - Can edit planets
                    </Typography>
                  )}
                  {(user?.role === 'ViewerType1' || user?.role === 'ViewerType2') && (
                    <Typography variant="body2" color="text.secondary" sx={{ mt: 1, fontStyle: 'italic' }}>
                      👁️ Viewer access - Read-only permissions
                    </Typography>
                  )}
                  {user?.assignedPlanetId && (
                    <Typography variant="body1" color="text.secondary">
                      Assigned Planet ID: {user.assignedPlanetId}
                    </Typography>
                  )}
                </CardContent>
              </Card>
            </Box>
            <Box sx={{ flex: 1 }}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    System Status
                  </Typography>
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    Total Planets: {planets.length}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Ready for planet evaluation and exploration.
                  </Typography>
                </CardContent>
              </Card>
            </Box>
          </Box>
        </Paper>        {/* Planets Section */}
        <Paper elevation={3} sx={{ p: 3 }}>
          {/* Debug Section - Remove this later */}
          <PlanetsGrid
            planets={planets}
            isLoading={isLoading}
            error={error}
            onEditPlanet={handleEditPlanet}
            onDeletePlanet={handleDeletePlanet}
            onAddPlanet={canUserCreatePlanet ? handleAddPlanet : undefined}
          />
        </Paper>
      </Box>

      {/* Delete Confirmation Dialog */}
      <Dialog
        open={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          Confirm Delete
        </DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete <strong>{planetToDelete?.name}</strong>? 
            This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>
            Cancel
          </Button>
          <Button 
            onClick={confirmDeletePlanet} 
            color="error" 
            variant="contained"
          >
            Delete
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar for notifications */}      <Snackbar
        open={snackbarOpen}
        autoHideDuration={6000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert 
          onClose={handleCloseSnackbar} 
          severity={snackbarSeverity} 
          variant="filled"
          sx={{ width: '100%' }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Container>
  );
};

export default DashboardPage;
