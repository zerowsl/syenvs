@echo off & setlocal enabledelayedexpansion & title syenvs
cd /d %~dp0

fltmc>nul
if errorlevel 1 ( 
	echo please right click the menu and select "run as administrator"
	pause & exit 1
)

set dotnet_cmd=dotnet
set i=0
:check_dotnet
%dotnet_cmd% >nul 2>nul
if errorlevel 1 (
	set /a i=i+1
	if !i!==1 set dotnet_cmd="dotnet\dotnet.exe" & goto check_dotnet
	if !i!==2 set dotnet_cmd="%ProgramFiles%\dotnet\dotnet.exe" & goto check_dotnet
	if !i!==3 set dotnet_cmd="%ProgramFiles(x86)%\dotnet\dotnet.exe" & goto check_dotnet
	goto error
) else (
	goto run
)

:error
echo 'dotnet' not find
pause & exit 1

:run
%dotnet_cmd% "SysEnvVars.dll" %*