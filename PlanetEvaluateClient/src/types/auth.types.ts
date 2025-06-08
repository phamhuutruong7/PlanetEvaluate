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
  assignedPlanetIds?: number[]; // Updated to match backend
  assignedPlanetId?: number; // Keep for backward compatibility
  assignedPlanetName?: string; // Keep for backward compatibility
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
  assignedPlanetIds?: number[]; // Updated to match backend
  assignedPlanetId?: number; // Keep for backward compatibility
  assignedPlanetName?: string; // Keep for backward compatibility
  createdAt: string;
  lastLogin?: string;
  token: string;
}

export enum UserRole {
  SuperAdmin = 'SuperAdmin',
  PlanetAdmin = 'PlanetAdmin',
  ViewerType1 = 'ViewerType1',
  ViewerType2 = 'ViewerType2',
  Viewer = 'Viewer' // Added for completeness
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

// Additional types for user management
export interface UpdateUserRequest {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password?: string;
}

export interface AdminUpdateUserRequest {
  id: number;
  role: string;
  assignedPlanetIds?: number[];
}

export interface UserManagementState {
  users: User[];
  selectedUser: User | null;
  isLoading: boolean;
  error: string | null;
}
