@echo off
title SY_idts
start "SY_idts" "C:\Windows\System32\cmd.exe"
dotnet HZ.IDTSCore.Api.dll --urls=http://0.0.0.0:64416
taskkill /f /im cmd.exe
exit