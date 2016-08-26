@echo off

REM *** Registers all ETW Event Sources in application

call :isAdmin
if %errorlevel% == 0 (
echo Running with admin rights.
) else (
echo Error: You must run this file "As Administrator." 1>&2
pause
exit /b 1
)

set basepath=%~dp0\bin\Debug

set filename=NServiceBus.EventSourceLogging.Samples.CustomEventLog.Net46.NServiceBus-Samples-CustomEventLogEventSourceLoggingNet46.etwManifest
set man=%basepath%\%filename%.man
set dll=%basepath%\%filename%.dll

echo Uninstalling NServiceBus-Samples-CustomEventLogEventSourceLoggingNet46
wevtutil.exe um "%man%"

echo Installing NServiceBus-Samples-CustomEventLogEventSourceLoggingNet46
wevtutil.exe im "%man%" /rf:"%dll%" /mf:"%dll%"
pause
exit /b

:isAdmin
fsutil dirty query %systemdrive% >nul
exit /b