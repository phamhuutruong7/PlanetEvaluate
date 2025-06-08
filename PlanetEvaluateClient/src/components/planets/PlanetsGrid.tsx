import React from 'react';
import { Box, Typography, CircularProgress, Alert, Fab, Tooltip } from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import PlanetCard from './PlanetCard';
import { Planet } from '../../types/planet.types';
import { useAppSelector } from '../../utils/hooks';
import { canCreatePlanet } from '../../utils/permissions';

interface PlanetsGridProps {
  planets: Planet[];
  isLoading: boolean;
  error: string | null;
  onEditPlanet?: (planet: Planet) => void;
  onDeletePlanet?: (planet: Planet) => void;
  onAddPlanet?: () => void;
}

const PlanetsGrid: React.FC<PlanetsGridProps> = ({
  planets,
  isLoading,
  error,
  onEditPlanet,
  onDeletePlanet,
  onAddPlanet,
}) => {
  const { user } = useAppSelector((state) => state.auth);
  const canUserCreatePlanet = canCreatePlanet(user?.role);
  if (isLoading) {
    return (
      <Box 
        display="flex" 
        justifyContent="center" 
        alignItems="center" 
        minHeight="400px"
      >
        <Box textAlign="center">
          <CircularProgress size={60} sx={{ mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            Loading planets...
          </Typography>
        </Box>
      </Box>
    );
  }

  if (error) {
    return (
      <Box mb={3}>
        <Alert severity="error" variant="outlined">
          <Typography variant="h6" gutterBottom>
            Error Loading Planets
          </Typography>
          {error}
        </Alert>
      </Box>
    );
  }

  if (planets.length === 0) {
    return (
      <Box 
        display="flex" 
        flexDirection="column"
        alignItems="center" 
        justifyContent="center" 
        minHeight="400px"
        textAlign="center"
      >
        <Typography variant="h4" color="text.secondary" gutterBottom>
          🪐
        </Typography>
        <Typography variant="h6" color="text.secondary" gutterBottom>
          No Planets Found
        </Typography>
        <Typography variant="body1" color="text.secondary" mb={3}>
          Start exploring the galaxy by adding your first planet!
        </Typography>        {onAddPlanet && canUserCreatePlanet && (
          <Fab 
            color="primary" 
            variant="extended"
            onClick={onAddPlanet}
            sx={{ mt: 2 }}
          >
            <AddIcon sx={{ mr: 1 }} />
            Add Planet
          </Fab>
        )}
      </Box>
    );
  }

  return (
    <Box sx={{ position: 'relative' }}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" component="h2" fontWeight="bold">
          Planets ({planets.length})
        </Typography>        {onAddPlanet && canUserCreatePlanet && (
          <Tooltip title="Add New Planet">
            <Fab 
              color="primary" 
              size="medium"
              onClick={onAddPlanet}
              sx={{
                boxShadow: 3,
                '&:hover': {
                  boxShadow: 6,
                }
              }}
            >
              <AddIcon />
            </Fab>
          </Tooltip>
        )}
      </Box>      {/* Planets Grid */}
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(2, 1fr)',
            md: 'repeat(3, 1fr)',
            lg: 'repeat(4, 1fr)',
          },
          gap: 3,
        }}
      >        {planets.map((planet) => (
          <PlanetCard
            key={planet.id}
            planet={planet}
            onEdit={onEditPlanet}
            onDelete={onDeletePlanet}
          />
        ))}
      </Box>      {/* Floating Action Button for Add (Alternative position) */}
      {onAddPlanet && canUserCreatePlanet && planets.length > 0 && (
        <Fab
          color="primary"
          aria-label="add planet"
          onClick={onAddPlanet}
          sx={{
            position: 'fixed',
            bottom: 24,
            right: 24,
            zIndex: 1000,
            boxShadow: 4,
            '&:hover': {
              boxShadow: 8,
            }
          }}
        >
          <AddIcon />
        </Fab>
      )}
    </Box>
  );
};

export default PlanetsGrid;
