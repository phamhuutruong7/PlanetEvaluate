import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../utils/hooks';
import { createPlanet } from '../store/planetsSlice';
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
} from '@mui/material';
import {
  Public as PlanetIcon,
  ArrowBack as BackIcon,
  Save as SaveIcon,
} from '@mui/icons-material';
import { Planet } from '../types/planet.types';

const CreatePlanetPage: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

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

  // Handle form field changes
  const handleInputChange = (field: string, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));

    // Clear error for this field when user starts typing
    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: ''
      }));
    }
  };

  // Handle form submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);
    setSubmitError(null);

    try {
      // Create planet data without id, createdAt, updatedAt
      const planetData: Omit<Planet, 'id' | 'createdAt' | 'updatedAt'> = {
        name: formData.name.trim(),
        type: formData.type || undefined,
        mass: formData.mass,
        radius: formData.radius,
        distanceFromSun: formData.distanceFromSun,
        numberOfMoons: formData.numberOfMoons,
        hasAtmosphere: formData.hasAtmosphere,
        oxygenVolume: formData.oxygenVolume,
        waterVolume: formData.waterVolume,
        hardnessOfRock: formData.hardnessOfRock,
        threateningCreature: formData.threateningCreature,
        description: formData.description.trim() || undefined,
      };

      await dispatch(createPlanet(planetData)).unwrap();
      
      // Navigate back to dashboard on success
      navigate('/dashboard');
    } catch (error: any) {
      setSubmitError(error || 'Failed to create planet');
    } finally {
      setIsSubmitting(false);
    }
  };

  // Handle cancel
  const handleCancel = () => {
    navigate('/dashboard');
  };

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50', py: 4 }}>
      <Box sx={{ maxWidth: 'lg', mx: 'auto', px: 3 }}>
        {/* Header */}
        <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
          <Box display="flex" alignItems="center" justifyContent="space-between">
            <Box display="flex" alignItems="center">
              <PlanetIcon sx={{ fontSize: 32, mr: 2, color: 'primary.main' }} />
              <Typography variant="h4" component="h1" fontWeight="bold">
                Create New Planet
              </Typography>
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
            <form onSubmit={handleSubmit}>              {/* Error Alert */}
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
                  </Typography>
                  <Divider sx={{ mb: 3 }} />
                </Box>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 3 }}>
                  <TextField
                    fullWidth
                    label="Planet Name"
                    value={formData.name}
                    onChange={(e) => handleInputChange('name', e.target.value)}
                    error={!!errors.name}
                    helperText={errors.name}
                    required
                    disabled={isSubmitting}
                  />

                  <FormControl fullWidth>
                    <InputLabel>Planet Type</InputLabel>
                    <Select
                      value={formData.type}
                      label="Planet Type"
                      onChange={(e) => handleInputChange('type', e.target.value)}
                      disabled={isSubmitting}
                    >
                      <MenuItem value="">
                        <em>Select a type</em>
                      </MenuItem>
                      {planetTypes.map((type) => (
                        <MenuItem key={type} value={type}>
                          {type}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Box>

                <Box>
                  <TextField
                    fullWidth
                    label="Description"
                    value={formData.description}
                    onChange={(e) => handleInputChange('description', e.target.value)}
                    multiline
                    rows={3}
                    disabled={isSubmitting}
                    placeholder="Enter a description of the planet..."
                  />
                </Box>                {/* Physical Properties */}
                <Box>
                  <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                    Physical Properties
                  </Typography>
                  <Divider sx={{ mb: 3 }} />
                </Box>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(3, 1fr)' }, gap: 3 }}>
                  <TextField
                    fullWidth
                    label="Mass (Earth masses)"
                    type="number"
                    value={formData.mass}
                    onChange={(e) => handleInputChange('mass', parseFloat(e.target.value) || 0)}
                    error={!!errors.mass}
                    helperText={errors.mass || 'Relative to Earth (1.0 = Earth mass)'}
                    inputProps={{ min: 0, step: 0.1 }}
                    disabled={isSubmitting}
                  />

                  <TextField
                    fullWidth
                    label="Radius (Earth radii)"
                    type="number"
                    value={formData.radius}
                    onChange={(e) => handleInputChange('radius', parseFloat(e.target.value) || 0)}
                    error={!!errors.radius}
                    helperText={errors.radius || 'Relative to Earth (1.0 = Earth radius)'}
                    inputProps={{ min: 0, step: 0.1 }}
                    disabled={isSubmitting}
                  />

                  <TextField
                    fullWidth
                    label="Distance from Sun (AU)"
                    type="number"
                    value={formData.distanceFromSun}
                    onChange={(e) => handleInputChange('distanceFromSun', parseFloat(e.target.value) || 0)}
                    error={!!errors.distanceFromSun}
                    helperText={errors.distanceFromSun || 'Astronomical Units (1.0 = Earth distance)'}
                    inputProps={{ min: 0, step: 0.1 }}
                    disabled={isSubmitting}
                  />
                </Box>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 3 }}>
                  <TextField
                    fullWidth
                    label="Number of Moons"
                    type="number"
                    value={formData.numberOfMoons}
                    onChange={(e) => handleInputChange('numberOfMoons', parseInt(e.target.value) || 0)}
                    error={!!errors.numberOfMoons}
                    helperText={errors.numberOfMoons}
                    inputProps={{ min: 0, step: 1 }}
                    disabled={isSubmitting}
                  />

                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.hasAtmosphere}
                        onChange={(e) => handleInputChange('hasAtmosphere', e.target.checked)}
                        disabled={isSubmitting}
                      />
                    }
                    label="Has Atmosphere"
                  />
                </Box>                {/* Environmental Properties */}
                <Box>
                  <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                    Environmental Properties
                  </Typography>
                  <Divider sx={{ mb: 3 }} />
                </Box>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 3 }}>
                  <Box>
                    <Typography gutterBottom>Oxygen Volume (%)</Typography>
                    <Box sx={{ px: 2 }}>
                      <Slider
                        value={formData.oxygenVolume}
                        onChange={(e, value) => handleInputChange('oxygenVolume', value as number)}
                        min={0}
                        max={100}
                        step={1}
                        marks={[
                          { value: 0, label: '0%' },
                          { value: 21, label: '21% (Earth)' },
                          { value: 50, label: '50%' },
                          { value: 100, label: '100%' }
                        ]}
                        valueLabelDisplay="on"
                        disabled={isSubmitting}
                      />
                    </Box>
                    {errors.oxygenVolume && (
                      <Typography variant="caption" color="error">
                        {errors.oxygenVolume}
                      </Typography>
                    )}
                  </Box>

                  <Box>
                    <Typography gutterBottom>Water Coverage (%)</Typography>
                    <Box sx={{ px: 2 }}>
                      <Slider
                        value={formData.waterVolume}
                        onChange={(e, value) => handleInputChange('waterVolume', value as number)}
                        min={0}
                        max={100}
                        step={1}
                        marks={[
                          { value: 0, label: '0%' },
                          { value: 71, label: '71% (Earth)' },
                          { value: 100, label: '100%' }
                        ]}
                        valueLabelDisplay="on"
                        disabled={isSubmitting}
                      />
                    </Box>
                    {errors.waterVolume && (
                      <Typography variant="caption" color="error">
                        {errors.waterVolume}
                      </Typography>
                    )}
                  </Box>
                </Box>                {/* Danger & Terrain Properties */}
                <Box>
                  <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                    Danger & Terrain Assessment
                  </Typography>
                  <Divider sx={{ mb: 3 }} />
                </Box>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 3 }}>
                  <Box>
                    <Typography gutterBottom>Rock Hardness (1-10)</Typography>
                    <Box sx={{ px: 2 }}>
                      <Slider
                        value={formData.hardnessOfRock}
                        onChange={(e, value) => handleInputChange('hardnessOfRock', value as number)}
                        min={1}
                        max={10}
                        step={1}
                        marks={[
                          { value: 1, label: 'Soft' },
                          { value: 5, label: 'Medium' },
                          { value: 10, label: 'Very Hard' }
                        ]}
                        valueLabelDisplay="on"
                        disabled={isSubmitting}
                      />
                    </Box>
                    <Typography variant="caption" color="text.secondary">
                      1 = Very soft rocks, 10 = Extremely hard rocks
                    </Typography>
                  </Box>

                  <Box>
                    <Typography gutterBottom>Threatening Creatures (1-10)</Typography>
                    <Box sx={{ px: 2 }}>
                      <Slider
                        value={formData.threateningCreature}
                        onChange={(e, value) => handleInputChange('threateningCreature', value as number)}
                        min={1}
                        max={10}
                        step={1}
                        marks={[
                          { value: 1, label: 'Safe' },
                          { value: 5, label: 'Moderate' },
                          { value: 10, label: 'Very Dangerous' }
                        ]}
                        valueLabelDisplay="on"
                        disabled={isSubmitting}
                      />
                    </Box>
                    <Typography variant="caption" color="text.secondary">
                      1 = No threat, 10 = Extremely dangerous creatures
                    </Typography>
                  </Box>
                </Box>                {/* Preview */}
                <Box>
                  <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                    Preview
                  </Typography>
                  <Divider sx={{ mb: 3 }} />
                  <Box display="flex" gap={1} flexWrap="wrap">
                    <Chip label={`Mass: ${formData.mass}x Earth`} variant="outlined" />
                    <Chip label={`Radius: ${formData.radius}x Earth`} variant="outlined" />
                    <Chip label={`Distance: ${formData.distanceFromSun} AU`} variant="outlined" />
                    <Chip label={`Moons: ${formData.numberOfMoons}`} variant="outlined" />
                    <Chip 
                      label={formData.hasAtmosphere ? 'Has Atmosphere' : 'No Atmosphere'} 
                      color={formData.hasAtmosphere ? 'success' : 'default'}
                      variant="outlined" 
                    />
                    <Chip label={`Oxygen: ${formData.oxygenVolume}%`} variant="outlined" />
                    <Chip label={`Water: ${formData.waterVolume}%`} variant="outlined" />
                    <Chip 
                      label={`Rock: ${formData.hardnessOfRock}/10`} 
                      color={formData.hardnessOfRock <= 3 ? 'success' : formData.hardnessOfRock <= 6 ? 'warning' : 'error'}
                      variant="outlined" 
                    />
                    <Chip 
                      label={`Threat: ${formData.threateningCreature}/10`} 
                      color={formData.threateningCreature <= 3 ? 'success' : formData.threateningCreature <= 6 ? 'warning' : 'error'}
                      variant="outlined" 
                    />
                  </Box>
                </Box>

                {/* Action Buttons */}
                <Box>
                  <Box display="flex" justifyContent="flex-end" gap={2} sx={{ mt: 3 }}>
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
                      startIcon={<SaveIcon />}
                      disabled={isSubmitting}
                      size="large"
                    >
                      {isSubmitting ? 'Creating Planet...' : 'Create Planet'}
                    </Button>
                  </Box>
                </Box>
              </Box>
            </form>
          </CardContent>
        </Card>
      </Box>
    </Box>
  );
};

export default CreatePlanetPage;
