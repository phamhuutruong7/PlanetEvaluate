# Test script for Planet Evaluate API Authorization System
# This script tests the new service-based authorization for different user roles

$baseUrl = "http://localhost:5219"

# Function to make API requests
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Token = $null,
        [object]$Body = $null
    )
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    if ($Token) {
        $headers["Authorization"] = "Bearer $Token"
    }
    
    try {
        $response = if ($Body) {
            Invoke-RestMethod -Uri $Url -Method $Method -Headers $headers -Body ($Body | ConvertTo-Json)
        } else {
            Invoke-RestMethod -Uri $Url -Method $Method -Headers $headers
        }
        return @{ Success = $true; Data = $response }
    } catch {
        return @{ Success = $false; Error = $_.Exception.Message; Status = $_.Exception.Response.StatusCode }
    }
}

# Test login for different users
Write-Host "=== Testing Planet Evaluate API Authorization System ===" -ForegroundColor Green
Write-Host ""

# Test users based on the seeded data
$testUsers = @(
    @{ Username = "superadmin"; Password = "password123"; ExpectedRole = "SuperAdmin"; Description = "Full CRUD to all planets" }
    @{ Username = "planetadmin"; Password = "password123"; ExpectedRole = "PlanetAdmin"; Description = "Admin for specific planet" }
    @{ Username = "viewer1"; Password = "password123"; ExpectedRole = "viewer1"; Description = "Read-only access to planet 1 only" }
    @{ Username = "viewer2"; Password = "password123"; ExpectedRole = "viewer2"; Description = "Read-only access to planets 1 and 3" }
    @{ Username = "viewergeneric"; Password = "password123"; ExpectedRole = "viewer"; Description = "Read-only access to assigned planets" }
)

$tokens = @{}

# Login all users
foreach ($user in $testUsers) {
    Write-Host "Logging in user: $($user.Username) ($($user.Description))" -ForegroundColor Yellow
    
    $loginResult = Invoke-ApiRequest -Method "POST" -Url "$baseUrl/auth/login" -Body @{
        username = $user.Username
        password = $user.Password
    }
    
    if ($loginResult.Success) {
        $tokens[$user.Username] = $loginResult.Data.token
        Write-Host "✅ Login successful for $($user.Username)" -ForegroundColor Green
    } else {
        Write-Host "❌ Login failed for $($user.Username): $($loginResult.Error)" -ForegroundColor Red
    }
    Write-Host ""
}

# Test planet access for each user
Write-Host "=== Testing Planet Access ===" -ForegroundColor Green
Write-Host ""

foreach ($user in $testUsers) {
    if ($tokens.ContainsKey($user.Username)) {
        Write-Host "Testing access for $($user.Username) ($($user.Description)):" -ForegroundColor Yellow
        
        # Test GET all planets
        $planetsResult = Invoke-ApiRequest -Method "GET" -Url "$baseUrl/planets" -Token $tokens[$user.Username]
        if ($planetsResult.Success) {
            $planetCount = $planetsResult.Data.Count
            Write-Host "  ✅ GET /planets - Can access $planetCount planets" -ForegroundColor Green
        } else {
            Write-Host "  ❌ GET /planets - Access denied: $($planetsResult.Status)" -ForegroundColor Red
        }
        
        # Test GET specific planets (1, 2, 3)
        for ($planetId = 1; $planetId -le 3; $planetId++) {
            $planetResult = Invoke-ApiRequest -Method "GET" -Url "$baseUrl/planets/$planetId" -Token $tokens[$user.Username]
            if ($planetResult.Success) {
                Write-Host "  ✅ GET /planets/$planetId - Access granted" -ForegroundColor Green
            } else {
                Write-Host "  ❌ GET /planets/$planetId - Access denied: $($planetResult.Status)" -ForegroundColor Red
            }
        }
        
        # Test POST (create) - should only work for SuperAdmin and PlanetAdmin
        $createResult = Invoke-ApiRequest -Method "POST" -Url "$baseUrl/planets" -Token $tokens[$user.Username] -Body @{
            name = "Test Planet for $($user.Username)"
            description = "Test planet created by $($user.Username)"
            surfaceArea = 1000
            population = 5000
        }
        if ($createResult.Success) {
            Write-Host "  ✅ POST /planets - Create access granted" -ForegroundColor Green
        } else {
            Write-Host "  ❌ POST /planets - Create access denied: $($createResult.Status)" -ForegroundColor Red
        }
        
        Write-Host ""
    }
}

Write-Host "=== Authorization Test Complete ===" -ForegroundColor Green
