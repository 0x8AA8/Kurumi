@echo off
title nhitomi - Proxy Server
color 0C

echo ========================================
echo   nhitomi Proxy Server
echo ========================================
echo.
echo This runs the HTTP proxy server for
echo caching and serving images.
echo.

cd /d "%~dp0"

dotnet run --project nhitomi.Proxy

echo.
echo ----------------------------------------
echo Press any key to exit...
pause >nul
