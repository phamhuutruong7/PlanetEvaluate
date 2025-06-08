// Habitability-related type definitions

export enum HabitabilityLevel {
  Uninhabitable = 0,
  Poor = 1,
  Fair = 2,
  Good = 3,
  Excellent = 4,
  Ideal = 5
}

export interface HabitabilityFactorScores {
  oxygenScore: number;
  waterScore: number;
  atmosphereScore: number;
  distanceScore: number;
  safetyScore: number;
  terrainScore: number;
}

export interface HabitabilityEvaluation {
  planetId: number;
  planetName: string;
  overallHabitabilityScore: number;
  factorScores: HabitabilityFactorScores;
  habitabilityLevel: HabitabilityLevel;
  positiveFactors: string[];
  negativeFactors: string[];
  summary: string;
  evaluatedAt: string;
}

export interface HabitabilityState {
  evaluations: HabitabilityEvaluation[];
  currentEvaluation: HabitabilityEvaluation | null;
  rankings: HabitabilityEvaluation[];
  mostHabitable: HabitabilityEvaluation | null;
  isLoading: boolean;
  error: string | null;
}
