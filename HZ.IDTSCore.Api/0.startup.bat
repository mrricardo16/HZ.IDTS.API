@echo off
title hz_dts
start "hz_dts" "C:\Windows\System32\cmd.exe"
dotnet HZ.IDTSCore.Api.dll urls=http://192.168.8.159:8888
taskkill /f /im cmd.exe
exit