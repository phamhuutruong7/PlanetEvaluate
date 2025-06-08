export interface LoginRequest {
  username: string;
  password: string;
}

export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  assignedPlanetId?: number;
  assignedPlanetName?: string;
  createdAt: string;
  lastLogin?: string;
}

export interface LoginResponse {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  assignedPlanetId?: number;
  assignedPlanetName?: string;
  createdAt: string;
  lastLogin?: string;
  token: string;
}

export enum UserRole {
  SuperAdmin = 'SuperAdmin',
  PlanetAdmin = 'PlanetAdmin',
  ViewerType1 = 'ViewerType1',
  ViewerType2 = 'ViewerType2'
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}
