import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { AuthState, LoginRequest, UserResponse, mapUserResponseToUser, mapLoginResponseToUser } from '../types/auth.types';
import { authService } from '../services/auth.service';

const initialState: AuthState = {
  user: null,
  token: localStorage.getItem('authToken'),
  isAuthenticated: false,
  isLoading: false,
  error: null,
};

// Async thunks
export const loginUser = createAsyncThunk(
  'auth/login',
  async (credentials: LoginRequest, { rejectWithValue }) => {
    try {
      console.log('=== LOGIN ATTEMPT ===');
      console.log('Credentials:', { username: credentials.username });
      
      const response = await authService.login(credentials);
      console.log('Login response received:', {
        hasToken: !!response.token,
        hasUser: !!response.username,
        tokenPreview: response.token ? response.token.substring(0, 50) + '...' : 'None',
        user: { id: response.id, username: response.username, email: response.email }
      });
      
      localStorage.setItem('authToken', response.token);
      console.log('Token stored in localStorage as "authToken"');
      
      // Extract user data (exclude token) for storage
      const { token, ...userData } = response;
      localStorage.setItem('user', JSON.stringify(userData));
      console.log('User data stored in localStorage:', userData);
      
      return response;
    } catch (error: any) {
      console.error('=== LOGIN ERROR ===', error);
      return rejectWithValue(
        error.response?.data?.message || 'Login failed'
      );
    }
  }
);

export const getCurrentUser = createAsyncThunk(
  'auth/getCurrentUser',
  async (_, { rejectWithValue }) => {
    try {
      const userResponse: UserResponse = await authService.getCurrentUser();
      return mapUserResponseToUser(userResponse);
    } catch (error: any) {
      return rejectWithValue(
        error.response?.data?.message || 'Failed to get user'
      );
    }
  }
);

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: (state) => {
      authService.logout();
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
      state.error = null;
    },
    clearError: (state) => {
      state.error = null;
    },    initializeAuth: (state) => {
      console.log('=== INITIALIZING AUTH ===');
      const token = localStorage.getItem('authToken');
      const userString = localStorage.getItem('user');
      
      console.log('Auth initialization:', {
        tokenExists: !!token,
        userStringExists: !!userString,
        tokenPreview: token ? token.substring(0, 50) + '...' : 'None'
      });
      
      if (token && userString) {
        try {
          const user = JSON.parse(userString);
          console.log('Parsed user from localStorage:', user);
          state.token = token;
          state.user = user;
          state.isAuthenticated = true;
          console.log('✅ Auth initialized successfully');
        } catch (error) {
          console.error('❌ Error parsing user from localStorage:', error);
          // Clear invalid stored data
          localStorage.removeItem('authToken');
          localStorage.removeItem('user');
        }
      } else {
        console.log('❌ Auth not initialized - missing token or user data');
      }
    },
  },
  extraReducers: (builder) => {
    builder
      // Login
      .addCase(loginUser.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })      .addCase(loginUser.fulfilled, (state, action) => {
        console.log('=== LOGIN FULFILLED ===');
        console.log('Action payload:', action.payload);
        
        state.isLoading = false;
        // Convert login response to User type and extract token
        const { token, ...loginResponseData } = action.payload;
        const userData = mapLoginResponseToUser({ ...loginResponseData, token } as any);
        state.user = userData;
        state.token = token;
        state.isAuthenticated = true;
        state.error = null;
        
        console.log('Auth state updated:', {
          isAuthenticated: state.isAuthenticated,
          hasUser: !!state.user,
          hasToken: !!state.token,
          user: state.user
        });
      })
      .addCase(loginUser.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
        state.isAuthenticated = false;
      })
      // Get current user
      .addCase(getCurrentUser.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(getCurrentUser.fulfilled, (state, action) => {
        state.isLoading = false;
        state.user = action.payload;
        state.isAuthenticated = true;
      })
      .addCase(getCurrentUser.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
        state.isAuthenticated = false;
      });
  },
});

export const { logout, clearError, initializeAuth } = authSlice.actions;
export default authSlice.reducer;
