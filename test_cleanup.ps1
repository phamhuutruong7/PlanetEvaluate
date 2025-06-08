# API Cleanup Test Script
# Tests the remaining endpoints after cleanup

$baseUrl = "http://localhost:5219/api"

Write-Host "Testing cleaned Planet Evaluate API endpoints..." -ForegroundColor Green

# Test 1: Login
Write-Host "`n1. Testing Login endpoint..." -ForegroundColor Yellow
$loginBody = @{
    username = "superadmin"
    password = "password123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "   ✓ Login successful" -ForegroundColor Green
    Write-Host "   Token: $($token.Substring(0, 50))..." -ForegroundColor Gray
} catch {
    Write-Host "   ✗ Login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Get current user
Write-Host "`n2. Testing Get Current User endpoint..." -ForegroundColor Yellow
$headers = @{ Authorization = "Bearer $token" }

try {
    $userResponse = Invoke-RestMethod -Uri "$baseUrl/auth/me" -Method GET -Headers $headers
    Write-Host "   ✓ Get current user successful" -ForegroundColor Green
    Write-Host "   User: $($userResponse.username) ($($userResponse.role))" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ Get current user failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get all users
Write-Host "`n3. Testing Get All Users endpoint..." -ForegroundColor Yellow
try {
    $usersResponse = Invoke-RestMethod -Uri "$baseUrl/auth/users" -Method GET -Headers $headers
    Write-Host "   ✓ Get all users successful" -ForegroundColor Green
    Write-Host "   Users count: $($usersResponse.Count)" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ Get all users failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Get all planets
Write-Host "`n4. Testing Get All Planets endpoint..." -ForegroundColor Yellow
try {
    $planetsResponse = Invoke-RestMethod -Uri "$baseUrl/planets" -Method GET -Headers $headers
    Write-Host "   ✓ Get all planets successful" -ForegroundColor Green
    Write-Host "   Planets count: $($planetsResponse.Count)" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ Get all planets failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Test habitability endpoints
Write-Host "`n5. Testing Habitability endpoints..." -ForegroundColor Yellow
try {
    # Get most habitable planet
    $habitabilityResponse = Invoke-RestMethod -Uri "$baseUrl/habitability/most-habitable" -Method GET -Headers $headers
    Write-Host "   ✓ Most habitable planet endpoint successful" -ForegroundColor Green
    Write-Host "   Most habitable planet: $($habitabilityResponse.planetName) (Score: $($habitabilityResponse.overallScore))" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ Habitability endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Verify removed endpoints return 404
Write-Host "`n6. Testing removed endpoints (should fail)..." -ForegroundColor Yellow

# Test removed UpdateUser endpoint
try {
    $updateBody = @{
        id = 1
        username = "test"
        email = "test@example.com"
        firstName = "Test"
        lastName = "User"
    } | ConvertTo-Json
    
    Invoke-RestMethod -Uri "$baseUrl/auth/users" -Method PUT -Body $updateBody -ContentType "application/json" -Headers $headers
    Write-Host "   ✗ UpdateUser endpoint still exists (should be removed)" -ForegroundColor Red
} catch {
    Write-Host "   ✓ UpdateUser endpoint correctly removed (404/405 expected)" -ForegroundColor Green
}

# Test removed WeatherForecast endpoint
try {
    Invoke-RestMethod -Uri "http://localhost:5219/weatherforecast" -Method GET
    Write-Host "   ✗ WeatherForecast endpoint still exists (should be removed)" -ForegroundColor Red
} catch {
    Write-Host "   ✓ WeatherForecast endpoint correctly removed (404 expected)" -ForegroundColor Green
}

Write-Host "`nAPI cleanup testing completed!" -ForegroundColor Green
