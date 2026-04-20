@echo off
setlocal

echo ==========================================
echo  Visual Studio WinForms Designer cleanup
echo ==========================================
echo.
echo ВАЖНО: Закройте Visual Studio перед запуском.
echo.

REM 1) Удаляем папку .vs в текущем каталоге (если есть)
if exist ".vs" (
  echo [1/3] Removing .vs ...
  rmdir /s /q ".vs"
) else (
  echo [1/3] .vs not found - skip
)

REM 2) Удаляем все bin/obj рекурсивно
echo [2/3] Removing all bin/obj folders recursively...
for /d /r %%D in (bin obj) do (
  if exist "%%D" (
    echo     deleting: %%D
    rmdir /s /q "%%D"
  )
)

REM 3) Чистим ComponentModelCache у VS 18.*
echo [3/3] Removing Visual Studio ComponentModelCache (18.*) ...
for /d %%V in ("%LOCALAPPDATA%\Microsoft\VisualStudio\18.*") do (
  if exist "%%V\ComponentModelCache" (
    echo     deleting: %%V\ComponentModelCache
    rmdir /s /q "%%V\ComponentModelCache"
  )
)

echo.
echo Done. Теперь откройте решение и сделайте Rebuild.
pause
endlocal