SET LogFile="log.txt"
IF EXIST %LogFile% del /F %LogFile%
call start /wait install_service.bat
type %LogFile%
pause