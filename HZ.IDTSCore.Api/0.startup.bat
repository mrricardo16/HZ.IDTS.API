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
||||||| .r981
@echo off
title hz_dts
start "hz_dts" "C:\Windows\System32\cmd.exe"
dotnet HZ.IDTSCore.Api.dll urls=http://192.168.8.159:8888
taskkill /f /im cmd.exe
exit=======
@echo off
title hz_dts
start "hz_dts" "C:\Windows\System32\cmd.exe"
dotnet HZ.IDTSCore.Api.dll urls=http://192.168.12.168:8888
taskkill /f /im cmd.exe
exit>>>>>>> .r992
