@echo off
title nhitomi - Client Tester
color 0D

echo ========================================
echo   nhitomi Client Tester
echo ========================================
echo.
echo This tool tests the nhentai and Hitomi
echo clients to verify they can fetch data.
echo.

cd /d "%~dp0"

dotnet run --project nhitomi.ClientTester

echo.
echo ----------------------------------------
echo Press any key to exit...
pause >nul
