import React from 'react';
import {
  Card,
  CardContent,
  CardActions,
  Typography,
  Box,
  Chip,
  LinearProgress,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  Public as PlanetIcon,
  WaterDrop as WaterIcon,
  Air as AtmosphereIcon,
  Shield as RockIcon,
  Warning as ThreatIcon,
  Brightness7 as SunIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';
import { Planet } from '../../types/planet.types';
import { useAppSelector } from '../../utils/hooks';
import { canEditPlanet, canDeletePlanet } from '../../utils/permissions';

interface PlanetCardProps {
  planet: Planet;
  onEdit?: (planet: Planet) => void;
  onDelete?: (planet: Planet) => void;
}

const PlanetCard: React.FC<PlanetCardProps> = ({ 
  planet, 
  onEdit, 
  onDelete 
}) => {
  const { user } = useAppSelector((state) => state.auth);
  const canUserEditPlanet = canEditPlanet(user?.role);
  const canUserDeletePlanet = canDeletePlanet(user?.role);

  const getThreatColor = (threat?: number) => {
    if (!threat) return 'success';
    if (threat <= 3) return 'success';
    if (threat <= 6) return 'warning';
    return 'error';
  };

  const getRockHardnessColor = (hardness?: number) => {
    if (!hardness) return 'info';
    if (hardness <= 3) return 'success';
    if (hardness <= 7) return 'warning';
    return 'error';
  };

  const formatDistance = (distance?: number) => {
    if (!distance) return 'Unknown';
    return `${distance.toFixed(2)} AU`;
  };

  const formatMass = (mass?: number) => {
    if (!mass) return 'Unknown';
    return `${mass.toFixed(2)} Earth masses`;
  };

  const formatRadius = (radius?: number) => {
    if (!radius) return 'Unknown';
    return `${radius.toFixed(2)} Earth radii`;
  };

  return (
    <Card 
      sx={{ 
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        transition: 'transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: 4,
        },
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        color: 'white',
      }}
    >
      <CardContent sx={{ flexGrow: 1, pb: 1 }}>
        {/* Header */}
        <Box display="flex" alignItems="center" mb={2}>
          <PlanetIcon sx={{ mr: 1, fontSize: 28 }} />
          <Box flexGrow={1}>
            <Typography variant="h6" component="h2" fontWeight="bold">
              {planet.name}
            </Typography>
            {planet.type && (
              <Typography variant="body2" sx={{ opacity: 0.8 }}>
                {planet.type}
              </Typography>
            )}
          </Box>
          {planet.hasAtmosphere && (
            <Tooltip title="Has Atmosphere">
              <AtmosphereIcon sx={{ color: 'lightblue' }} />
            </Tooltip>
          )}
        </Box>

        {/* Description */}
        {planet.description && (
          <Typography variant="body2" sx={{ mb: 2, opacity: 0.9 }}>
            {planet.description}
          </Typography>
        )}

        {/* Basic Info */}
        <Box mb={2}>
          <Typography variant="body2" sx={{ mb: 1 }}>
            <SunIcon sx={{ fontSize: 16, mr: 0.5, verticalAlign: 'text-bottom' }} />
            Distance: {formatDistance(planet.distanceFromSun)}
          </Typography>
          <Typography variant="body2" sx={{ mb: 1 }}>
            Mass: {formatMass(planet.mass)}
          </Typography>
          <Typography variant="body2" sx={{ mb: 1 }}>
            Radius: {formatRadius(planet.radius)}
          </Typography>
          {planet.numberOfMoons !== undefined && (
            <Typography variant="body2">
              Moons: {planet.numberOfMoons}
            </Typography>
          )}
        </Box>

        {/* Environmental Indicators */}
        <Box mb={2}>
          {planet.waterVolume !== undefined && (
            <Box mb={1}>
              <Box display="flex" alignItems="center" mb={0.5}>
                <WaterIcon sx={{ fontSize: 16, mr: 0.5 }} />
                <Typography variant="body2">
                  Water Coverage: {planet.waterVolume}%
                </Typography>
              </Box>
              <LinearProgress 
                variant="determinate" 
                value={planet.waterVolume} 
                sx={{ 
                  height: 6, 
                  borderRadius: 3,
                  backgroundColor: 'rgba(255,255,255,0.3)',
                  '& .MuiLinearProgress-bar': {
                    backgroundColor: '#64b5f6',
                  }
                }}
              />
            </Box>
          )}

          {planet.oxygenVolume !== undefined && (
            <Box mb={1}>
              <Box display="flex" alignItems="center" mb={0.5}>
                <AtmosphereIcon sx={{ fontSize: 16, mr: 0.5 }} />
                <Typography variant="body2">
                  Oxygen: {planet.oxygenVolume}%
                </Typography>
              </Box>
              <LinearProgress 
                variant="determinate" 
                value={planet.oxygenVolume} 
                sx={{ 
                  height: 6, 
                  borderRadius: 3,
                  backgroundColor: 'rgba(255,255,255,0.3)',
                  '& .MuiLinearProgress-bar': {
                    backgroundColor: '#81c784',
                  }
                }}
              />
            </Box>
          )}
        </Box>

        {/* Threat and Rock Hardness */}
        <Box display="flex" gap={1} flexWrap="wrap">
          {planet.hardnessOfRock !== undefined && (
            <Chip
              icon={<RockIcon />}
              label={`Rock: ${planet.hardnessOfRock}/10`}
              size="small"
              color={getRockHardnessColor(planet.hardnessOfRock)}
              variant="outlined"
              sx={{ 
                backgroundColor: 'rgba(255,255,255,0.1)',
                color: 'white',
                borderColor: 'rgba(255,255,255,0.3)'
              }}
            />
          )}
          {planet.threateningCreature !== undefined && (
            <Chip
              icon={<ThreatIcon />}
              label={`Threat: ${planet.threateningCreature}/10`}
              size="small"
              color={getThreatColor(planet.threateningCreature)}
              variant="outlined"
              sx={{ 
                backgroundColor: 'rgba(255,255,255,0.1)',
                color: 'white',
                borderColor: 'rgba(255,255,255,0.3)'
              }}
            />
          )}
        </Box>
      </CardContent>      {/* Actions */}
      <CardActions sx={{ justifyContent: 'flex-end', pt: 0 }}>
        {onEdit && canUserEditPlanet && (
          <Tooltip title="Edit Planet">
            <IconButton 
              size="small" 
              onClick={() => onEdit(planet)}
              sx={{ color: 'white' }}
            >
              <EditIcon />
            </IconButton>
          </Tooltip>
        )}
        {onDelete && canUserDeletePlanet && (
          <Tooltip title="Delete Planet">
            <IconButton 
              size="small" 
              onClick={() => onDelete(planet)}
              sx={{ color: 'white' }}
            >
              <DeleteIcon />
            </IconButton>
          </Tooltip>
        )}
      </CardActions>
    </Card>
  );
};

export default PlanetCard;
