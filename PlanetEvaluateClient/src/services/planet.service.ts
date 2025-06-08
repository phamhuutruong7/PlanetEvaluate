import axios from 'axios';
import { Planet } from '../types/planet.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5137/api';

// Create axios instance with base configuration
const planetApi = axios.create({
  baseURL: API_BASE_URL,
});

// Add request interceptor to include auth token
planetApi.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken'); // Changed from 'token' to 'authToken'
    console.log('Planet API Request:', {
      url: config.url,
      method: config.method,
      baseURL: config.baseURL,
      hasToken: !!token,
      tokenPreview: token ? `${token.substring(0, 20)}...` : 'None'
    });
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    console.error('Planet API Request Error:', error);
    return Promise.reject(error);
  }
);

// Add response interceptor to log responses and errors
planetApi.interceptors.response.use(
  (response) => {
    console.log('Planet API Response:', {
      status: response.status,
      statusText: response.statusText,
      dataType: typeof response.data,
      dataLength: Array.isArray(response.data) ? response.data.length : 'N/A',
      data: response.data
    });
    return response;
  },
  (error) => {
    console.error('Planet API Response Error:', {
      status: error.response?.status,
      statusText: error.response?.statusText,
      data: error.response?.data,
      message: error.message,
      config: {
        url: error.config?.url,
        method: error.config?.method,
        headers: error.config?.headers
      }
    });
    return Promise.reject(error);
  }
);

export const planetService = {  // Get all planets
  getAllPlanets: async (): Promise<Planet[]> => {
    try {
      const response = await planetApi.get<Planet[]>('/planets');
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },

  // Get planet by ID
  getPlanetById: async (id: number): Promise<Planet> => {
    const response = await planetApi.get<Planet>(`/planets/${id}`);
    return response.data;
  },

  // Create new planet
  createPlanet: async (planetData: Omit<Planet, 'id' | 'createdAt' | 'updatedAt'>): Promise<Planet> => {
    const response = await planetApi.post<Planet>('/planets', planetData);
    return response.data;
  },

  // Update planet
  updatePlanet: async (id: number, planetData: Partial<Planet>): Promise<Planet> => {
    const response = await planetApi.put<Planet>(`/planets/${id}`, planetData);
    return response.data;
  },

  // Delete planet
  deletePlanet: async (id: number): Promise<void> => {
    await planetApi.delete(`/planets/${id}`);
  },
};
