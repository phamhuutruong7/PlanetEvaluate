export interface Planet {
  id: number;
  name: string;
  type?: string;
  mass?: number; // In Earth masses
  radius?: number; // In Earth radii
  distanceFromSun?: number; // In AU
  numberOfMoons?: number;
  hasAtmosphere: boolean;
  oxygenVolume?: number; // Percentage of oxygen in atmosphere (0-100)
  waterVolume?: number; // Percentage of surface covered by water (0-100)
  hardnessOfRock?: number; // Scale 1-10 (1 is softest, 10 is hardest)
  threateningCreature?: number; // Scale 1-10 (1 is lowest threat, 10 is highest threat)
  createdAt: string;
  updatedAt?: string;
  description?: string;
}

export interface PlanetsState {
  planets: Planet[];
  isLoading: boolean;
  error: string | null;
}
