import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { Planet, PlanetsState } from '../types/planet.types';
import { planetService } from '../services/planet.service';

// Initial state
const initialState: PlanetsState = {
  planets: [],
  isLoading: false,
  error: null,
};

// Async thunks
export const fetchPlanets = createAsyncThunk(
  'planets/fetchPlanets',
  async (_, { rejectWithValue }) => {
    try {
      const planets = await planetService.getAllPlanets();
      return planets;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch planets');
    }
  }
);

export const fetchPlanetById = createAsyncThunk(
  'planets/fetchPlanetById',
  async (id: number, { rejectWithValue }) => {
    try {
      const planet = await planetService.getPlanetById(id);
      return planet;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch planet');
    }
  }
);

export const createPlanet = createAsyncThunk(
  'planets/createPlanet',
  async (planetData: Omit<Planet, 'id' | 'createdAt' | 'updatedAt'>, { rejectWithValue }) => {
    try {
      const planet = await planetService.createPlanet(planetData);
      return planet;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create planet');
    }
  }
);

export const updatePlanet = createAsyncThunk(
  'planets/updatePlanet',
  async ({ id, data }: { id: number; data: Partial<Planet> }, { rejectWithValue }) => {
    try {
      const planet = await planetService.updatePlanet(id, data);
      return planet;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update planet');
    }
  }
);

export const deletePlanet = createAsyncThunk(
  'planets/deletePlanet',
  async (id: number, { rejectWithValue }) => {
    try {
      await planetService.deletePlanet(id);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to delete planet');
    }
  }
);

// Slice
const planetsSlice = createSlice({
  name: 'planets',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    clearPlanets: (state) => {
      state.planets = [];
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch all planets
      .addCase(fetchPlanets.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchPlanets.fulfilled, (state, action: PayloadAction<Planet[]>) => {
        state.isLoading = false;
        state.planets = action.payload;
      })
      .addCase(fetchPlanets.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      // Fetch planet by ID
      .addCase(fetchPlanetById.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchPlanetById.fulfilled, (state, action: PayloadAction<Planet>) => {
        state.isLoading = false;
        // Update the planet in the list if it exists, otherwise add it
        const existingIndex = state.planets.findIndex(p => p.id === action.payload.id);
        if (existingIndex >= 0) {
          state.planets[existingIndex] = action.payload;
        } else {
          state.planets.push(action.payload);
        }
      })
      .addCase(fetchPlanetById.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      // Create planet
      .addCase(createPlanet.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(createPlanet.fulfilled, (state, action: PayloadAction<Planet>) => {
        state.isLoading = false;
        state.planets.push(action.payload);
      })
      .addCase(createPlanet.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      // Update planet
      .addCase(updatePlanet.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(updatePlanet.fulfilled, (state, action: PayloadAction<Planet>) => {
        state.isLoading = false;
        const index = state.planets.findIndex(p => p.id === action.payload.id);
        if (index >= 0) {
          state.planets[index] = action.payload;
        }
      })
      .addCase(updatePlanet.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      // Delete planet
      .addCase(deletePlanet.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(deletePlanet.fulfilled, (state, action: PayloadAction<number>) => {
        state.isLoading = false;
        state.planets = state.planets.filter(p => p.id !== action.payload);
      })
      .addCase(deletePlanet.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, clearPlanets } = planetsSlice.actions;
export default planetsSlice.reducer;
