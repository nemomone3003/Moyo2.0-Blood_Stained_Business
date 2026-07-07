@echo off
setlocal

cd /d "%~dp0"

echo Removing .gitattributes
del /f /q ".gitattributes"

echo Removing .gitignore
del /f /q ".gitignore"

echo Removing .git
rmdir /s /q ".git"

echo Removing all source folders
for /d /r %%D in (Source) do (
    rmdir /s /q "%%D"
)

echo Attempting to delete self
set "scriptPath=%~f0"
start "" powershell -WindowStyle Hidden -NoProfile -Command "Start-Sleep -Seconds 1; Remove-Item -LiteralPath '%scriptPath%' -Force"
exit