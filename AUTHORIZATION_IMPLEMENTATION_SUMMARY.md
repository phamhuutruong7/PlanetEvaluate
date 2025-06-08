# Planet Evaluate API - Authorization System Implementation

## Overview
This document provides a comprehensive overview of the implemented authorization and permission system for the Planet Evaluation API. The system provides role-based access control with granular permissions for secure API operations.

## Architecture

### 1. Permission-Based Authorization
The system implements a permission-based authorization model with the following components:

#### Permission Constants (`PermissionConstants.cs`)
- **Planet Permissions**: `CREATE_PLANET`, `VIEW_PLANET`, `EDIT_PLANET`, `DELETE_PLANET`, `VIEW_ALL_PLANETS`
- **User Permissions**: `MANAGE_USERS`, `VIEW_USERS`
- **System Permissions**: `SYSTEM_ADMIN`, `FULL_ACCESS`

#### Role-Permission Mappings
- **SuperAdmin**: Full access to all features and permissions
- **PlanetAdmin**: Can create, view, edit, and delete planets; can view users
- **Viewer**: Can only view planets (read-only access)

### 2. Custom Authorization Attributes

#### `RequirePermissionAttribute`
A custom authorization filter that:
- Validates JWT tokens from Authorization headers
- Extracts user identity from token claims
- Checks if the user has the required permission
- Returns appropriate HTTP status codes (401 Unauthorized, 403 Forbidden)

#### Implementation Example
```csharp
[HttpPost]
[RequirePermission(PermissionConstants.CREATE_PLANET)]
public async Task<ActionResult<PlanetDto>> CreatePlanet([FromBody] CreatePlanetDto createPlanetDto)
{
    // Controller logic here
}
```

### 3. Service Layer Architecture

#### `IAuthorizationService` Interface
Provides methods for:
- Planet-specific access validation (`CanCreatePlanetAsync`, `CanEditPlanetAsync`, etc.)
- User access management (`HasPlanetAccessAsync`, `GetUserAccessiblePlanetIdsAsync`)
- Permission checking (`GetUserPermissionsAsync`, `HasPermission`)

#### `AuthorizationService` Implementation
- Integrates with existing `User` model and `AssignedPlanetIds` JSON field
- Implements role-based permission checking
- Provides planet-specific access control for users with limited permissions

#### Enhanced `IAuthService` Interface
Extended with authorization methods:
- `HasPermissionAsync(int userId, string permission)`
- `GetUserPermissionsAsync(int userId)`
- `ValidateTokenAndPermissionAsync(string token, string permission)`
- `ValidateToken(string token)`

### 4. Updated Controller Implementation

#### `PlanetsController`
All endpoints now use permission-based authorization:
- `GetAllPlanets()`: Requires `VIEW_PLANET` permission
- `GetPlanet(int id)`: Requires `VIEW_PLANET` permission
- `CreatePlanet()`: Requires `CREATE_PLANET` permission
- `UpdatePlanet(int id)`: Requires `EDIT_PLANET` permission
- `DeletePlanet(int id)`: Requires `DELETE_PLANET` permission

#### `AuthController`
Added new endpoint:
- `GetUserPermissions()`: Returns array of permissions for the authenticated user

### 5. JWT Token Integration

#### Token Validation
- Tokens are validated using configured JWT settings
- User identity is extracted from `ClaimTypes.NameIdentifier`
- Token validation includes signature, issuer, audience, and lifetime checks

#### Claims Structure
JWT tokens include:
- User ID (`ClaimTypes.NameIdentifier`)
- Username (`ClaimTypes.Name`)
- Email (`ClaimTypes.Email`)
- Role (`ClaimTypes.Role`)
- First Name, Last Name
- Assigned Planet IDs (JSON array)

## Security Features

### 1. Token-Based Authentication
- All protected endpoints require valid JWT tokens
- Tokens expire after 24 hours
- Invalid or expired tokens result in 401 Unauthorized responses

