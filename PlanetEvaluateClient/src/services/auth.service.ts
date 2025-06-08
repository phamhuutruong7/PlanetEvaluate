import axios from 'axios';
import { LoginRequest, LoginResponse } from '../types/auth.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5219/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle auth errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Type definitions for new endpoints
export interface UserResponse {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  assignedPlanetIds: number[];
  createdAt: string;
  lastLogin?: string;
  token?: string;
}

export const authService = {
  // Authentication endpoints
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await apiClient.post('/auth/login', credentials);
    return response.data;
  },

  logout: async () => {
    try {
      await apiClient.post('/auth/logout');
    } catch (error) {
      console.warn('Logout request failed, but continuing with local cleanup');
    }
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
  },

  getCurrentUser: async (): Promise<UserResponse> => {
    const response = await apiClient.get('/auth/me');
    return response.data;
  },

  // User management endpoints
  getAllUsers: async (): Promise<UserResponse[]> => {
    const response = await apiClient.get('/auth/users');
    return response.data;
  },

  getUserById: async (id: number): Promise<UserResponse> => {
    const response = await apiClient.get(`/auth/users/${id}`);
    return response.data;
  },
  // Planet assignment endpoints
  assignPlanetToUser: async (userId: number, planetId: number, permissionLevel: string = 'Read'): Promise<void> => {
    await apiClient.post(`/auth/users/${userId}/assign-planet/${planetId}?permissionLevel=${encodeURIComponent(permissionLevel)}`);
  },

  removePlanetFromUser: async (userId: number, planetId: number): Promise<void> => {
    await apiClient.delete(`/auth/users/${userId}/remove-planet/${planetId}`);
  },
};

export default apiClient;
