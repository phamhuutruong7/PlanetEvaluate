import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '../../utils/hooks';
import { UserRole } from '../../types/auth.types';

interface RoleGuardProps {
  children: React.ReactNode;
  allowedRoles: UserRole[];
  redirectTo?: string;
}

const RoleGuard: React.FC<RoleGuardProps> = ({ 
  children, 
  allowedRoles, 
  redirectTo = '/dashboard' 
}) => {
  const { user, isAuthenticated } = useAppSelector((state) => state.auth);
  const location = useLocation();

  // If not authenticated, let PrivateRoute handle the redirect to login
  if (!isAuthenticated || !user) {
    return null;
  }

  // Check if user's role is in the allowed roles
  const hasPermission = allowedRoles.includes(user.role);

  if (!hasPermission) {
    // Redirect to dashboard with state to show a message
    return <Navigate 
      to={redirectTo} 
      replace 
      state={{ 
        from: location.pathname,
        message: `Access denied. You don't have permission to access this page.`
      }} 
    />;
  }

  return <>{children}</>;
};

export default RoleGuard;
