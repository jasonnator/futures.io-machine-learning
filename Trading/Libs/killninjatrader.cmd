@ECHO OFF
VERIFY>NUL
if ERRORLEVEL 1 GOTO NOCMDEXT
SETLOCAL EnableExtensions
IF ERRORLEVEL 1 EXIT /B 5
SETLOCAL EnableDelayedExpansion
IF ERRORLEVEL 1 EXIT /B 5

SET _imagename=NinjaTrader.exe
SET _process_check_cmd=TASKLIST /FI "IMAGENAME eq %_imagename%" /FO TABLE /NH 2^>NUL ^| FIND /I /N "%_imagename%" ^>NUL

%_process_check_cmd%
IF !ERRORLEVEL! NEQ 0 GOTO allClear

ECHO NinjaTrader is running, shutting it down...
TASKKILL /T /IM %_imagename%
IF !ERRORLEVEL! NEQ 0 GOTO killProcess

ECHO Waiting for NinjaTrader to shut down within 15s...
FOR /L %%G IN (1,1,15) DO (
    SLEEP 1
    %_process_check_cmd%
    IF !ERRORLEVEL! NEQ 0 GOTO terminationConfirmed)

:killProcess
ECHO NinjaTrader was unable to shut down in time, killing process...
TASKKILL /F /T /IM %_imagename%
IF !ERRORLEVEL! NEQ 0 GOTO setError
SLEEP 1
%_process_check_cmd%
IF !ERRORLEVEL! NEQ 0 GOTO terminationConfirmed

:setError
ECHO Unable to terminate NinjaTrader process!
EXIT /B 0

:allClear
ECHO NinjaTrader is not running
EXIT /B 0

:terminationConfirmed
EXIT /B 0

ENDLOCAL
ENDLOCAL
:NOCMDEXT
ECHO Command extensions not available