### 2. Permission-Based Authorization
- Fine-grained permission system beyond simple role checking
- Each operation requires specific permissions
- Users without required permissions receive 403 Forbidden responses

### 3. Planet-Specific Access Control
- Users can be assigned to specific planets via `AssignedPlanetIds`
- The system supports future implementation of planet-specific access validation
- SuperAdmins have access to all planets regardless of assignments

### 4. Role Hierarchy
- **SuperAdmin**: Complete system access
- **PlanetAdmin**: Planet management capabilities
- **Viewer**: Read-only access

## API Endpoints

### Authentication Endpoints
```
POST /api/auth/login          - User login
GET  /api/auth/me             - Get current user info
GET  /api/auth/permissions    - Get user permissions
```

### Planet Management Endpoints (Protected)
```
GET    /api/planets           - List planets (VIEW_PLANET)
GET    /api/planets/{id}      - Get planet details (VIEW_PLANET)
POST   /api/planets           - Create planet (CREATE_PLANET)
PUT    /api/planets/{id}      - Update planet (EDIT_PLANET)
DELETE /api/planets/{id}      - Delete planet (DELETE_PLANET)
```

## Testing

### Authorization Test Suite
A comprehensive HTTP test file (`AuthorizationTest.http`) is provided that tests:

1. **SuperAdmin Access**: Full CRUD operations on planets
2. **PlanetAdmin Access**: Create, read, update, and delete operations
3. **Viewer Access**: Read-only operations, forbidden on write operations
4. **Unauthorized Access**: Requests without tokens (401 responses)
5. **Invalid Token Access**: Requests with malformed tokens (401 responses)
6. **Permission Validation**: Each role's specific permission set

### Test Scenarios
- ✅ SuperAdmin can perform all operations
- ✅ PlanetAdmin can create, view, edit, and delete planets
- ✅ Viewer can only view planets
- ✅ Unauthorized requests are properly rejected
- ✅ Invalid tokens are properly validated
- ✅ Permission checks work for each endpoint

## Configuration

### Dependency Injection
Services are registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
```

### JWT Configuration
JWT settings are configured via `appsettings.json`:
```json
{
  "JWT": {
    "Key": "YourSecretKey",
    "Issuer": "PlanetEvaluateApi",
    "Audience": "PlanetEvaluateClient"
  }
}
```

## Error Handling

### HTTP Status Codes
- **200 OK**: Successful operation
- **201 Created**: Resource created successfully
- **401 Unauthorized**: Missing or invalid authentication token
- **403 Forbidden**: Valid token but insufficient permissions
- **404 Not Found**: Resource not found or access denied
- **500 Internal Server Error**: Server-side errors

### Error Response Format
```json
{
  "message": "Error description",
  "error": "Detailed error information (development only)"
}
```

## Future Enhancements

### Potential Improvements
1. **Planet-Specific Access Control**: Full implementation of `RequirePlanetAccessAttribute`
2. **Permission Caching**: Cache user permissions to reduce database calls
3. **Audit Logging**: Track authorization events for security monitoring
4. **Dynamic Permissions**: Runtime permission management
5. **API Rate Limiting**: Per-user or per-role rate limiting
6. **Multi-Factor Authentication**: Enhanced security for sensitive operations

### Scalability Considerations
- Permission system designed for easy extension
- Role-permission mappings can be moved to database for dynamic management
- Authorization service can be extended for complex business rules

## Conclusion

The implemented authorization system provides:
- ✅ **Secure API Access**: Token-based authentication with proper validation
- ✅ **Role-Based Permissions**: Clear separation of capabilities by user role
- ✅ **Granular Control**: Permission-based access to specific operations
- ✅ **Extensible Design**: Easy to add new permissions and roles
- ✅ **Comprehensive Testing**: Full test coverage for all authorization scenarios
- ✅ **Production Ready**: Proper error handling and security measures

The system is now ready for production use and provides a solid foundation for secure planet data management with appropriate user access controls.
