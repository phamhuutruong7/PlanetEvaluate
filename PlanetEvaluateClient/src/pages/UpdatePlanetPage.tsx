import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../utils/hooks';
import { updatePlanet, fetchPlanetById } from '../store/planetsSlice';
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Switch,
  FormControlLabel,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Slider,
  Alert,
  Paper,
  Divider,
  Chip,
  CircularProgress,
} from '@mui/material';
import {
  ArrowBack as BackIcon,
  Save as SaveIcon,
  Edit as EditIcon,
} from '@mui/icons-material';

const UpdatePlanetPage: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { id } = useParams<{ id: string }>();
  const { planets } = useAppSelector((state) => state.planets);

  // Find the planet to edit
  const planetToEdit = planets.find(p => p.id === Number(id));

  // Form state
  const [formData, setFormData] = useState({
    name: '',
    type: '',
    mass: 1.0,
    radius: 1.0,
    distanceFromSun: 1.0,
    numberOfMoons: 0,
    hasAtmosphere: true,
    oxygenVolume: 0,
    waterVolume: 0,
    hardnessOfRock: 5,
    threateningCreature: 1,
    description: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isLoadingPlanet, setIsLoadingPlanet] = useState(false);

  // Planet types
  const planetTypes = [
    'Terrestrial',
    'Gas Giant',
    'Ice Giant',
    'Dwarf Planet',
    'Super Earth',
    'Ocean World',
    'Desert World',
    'Frozen World',
    'Volcanic World',
  ];

  // Load planet data on component mount
  useEffect(() => {
    const loadPlanetData = async () => {
      if (!id) {
        navigate('/dashboard');
        return;
      }

      const planetId = Number(id);
      let planet = planetToEdit;

      // If planet is not in the store, fetch it
      if (!planet) {
        setIsLoadingPlanet(true);
        try {
          const fetchedPlanet = await dispatch(fetchPlanetById(planetId)).unwrap();
          planet = fetchedPlanet;
        } catch (error) {
          setSubmitError('Failed to load planet data');
          setIsLoadingPlanet(false);
          return;
        }
        setIsLoadingPlanet(false);
      }

      // Populate form with planet data
      if (planet) {
        setFormData({
          name: planet.name || '',
          type: planet.type || '',
          mass: planet.mass || 1.0,
          radius: planet.radius || 1.0,
          distanceFromSun: planet.distanceFromSun || 1.0,
          numberOfMoons: planet.numberOfMoons || 0,
          hasAtmosphere: planet.hasAtmosphere ?? true,
          oxygenVolume: planet.oxygenVolume || 0,
          waterVolume: planet.waterVolume || 0,
          hardnessOfRock: planet.hardnessOfRock || 5,
          threateningCreature: planet.threateningCreature || 1,
          description: planet.description || '',
        });
      }
    };

    loadPlanetData();
  }, [id, planetToEdit, dispatch, navigate]);

  // Form validation
  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Planet name is required';
    }

    if (formData.mass <= 0) {
      newErrors.mass = 'Mass must be greater than 0';
    }

    if (formData.radius <= 0) {
      newErrors.radius = 'Radius must be greater than 0';
    }

    if (formData.distanceFromSun <= 0) {
      newErrors.distanceFromSun = 'Distance from sun must be greater than 0';
    }

    if (formData.numberOfMoons < 0) {
      newErrors.numberOfMoons = 'Number of moons cannot be negative';
    }

    if (formData.oxygenVolume < 0 || formData.oxygenVolume > 100) {
      newErrors.oxygenVolume = 'Oxygen volume must be between 0 and 100';
    }

    if (formData.waterVolume < 0 || formData.waterVolume > 100) {
      newErrors.waterVolume = 'Water volume must be between 0 and 100';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle form input changes
  const handleInputChange = (field: keyof typeof formData) => 
    (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | any) => {
      let value: any = event.target.value;
      
      // Convert numeric fields
      if (['mass', 'radius', 'distanceFromSun', 'oxygenVolume', 'waterVolume'].includes(field)) {
        value = parseFloat(value) || 0;
      } else if (['numberOfMoons', 'hardnessOfRock', 'threateningCreature'].includes(field)) {
        value = parseInt(value) || 0;
      } else if (field === 'hasAtmosphere') {
        value = event.target.checked;
      }

      setFormData(prev => ({ ...prev, [field]: value }));
      
      // Clear field error when user starts typing
      if (errors[field]) {
        setErrors(prev => ({ ...prev, [field]: '' }));
      }
    };

  // Handle form submission
  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    
    if (!validateForm() || !id) {
      return;
    }

    setIsSubmitting(true);
    setSubmitError(null);

    try {
      await dispatch(updatePlanet({ 
        id: Number(id), 
        data: formData 
      })).unwrap();
      navigate('/dashboard');
    } catch (error: any) {
      setSubmitError(error || 'Failed to update planet');
    } finally {
      setIsSubmitting(false);
    }
  };

  // Handle cancel
  const handleCancel = () => {
    navigate('/dashboard');
  };

  // Show loading spinner while fetching planet data
  if (isLoadingPlanet) {
    return (
      <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50', py: 4, display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
        <CircularProgress size={60} />
      </Box>
    );
  }

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50', py: 4 }}>
      <Box sx={{ maxWidth: 'lg', mx: 'auto', px: 3 }}>
        {/* Header */}
        <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
          <Box display="flex" alignItems="center" justifyContent="space-between">            <Box display="flex" alignItems="center">
              <EditIcon sx={{ fontSize: 32, mr: 2, color: 'primary.main' }} />
              <Box>
                <Typography variant="h4" component="h1" fontWeight="bold">
                  {formData.name || 'Loading...'}
                </Typography>
                <Typography variant="subtitle1" color="text.secondary">
                  Planet Details & Edit
                </Typography>
              </Box>
            </Box>
            <Button
              variant="outlined"
              startIcon={<BackIcon />}
              onClick={handleCancel}
              disabled={isSubmitting}
            >
              Back to Dashboard
            </Button>
          </Box>
        </Paper>

        {/* Form */}
        <Card elevation={3}>
          <CardContent sx={{ p: 4 }}>
            <form onSubmit={handleSubmit}>
              {/* Error Alert */}
              {submitError && (
                <Alert severity="error" sx={{ mb: 3 }}>
                  {submitError}
                </Alert>
              )}

              <Box sx={{ display: 'grid', gap: 3 }}>
                {/* Basic Information */}
                <Box>
                  <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
                    Basic Information
                    <Chip label="Required" size="small" color="primary" sx={{ ml: 2 }} />
                  </Typography>
                  <Divider sx={{ mb: 2 }} />
                  <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 2 }}>
                    <TextField
                      label="Planet Name"
                      value={formData.name}
                      onChange={handleInputChange('name')}
                      error={!!errors.name}
                      helperText={errors.name}
                      required
                      fullWidth
                    />
                    <FormControl fullWidth>
                      <InputLabel>Planet Type</InputLabel>
                      <Select
                        value={formData.type}
                        onChange={handleInputChange('type')}
                        label="Planet Type"
                      >
                        {planetTypes.map((type) => (
                          <MenuItem key={type} value={type}>
                            {type}
                          </MenuItem>
                        ))}
                      </Select>
                    </FormControl>
                  </Box>
                </Box>

                {/* Physical Properties */}
                <Box>
                  <Typography variant="h6" gutterBottom>
                    Physical Properties
                  </Typography>
                  <Divider sx={{ mb: 2 }} />
                  <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr 1fr' }, gap: 2 }}>
                    <TextField
                      label="Mass (Earth masses)"
                      type="number"
                      value={formData.mass}
                      onChange={handleInputChange('mass')}
                      error={!!errors.mass}
                      helperText={errors.mass}
                      inputProps={{ min: 0, step: 0.1 }}
                      required
                      fullWidth
                    />
                    <TextField
                      label="Radius (Earth radii)"
                      type="number"
                      value={formData.radius}
                      onChange={handleInputChange('radius')}
                      error={!!errors.radius}
                      helperText={errors.radius}
                      inputProps={{ min: 0, step: 0.1 }}
                      required
                      fullWidth
                    />
                    <TextField
                      label="Distance from Sun (AU)"
                      type="number"
                      value={formData.distanceFromSun}
                      onChange={handleInputChange('distanceFromSun')}
                      error={!!errors.distanceFromSun}
                      helperText={errors.distanceFromSun}
                      inputProps={{ min: 0, step: 0.1 }}
                      required
                      fullWidth
                    />
                  </Box>
                  <Box sx={{ mt: 2 }}>
                    <TextField
                      label="Number of Moons"
                      type="number"
                      value={formData.numberOfMoons}
                      onChange={handleInputChange('numberOfMoons')}
                      error={!!errors.numberOfMoons}
                      helperText={errors.numberOfMoons}
                      inputProps={{ min: 0 }}
                      fullWidth
                      sx={{ maxWidth: 300 }}
                    />
                  </Box>
                </Box>

                {/* Atmosphere & Composition */}
                <Box>
                  <Typography variant="h6" gutterBottom>
                    Atmosphere & Composition
                  </Typography>
                  <Divider sx={{ mb: 2 }} />
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.hasAtmosphere}
                        onChange={handleInputChange('hasAtmosphere')}
                      />
                    }
                    label="Has Atmosphere"
                    sx={{ mb: 2 }}
                  />
                  
                  <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 3 }}>
                    <Box>
                      <Typography gutterBottom>Oxygen Volume (%)</Typography>
                      <Slider
                        value={formData.oxygenVolume}
                        onChange={(_, value) => handleInputChange('oxygenVolume')({ target: { value } })}
                        min={0}
                        max={100}
                        valueLabelDisplay="auto"
                        marks={[
                          { value: 0, label: '0%' },
                          { value: 21, label: '21% (Earth)' },
                          { value: 50, label: '50%' },
                          { value: 100, label: '100%' },
                        ]}
                      />
                      {errors.oxygenVolume && (
                        <Typography color="error" variant="caption">
                          {errors.oxygenVolume}
                        </Typography>
                      )}
                    </Box>
                    
                    <Box>
                      <Typography gutterBottom>Water Volume (%)</Typography>
                      <Slider
                        value={formData.waterVolume}
                        onChange={(_, value) => handleInputChange('waterVolume')({ target: { value } })}
                        min={0}
                        max={100}
                        valueLabelDisplay="auto"
                        marks={[
                          { value: 0, label: '0%' },
                          { value: 71, label: '71% (Earth)' },
                          { value: 100, label: '100%' },
                        ]}
                      />
                      {errors.waterVolume && (
                        <Typography color="error" variant="caption">
                          {errors.waterVolume}
                        </Typography>
                      )}
                    </Box>
                  </Box>
                </Box>

                {/* Environmental Factors */}
                <Box>
                  <Typography variant="h6" gutterBottom>
                    Environmental Factors
                  </Typography>
                  <Divider sx={{ mb: 2 }} />
                  <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 3 }}>
                    <Box>
                      <Typography gutterBottom>Hardness of Rock (1-10)</Typography>
                      <Slider
                        value={formData.hardnessOfRock}
                        onChange={(_, value) => handleInputChange('hardnessOfRock')({ target: { value } })}
                        min={1}
                        max={10}
                        step={1}
                        valueLabelDisplay="auto"
                        marks={[
                          { value: 1, label: 'Soft' },
                          { value: 5, label: 'Medium' },
                          { value: 10, label: 'Very Hard' },
                        ]}
                      />
                    </Box>
                    
                    <Box>
                      <Typography gutterBottom>Threatening Creature Level (1-10)</Typography>
                      <Slider
                        value={formData.threateningCreature}
                        onChange={(_, value) => handleInputChange('threateningCreature')({ target: { value } })}
                        min={1}
                        max={10}
                        step={1}
                        valueLabelDisplay="auto"
                        marks={[
                          { value: 1, label: 'Peaceful' },
                          { value: 5, label: 'Moderate' },
                          { value: 10, label: 'Extremely Dangerous' },
                        ]}
                      />
                    </Box>
                  </Box>
                </Box>

                {/* Description */}
                <Box>
                  <Typography variant="h6" gutterBottom>
                    Description
                  </Typography>
                  <Divider sx={{ mb: 2 }} />
                  <TextField
                    label="Planet Description"
                    value={formData.description}
                    onChange={handleInputChange('description')}
                    multiline
                    rows={4}
                    fullWidth
                    placeholder="Describe the planet's unique characteristics, climate, notable features, etc."
                  />
                </Box>

                {/* Action Buttons */}
                <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', pt: 2 }}>
                  <Button
                    variant="outlined"
                    onClick={handleCancel}
                    disabled={isSubmitting}
                    size="large"
                  >
                    Cancel
                  </Button>
                  <Button
                    type="submit"
                    variant="contained"
                    startIcon={isSubmitting ? <CircularProgress size={20} /> : <SaveIcon />}
                    disabled={isSubmitting}
                    size="large"
                  >
                    {isSubmitting ? 'Updating...' : 'Update Planet'}
                  </Button>
                </Box>
              </Box>
            </form>
          </CardContent>
        </Card>
      </Box>
    </Box>
  );
};

export default UpdatePlanetPage;
