import { configureStore } from '@reduxjs/toolkit';
import authReducer from './authSlice';
import planetsReducer from './planetsSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    planets: planetsReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
