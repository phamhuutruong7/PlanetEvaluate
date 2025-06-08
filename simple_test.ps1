Write-Host "Testing Planet Evaluate API..." -ForegroundColor Green

# Test login
$loginBody = '{"Username":"superadmin","Password":"password123"}'
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5219/api/auth/login" -Method POST -ContentType "application/json" -Body $loginBody
    Write-Host "✅ Login successful! User: $($response.username), Role: $($response.role)" -ForegroundColor Green
    
    # Test planets
    $headers = @{"Authorization" = "Bearer $($response.token)"}
    $planets = Invoke-RestMethod -Uri "http://localhost:5219/api/planets" -Method GET -Headers $headers
    Write-Host "✅ Found $($planets.Count) planets accessible to $($response.username)" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}
