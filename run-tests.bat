@echo off
title nhitomi - Run Unit Tests
color 0E

echo ========================================
echo   nhitomi Unit Tests
echo ========================================
echo.

cd /d "%~dp0"

echo Running nhitomi.Core.UnitTests...
echo ----------------------------------------
dotnet test nhitomi.Core.UnitTests --verbosity normal

echo.
echo ----------------------------------------
echo Tests completed. Press any key to exit...
pause >nul
