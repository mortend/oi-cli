@echo off
setlocal
pushd "%~dp0"
set PATH="%PATH%;%ProgramFiles%\Microsoft Visual Studio\Installer"
set PATH="%PATH%;%ProgramFiles(x86)%\Microsoft Visual Studio\Installer"

for /f "usebackq tokens=1* delims=: " %%i in (`vswhere -latest -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

set MSBuild="%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe"

if not exist %MSBuild% (
  echo ERROR: Visual Studio 2017 was not found. >&2
  goto ERROR
)

%MSBuild% /m /p:Configuration=Release oi.sln || goto ERROR
copy /Y bin\Release\oi.exe . || goto ERROR

popd && exit /b 0

:ERROR
popd && pause && exit /b 1
