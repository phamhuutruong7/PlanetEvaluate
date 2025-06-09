# Final word
At the end of this, I think that would be better to write and run this in docker
it would be easier for user to use that. But I guess it would be fine.

# Planet Evaluation System

A full-stack application for evaluating planet habitability with a React TypeScript frontend and ASP.NET Core backend API.

## 🚀 Quick Start Guide

### Prerequisites

Before running the application, ensure you have the following installed:

- **Node.js** (version 16 or higher) - [Download here](https://nodejs.org/)
- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB, Express, or full version) - [Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Git** - [Download here](https://git-scm.com/downloads)

### 🎯 Complete Setup Instructions

Follow these steps in order to run the complete Planet Evaluation system:

## Step 1: Clone and Setup Repository

```bash
# Clone the repository
git clone <repository-url>
cd PlanetEvaluate
```

## Step 2: Backend API Setup (ASP.NET Core)

### 1. Navigate to API Directory
```bash
cd PlanetEvaluateApi
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Database Setup
```bash
# Update database with migrations (creates database and tables)
dotnet ef database update

# Note: The database seeder will automatically run on first startup
# It creates 4 test users and 8 planets
```

### 4. Start the Backend API
```bash
# Run with HTTPS (recommended)
dotnet run --launch-profile "https"

# The API will be available at:
# - HTTPS: https://localhost:7062
# - HTTP: http://localhost:5219
# - Swagger UI: https://localhost:7062/swagger
```

**Keep this terminal open - the API must be running for the frontend to work.**

## Step 3: Frontend Client Setup (React)

### 1. Open New Terminal and Navigate to Client Directory
```bash
# Open a NEW terminal window/tab
cd PlanetEvaluateClient
```

### 2. Install Dependencies
```bash
npm install
```

### 3. Start the Frontend Application
```bash
npm start

# The React app will open at: http://localhost:3001
```

## Step 4: Access the Application

1. **Frontend**: Open [http://localhost:3001](http://localhost:3001)
2. **Backend API Documentation**: Open [https://localhost:7062/swagger](https://localhost:7062/swagger)

### 🔐 Test Credentials

Use these pre-seeded accounts to test the system:

| Username | Password | Role | Access |
|----------|----------|------|--------|
| superadmin | password123 | SuperAdmin | All planets |
| planetadmin | password123 | PlanetAdmin | All planets |
| viewer1 | password123 | Viewer1 | Planet 1 only |
| viewer2 | password123 | Viewer2 | Planets 1 & 3 |

## 🏗️ Technology Stack

### Backend (PlanetEvaluateApi)
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **JWT Authentication** - Secure token-based auth
- **Swagger/OpenAPI** - API documentation
- **BCrypt** - Password hashing

### Frontend (PlanetEvaluateClient)
- **React 19.1.0** - UI framework
- **TypeScript 4.9.5** - Type safety
- **Material UI 7.1.1** - UI components
- **Redux Toolkit 2.8.2** - State management
- **React Router DOM 7.6.2** - Routing
- **Axios** - HTTP client

## 📁 Project Structure

```
PlanetEvaluate/
├── PlanetEvaluateApi/          # Backend ASP.NET Core API
│   ├── Controllers/            # API endpoints
│   ├── Services/              # Business logic
│   ├── Models/                # Data models
│   ├── DTOs/                  # Data transfer objects
│   ├── Data/                  # Database context & seeder
│   └── ...
└── PlanetEvaluateClient/      # Frontend React app
    ├── src/
    │   ├── components/        # React components
    │   ├── pages/            # Page components
    │   ├── services/         # API services
    │   ├── store/            # Redux store
    │   └── types/            # TypeScript types
    └── ...
```

## 🛠️ Development Commands

### Backend Commands
```bash
cd PlanetEvaluateApi

# Run in development mode
dotnet run

# Run with specific profile
dotnet run --launch-profile "https"

# Build the project
dotnet build

# Run tests
dotnet test

# Create new migration
dotnet ef migrations add <MigrationName>

# Update database
dotnet ef database update

# Reset database (development only)
dotnet ef database drop
dotnet ef database update
```

### Frontend Commands
```bash
cd PlanetEvaluateClient

# Start development server
npm start

# Build for production
npm run build

# Run tests
npm test

# Install new package
npm install <package-name>
```

## 🚨 Troubleshooting

### Common Issues

1. **Port conflicts**: If ports are in use, modify `launchSettings.json` (backend) or use different ports
2. **Database connection**: Ensure SQL Server is running and connection string is correct
3. **CORS errors**: The API is configured to allow all origins in development
4. **SSL certificate**: Use `--launch-profile "http"` if HTTPS certificate issues occur

### Verifying Setup

1. **Backend**: Visit `https://localhost:7062/swagger` - you should see the API documentation
2. **Frontend**: Visit `http://localhost:3001` - you should see the login page
3. **Database**: Login with test credentials should work successfully

## 🎯 Features

### Implemented Features
✅ **Authentication System** - JWT-based login/logout  
✅ **Role-based Authorization** - Different access levels  
✅ **Planet Management** - CRUD operations for planets  
✅ **Habitability Evaluation** - Algorithm to assess planet habitability  
✅ **Responsive UI** - Material UI components  
✅ **API Documentation** - Swagger/OpenAPI integration  
✅ **Database Seeding** - Automatic test data creation  

### Future Enhancements
- Advanced planet search and filtering
- Real-time habitability monitoring
- Data visualization and charts
- User management interface (for admin users)
- Export functionality (PDF, CSV)
- Mobile app support
- Notification system
- Multi-language support

## 📚 API Endpoints

### Authentication
- `POST /api/auth/login` - User authentication
- `GET /api/auth/profile` - Get user profile  
- `POST /api/auth/logout` - User logout

### Planets
- `GET /api/planets` - Get all accessible planets
- `GET /api/planets/{id}` - Get planet by ID
- `POST /api/planets` - Create new planet (Admin only)
- `PUT /api/planets/{id}` - Update planet (Admin only)
- `DELETE /api/planets/{id}` - Delete planet (Admin only)

### Habitability
- `POST /api/habitability/evaluate/{planetId}` - Evaluate planet habitability
- `GET /api/habitability/rankings` - Get planet habitability rankings

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Ready to explore the universe? Start your journey with the Planet Evaluation System!** 🌌🚀
