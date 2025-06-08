import { UserRole } from '../types/auth.types';

export const canCreatePlanet = (userRole: UserRole | undefined): boolean => {
  return userRole === UserRole.SuperAdmin;
};

export const canEditPlanet = (userRole: UserRole | undefined): boolean => {
  return userRole === UserRole.SuperAdmin || userRole === UserRole.PlanetAdmin;
};

export const canDeletePlanet = (userRole: UserRole | undefined): boolean => {
  return userRole === UserRole.SuperAdmin;
};

export const isAdmin = (userRole: UserRole | undefined): boolean => {
  return userRole === UserRole.SuperAdmin;
};

export const canViewAllPlanets = (userRole: UserRole | undefined): boolean => {
  return userRole === UserRole.SuperAdmin || userRole === UserRole.PlanetAdmin;
};

export const isViewer = (userRole: UserRole | undefined): boolean => {
  return userRole === UserRole.ViewerType1 || userRole === UserRole.ViewerType2;
};
