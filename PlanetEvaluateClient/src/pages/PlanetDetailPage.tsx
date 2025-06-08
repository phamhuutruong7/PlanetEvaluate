import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../utils/hooks';
import { fetchPlanetById } from '../store/planetsSlice';
import { canEditPlanet, canDeletePlanet } from '../utils/permissions';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Paper,
  Divider,
  Chip,
  CircularProgress,
  Alert,
  LinearProgress,
} from '@mui/material';
import {
  Public as PlanetIcon,
  ArrowBack as BackIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Air as AtmosphereIcon,
  Terrain as TerrainIcon,
} from '@mui/icons-material';
import { Planet } from '../types/planet.types';

const PlanetDetailPage: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { id } = useParams<{ id: string }>();
  const { planets } = useAppSelector((state) => state.planets);
  const { user } = useAppSelector((state) => state.auth);

  const [planet, setPlanet] = useState<Planet | null>(null);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [isLoadingPlanet, setIsLoadingPlanet] = useState(false);

  // Permission checks
  const canUserEditPlanet = canEditPlanet(user?.role);
  const canUserDeletePlanet = canDeletePlanet(user?.role);

  // Load planet data on component mount
  useEffect(() => {
    const loadPlanetData = async () => {
      if (!id) {
        navigate('/dashboard');
        return;
      }

      const planetId = Number(id);
      let targetPlanet = planets.find(p => p.id === planetId);

      // If planet is not in the store, fetch it
      if (!targetPlanet) {
        setIsLoadingPlanet(true);
        try {
          const fetchedPlanet = await dispatch(fetchPlanetById(planetId)).unwrap();
          targetPlanet = fetchedPlanet;
        } catch (error: any) {
          setLoadError('Failed to load planet data');
          setIsLoadingPlanet(false);
          return;
        }
        setIsLoadingPlanet(false);
      }

      setPlanet(targetPlanet || null);
    };

    loadPlanetData();
  }, [id, planets, dispatch, navigate]);

  const handleBack = () => {
    navigate('/dashboard');
  };

  const handleEdit = () => {
    if (planet) {
      navigate(`/update-planet/${planet.id}`);
    }
  };

  const handleDelete = () => {
    if (planet) {
      // Navigate back to dashboard with planet deletion request
      navigate('/dashboard', { 
        state: { 
          requestDelete: planet.id,
          message: `Delete request for ${planet.name}` 
        } 
      });
    }
  };

  // Show loading spinner while fetching planet data
  if (isLoadingPlanet) {
    return (
      <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50', py: 4, display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
        <CircularProgress size={60} />
      </Box>
    );
  }

  // Show error if failed to load
  if (loadError) {
    return (
      <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50', py: 4 }}>
        <Box sx={{ maxWidth: 'lg', mx: 'auto', px: 3 }}>
          <Alert severity="error" sx={{ mb: 3 }}>
            {loadError}
          </Alert>
          <Button variant="outlined" startIcon={<BackIcon />} onClick={handleBack}>
            Back to Dashboard
          </Button>
        </Box>
      </Box>
    );
  }

  // Show not found if planet doesn't exist
  if (!planet) {
    return (
      <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50', py: 4 }}>
        <Box sx={{ maxWidth: 'lg', mx: 'auto', px: 3 }}>
          <Alert severity="warning" sx={{ mb: 3 }}>
            Planet not found
          </Alert>
          <Button variant="outlined" startIcon={<BackIcon />} onClick={handleBack}>
            Back to Dashboard
          </Button>
        </Box>
      </Box>
    );
  }

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50', py: 4 }}>
      <Box sx={{ maxWidth: 'lg', mx: 'auto', px: 3 }}>
        {/* Header */}
        <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
          <Box display="flex" alignItems="center" justifyContent="space-between">
            <Box display="flex" alignItems="center">
              <PlanetIcon sx={{ fontSize: 32, mr: 2, color: 'primary.main' }} />
              <Box>
                <Typography variant="h4" component="h1" fontWeight="bold">
                  {planet.name}
                </Typography>
                <Typography variant="subtitle1" color="text.secondary">
                  {planet.type} Planet
                </Typography>
              </Box>
            </Box>
            <Box display="flex" gap={2}>
              {canUserEditPlanet && (
                <Button
                  variant="outlined"
                  startIcon={<EditIcon />}
                  onClick={handleEdit}
                  color="primary"
                >
                  Edit Planet
                </Button>
              )}
              {canUserDeletePlanet && (
                <Button
                  variant="outlined"
                  startIcon={<DeleteIcon />}
                  onClick={handleDelete}
                  color="error"
                >
                  Delete Planet
                </Button>
              )}
              <Button
                variant="outlined"
                startIcon={<BackIcon />}
                onClick={handleBack}
              >
                Back to Dashboard
              </Button>
            </Box>
          </Box>
        </Paper>

        {/* Main Content */}
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 3 }}>
          {/* Basic Information */}
          <Card elevation={3}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Basic Information
              </Typography>
              <Divider sx={{ mb: 2 }} />
              <Box sx={{ display: 'grid', gap: 2 }}>
                <Box display="flex" justifyContent="space-between">
                  <Typography color="text.secondary">Name:</Typography>
                  <Typography fontWeight="medium">{planet.name}</Typography>
                </Box>
                <Box display="flex" justifyContent="space-between">
                  <Typography color="text.secondary">Type:</Typography>
                  <Chip label={planet.type} size="small" color="primary" />
                </Box>
                <Box display="flex" justifyContent="space-between">
                  <Typography color="text.secondary">Mass:</Typography>
                  <Typography fontWeight="medium">{planet.mass} Earth masses</Typography>
                </Box>
                <Box display="flex" justifyContent="space-between">
                  <Typography color="text.secondary">Radius:</Typography>
                  <Typography fontWeight="medium">{planet.radius} Earth radii</Typography>
                </Box>
                <Box display="flex" justifyContent="space-between">
                  <Typography color="text.secondary">Distance from Sun:</Typography>
                  <Typography fontWeight="medium">{planet.distanceFromSun} AU</Typography>
                </Box>
                <Box display="flex" justifyContent="space-between">
                  <Typography color="text.secondary">Number of Moons:</Typography>
                  <Typography fontWeight="medium">{planet.numberOfMoons}</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>

          {/* Atmospheric Conditions */}
          <Card elevation={3}>
            <CardContent>
              <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
                <AtmosphereIcon sx={{ mr: 1 }} />
                Atmospheric Conditions
              </Typography>
              <Divider sx={{ mb: 2 }} />
              <Box sx={{ display: 'grid', gap: 2 }}>
                <Box display="flex" justifyContent="space-between" alignItems="center">
                  <Typography color="text.secondary">Has Atmosphere:</Typography>
                  <Chip 
                    label={planet.hasAtmosphere ? 'Yes' : 'No'} 
                    size="small" 
                    color={planet.hasAtmosphere ? 'success' : 'default'} 
                  />
                </Box>
                <Box>
                  <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ mb: 1 }}>
                    <Typography color="text.secondary">Oxygen Volume:</Typography>
                    <Typography fontWeight="medium">{planet.oxygenVolume ?? 0}%</Typography>
                  </Box>
                  <LinearProgress 
                    variant="determinate" 
                    value={planet.oxygenVolume ?? 0} 
                    sx={{ height: 8, borderRadius: 4 }}
                    color="info"
                  />
                </Box>
                <Box>
                  <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ mb: 1 }}>
                    <Typography color="text.secondary">Water Volume:</Typography>
                    <Typography fontWeight="medium">{planet.waterVolume ?? 0}%</Typography>
                  </Box>
                  <LinearProgress 
                    variant="determinate" 
                    value={planet.waterVolume ?? 0} 
                    sx={{ height: 8, borderRadius: 4 }}
                    color="primary"
                  />
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Box>

        {/* Environmental Factors */}
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 3, mt: 3 }}>
          <Card elevation={3}>
            <CardContent>
              <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
                <TerrainIcon sx={{ mr: 1 }} />
                Environmental Factors
              </Typography>
              <Divider sx={{ mb: 2 }} />
              <Box sx={{ display: 'grid', gap: 2 }}>
                <Box>
                  <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ mb: 1 }}>
                    <Typography color="text.secondary">Rock Hardness:</Typography>
                    <Typography fontWeight="medium">{planet.hardnessOfRock ?? 5}/10</Typography>
                  </Box>
                  <LinearProgress 
                    variant="determinate" 
                    value={((planet.hardnessOfRock ?? 5) / 10) * 100} 
                    sx={{ height: 8, borderRadius: 4 }}
                    color="warning"
                  />
                </Box>
                <Box>
                  <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ mb: 1 }}>
                    <Typography color="text.secondary">Threat Level:</Typography>
                    <Typography fontWeight="medium">{planet.threateningCreature ?? 1}/10</Typography>
                  </Box>
                  <LinearProgress 
                    variant="determinate" 
                    value={((planet.threateningCreature ?? 1) / 10) * 100} 
                    sx={{ height: 8, borderRadius: 4 }}
                    color="error"
                  />
                </Box>
              </Box>
            </CardContent>
          </Card>

          {/* Description */}
          <Card elevation={3}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Description
              </Typography>
              <Divider sx={{ mb: 2 }} />
              <Typography color="text.secondary" sx={{ lineHeight: 1.6 }}>
                {planet.description || 'No description available.'}
              </Typography>
            </CardContent>
          </Card>
        </Box>

        {/* Additional Information */}
        <Card elevation={3} sx={{ mt: 3 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Additional Information
            </Typography>
            <Divider sx={{ mb: 2 }} />
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr 1fr' }, gap: 2 }}>
              <Paper variant="outlined" sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Created
                </Typography>
                <Typography variant="body2" fontWeight="medium">
                  {planet.createdAt ? new Date(planet.createdAt).toLocaleDateString() : 'Unknown'}
                </Typography>
              </Paper>
              <Paper variant="outlined" sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Last Updated
                </Typography>
                <Typography variant="body2" fontWeight="medium">
                  {planet.updatedAt ? new Date(planet.updatedAt).toLocaleDateString() : 'Unknown'}
                </Typography>
              </Paper>
              <Paper variant="outlined" sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Planet ID
                </Typography>
                <Typography variant="body2" fontWeight="medium">
                  #{planet.id}
                </Typography>
              </Paper>
              <Paper variant="outlined" sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Habitability
                </Typography>
                <Chip 
                  label={
                    (planet.oxygenVolume ?? 0) > 15 && (planet.waterVolume ?? 0) > 30 && planet.hasAtmosphere
                      ? 'Potentially Habitable'
                      : 'Not Habitable'
                  }
                  size="small"
                  color={
                    (planet.oxygenVolume ?? 0) > 15 && (planet.waterVolume ?? 0) > 30 && planet.hasAtmosphere
                      ? 'success'
                      : 'error'
                  }
                />
              </Paper>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Box>
  );
};

export default PlanetDetailPage;
