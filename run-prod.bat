@echo off
title nhitomi Bot - Production Mode
color 0B

echo ========================================
echo   nhitomi Discord Bot - Production
echo ========================================
echo.

:: Set environment variables for production
set DOTNET_ENVIRONMENT=Production
set ASPNETCORE_ENVIRONMENT=Production

:: Production logging (less verbose)
set Logging__LogLevel__Default=Information
set Logging__LogLevel__Microsoft=Warning
set Logging__LogLevel__Microsoft.EntityFrameworkCore=Warning
set Logging__LogLevel__Discord=Warning

echo Environment: %DOTNET_ENVIRONMENT%
echo Log Level: Information
echo.
echo Starting bot...
echo ----------------------------------------
echo.

:: Change to project directory
cd /d "%~dp0"

:: Run the bot (built in Release mode)
dotnet run --project nhitomi -c Release

:: If the bot exits, pause to see any error messages
echo.
echo ----------------------------------------
echo Bot has stopped. Press any key to exit...
pause >nul
