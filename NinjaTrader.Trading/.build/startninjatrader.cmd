@ECHO OFF
VERIFY>NUL
if ERRORLEVEL 1 GOTO NOCMDEXT
SETLOCAL EnableExtensions
IF ERRORLEVEL 1 EXIT /B 5

ECHO Launching NinjaTrader...
START "" /D "C:\Program Files (x86)\NinjaTrader 8\bin64\" /B "StartDetached.exe" "C:\Program Files (x86)\NinjaTrader 8\bin64\NinjaTrader.exe"
EXIT /B 0

ENDLOCAL
:NOCMDEXT
ECHO Command extensions not available