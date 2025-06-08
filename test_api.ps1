# Simple API test script
$baseUrl = "http://localhost:5219/api"

# Test 1: Login with superadmin
Write-Host "Testing login with superadmin..." -ForegroundColor Yellow
try {
    $loginBody = @{
        Username = "superadmin"
        Password = "password123"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -ContentType "application/json" -Body $loginBody
    Write-Host "✅ Login successful!" -ForegroundColor Green
    Write-Host "User: $($response.username), Role: $($response.role)" -ForegroundColor Green
    
    # Test 2: Get planets with the token
    Write-Host "`nTesting planet access..." -ForegroundColor Yellow
    $headers = @{
        "Authorization" = "Bearer $($response.token)"
        "Content-Type" = "application/json"
    }
    
    $planets = Invoke-RestMethod -Uri "$baseUrl/planets" -Method GET -Headers $headers
    Write-Host "✅ Planet access successful! Found $($planets.Count) planets" -ForegroundColor Green
    
    foreach ($planet in $planets) {
        Write-Host "  - Planet $($planet.id): $($planet.name)" -ForegroundColor Cyan
    }
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}
