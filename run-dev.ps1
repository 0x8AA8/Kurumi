# nhitomi Discord Bot - Development Mode (PowerShell)
# Run with: powershell -ExecutionPolicy Bypass -File run-dev.ps1

$Host.UI.RawUI.WindowTitle = "nhitomi Bot - Development Mode"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  nhitomi Discord Bot - Dev Mode" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Set environment variables for development
$env:DOTNET_ENVIRONMENT = "Development"
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Enable detailed logging
$env:Logging__LogLevel__Default = "Debug"
$env:Logging__LogLevel__Microsoft = "Warning"
$env:Logging__LogLevel__Microsoft__EntityFrameworkCore = "Information"
$env:Logging__LogLevel__Discord = "Debug"
$env:Logging__LogLevel__nhitomi = "Debug"

# Enable sensitive data logging for EF Core
$env:Microsoft__EntityFrameworkCore__EnableSensitiveDataLogging = "true"

Write-Host "Environment: $env:DOTNET_ENVIRONMENT" -ForegroundColor Yellow
Write-Host "Log Level: Debug" -ForegroundColor Yellow
Write-Host ""

# Create logs directory if it doesn't exist
$logsDir = Join-Path $PSScriptRoot "logs"
if (-not (Test-Path $logsDir)) {
    New-Item -ItemType Directory -Path $logsDir | Out-Null
}

# Log file with timestamp
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logFile = Join-Path $logsDir "bot_$timestamp.log"

Write-Host "Log file: $logFile" -ForegroundColor Gray
Write-Host ""
Write-Host "Starting bot..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host ""

# Change to project directory
Set-Location $PSScriptRoot

# Run the bot and tee output to log file
try {
    dotnet run --project nhitomi 2>&1 | Tee-Object -FilePath $logFile
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host "Bot has stopped. Press any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
