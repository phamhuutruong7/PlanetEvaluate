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
  role: string; // API returns role as string, not enum
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

// API Response types
export interface UserResponse {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string; // API returns role as string
  assignedPlanetIds: number[];
  createdAt: string;
  lastLogin?: string;
  token?: string;
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

// Utility function to convert API response to frontend User type
export const mapUserResponseToUser = (userResponse: UserResponse): User => {
  // Convert string role to UserRole enum
  const mapRoleFromApi = (roleString: string): UserRole => {
    switch (roleString) {
      case 'SuperAdmin':
        return UserRole.SuperAdmin;
      case 'PlanetAdmin':
        return UserRole.PlanetAdmin;
      case 'ViewerType1':
        return UserRole.ViewerType1;
      case 'ViewerType2':
        return UserRole.ViewerType2;
      case 'Viewer':
        return UserRole.Viewer;
      default:
        console.warn(`Unknown role from API: ${roleString}, defaulting to Viewer`);
        return UserRole.Viewer;
    }
  };

  return {
    id: userResponse.id,
    username: userResponse.username,
    email: userResponse.email,
    firstName: userResponse.firstName,
    lastName: userResponse.lastName,
    role: mapRoleFromApi(userResponse.role),
    assignedPlanetIds: userResponse.assignedPlanetIds,
    createdAt: userResponse.createdAt,    lastLogin: userResponse.lastLogin
  };
};

// Utility function to convert API login response to frontend User type
export const mapLoginResponseToUser = (loginResponse: LoginResponse): User => {
  // Convert string role to UserRole enum
  const mapRoleFromApi = (roleString: string): UserRole => {
    switch (roleString) {
      case 'SuperAdmin':
        return UserRole.SuperAdmin;
      case 'PlanetAdmin':
        return UserRole.PlanetAdmin;
      case 'ViewerType1':
        return UserRole.ViewerType1;
      case 'ViewerType2':
        return UserRole.ViewerType2;
      case 'Viewer':
        return UserRole.Viewer;
      default:
        console.warn(`Unknown role from API: ${roleString}, defaulting to Viewer`);
        return UserRole.Viewer;
    }
  };

  return {
    id: loginResponse.id,
    username: loginResponse.username,
    email: loginResponse.email,
    firstName: loginResponse.firstName,
    lastName: loginResponse.lastName,
    role: mapRoleFromApi(loginResponse.role),
    assignedPlanetIds: loginResponse.assignedPlanetIds,
    assignedPlanetId: loginResponse.assignedPlanetId,
    assignedPlanetName: loginResponse.assignedPlanetName,
    createdAt: loginResponse.createdAt,
    lastLogin: loginResponse.lastLogin
  };
};
