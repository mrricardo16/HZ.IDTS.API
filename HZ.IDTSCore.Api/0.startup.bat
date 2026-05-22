@echo off
title hz_dts
setlocal
set "APP_DIR=%~dp0"
if not exist "%APP_DIR%HZ.IDTSCore.Api.dll" set "APP_DIR=%~dp0bin\Debug\netcoreapp3.1\"
pushd "%APP_DIR%"
dotnet HZ.IDTSCore.Api.dll --urls=http://0.0.0.0:64416
set "EXIT_CODE=%ERRORLEVEL%"
popd
endlocal & exit /b %EXIT_CODE%
