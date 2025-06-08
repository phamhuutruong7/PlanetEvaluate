# Planet Evaluation Client

A React TypeScript frontend application for the Planet Evaluation system.

## Features

- **Authentication**: Secure login with JWT tokens
- **Material UI**: Modern, responsive design with Material UI components
- **Redux State Management**: Centralized state management with Redux Toolkit
- **React Router**: Client-side routing with protected routes
- **TypeScript**: Full type safety throughout the application

## Technology Stack

- React 19.1.0
- TypeScript 4.9.5
- Material UI 7.1.1
- Redux Toolkit 2.8.2
- React Router DOM 7.6.2
- Axios for API calls

## Getting Started

### Prerequisites

- Node.js (version 16 or higher)
- npm or yarn
- Planet Evaluation API running on http://localhost:5137

### Installation

1. Install dependencies:
```bash
npm install
```

2. Set up environment variables:
```bash
# .env file is already configured for local development
REACT_APP_API_URL=http://localhost:5137/api
```

3. Start the development server:
```bash
npm start
```

The application will open at `http://localhost:3001`

### Testing Login

Use the following test credentials:
- **Username**: superadmin
- **Password**: password123

## Project Structure

```
src/
├── components/
│   ├── auth/
│   │   └── PrivateRoute.tsx      # Route protection component
│   └── common/
│       └── Layout.tsx            # Main application layout
├── pages/
│   ├── LoginPage.tsx             # Login page component
│   └── DashboardPage.tsx         # Dashboard page component
├── services/
│   └── auth.service.ts           # API service layer
├── store/
│   ├── index.ts                  # Redux store configuration
│   └── authSlice.ts              # Authentication state management
├── types/
│   └── auth.types.ts             # TypeScript type definitions
├── utils/
│   └── hooks.ts                  # Custom Redux hooks
├── App.tsx                       # Main application component
└── index.tsx                     # Application entry point
```

## Available Scripts

- `npm start` - Start development server
- `npm run build` - Build for production
- `npm test` - Run tests
- `npm run eject` - Eject from Create React App

## Features Implemented

✅ **Authentication System**
- Login page with form validation
- JWT token management
- Automatic token refresh handling
- Protected routes

✅ **UI Components**
- Responsive Material UI design
- Loading states and error handling
- User-friendly forms with validation
- Navigation layout with user menu

✅ **State Management**
- Redux store with auth slice
- Async thunks for API calls
- Proper error handling
- Automatic state persistence

## API Integration

The application integrates with the Planet Evaluation API:

- **POST** `/api/auth/login` - User authentication
- **GET** `/api/auth/profile` - Get user profile
- **POST** `/api/auth/logout` - User logout

## Future Enhancements

- Planet evaluation features
- User management (for admin users)
- Real-time updates
- Advanced search and filtering
- Data visualization
- Mobile app support
