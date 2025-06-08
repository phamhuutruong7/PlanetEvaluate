import axios from 'axios';
import { HabitabilityEvaluation } from '../types/habitability.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5219/api';

// Create axios instance with base configuration
const habitabilityApi = axios.create({
  baseURL: API_BASE_URL,
});

// Add request interceptor to include auth token
habitabilityApi.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    console.log('Habitability API Request:', {
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
    console.error('Habitability API Request Error:', error);
    return Promise.reject(error);
  }
);

// Add response interceptor to log responses and errors
habitabilityApi.interceptors.response.use(
  (response) => {
    console.log('Habitability API Response:', {
      status: response.status,
      statusText: response.statusText,
      dataType: typeof response.data,
      data: response.data
    });
    return response;
  },
  (error) => {
    console.error('Habitability API Response Error:', {
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

export const habitabilityService = {
  // Evaluate single planet habitability
  evaluatePlanet: async (planetId: number): Promise<HabitabilityEvaluation> => {
    try {
      const response = await habitabilityApi.get<HabitabilityEvaluation>(`/habitability/planet/${planetId}`);
      return response.data;
    } catch (error: any) {
      console.error('Error evaluating planet habitability:', error);
      throw error;
    }
  },
  // Rank all planets by habitability
  rankPlanets: async (): Promise<HabitabilityEvaluation[]> => {
    try {
      const response = await habitabilityApi.get<HabitabilityEvaluation[]>('/habitability/rank');
      return response.data;
    } catch (error: any) {
      console.error('Error ranking planets by habitability:', error);
      throw error;
    }
  },

  // Find most habitable planet
  findMostHabitable: async (): Promise<HabitabilityEvaluation> => {
    try {
      const response = await habitabilityApi.get<HabitabilityEvaluation>('/habitability/most-habitable');
      return response.data;
    } catch (error: any) {
      console.error('Error finding most habitable planet:', error);
      throw error;
    }
  },
};
