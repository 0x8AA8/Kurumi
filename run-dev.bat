@echo off
title nhitomi Bot - Development Mode
color 0A

echo ========================================
echo   nhitomi Discord Bot - Dev Mode
echo ========================================
echo.

:: Set environment variables for development
set DOTNET_ENVIRONMENT=Development
set ASPNETCORE_ENVIRONMENT=Development

:: Enable detailed logging
set Logging__LogLevel__Default=Debug
set Logging__LogLevel__Microsoft=Warning
set Logging__LogLevel__Microsoft.EntityFrameworkCore=Information
set Logging__LogLevel__Discord=Debug
set Logging__LogLevel__nhitomi=Debug

:: Enable sensitive data logging for EF Core (shows parameter values in queries)
set Microsoft__EntityFrameworkCore__EnableSensitiveDataLogging=true

echo Environment: %DOTNET_ENVIRONMENT%
echo Log Level: Debug
echo.
echo Starting bot...
echo ----------------------------------------
echo.

:: Change to project directory
cd /d "%~dp0"

:: Run the bot
dotnet run --project nhitomi

:: If the bot exits, pause to see any error messages
echo.
echo ----------------------------------------
echo Bot has stopped. Press any key to exit...
pause >nul
