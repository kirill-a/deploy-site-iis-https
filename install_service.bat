pkgmgr /iu:WCF-HTTP-Activation >> log.txt
IF %ERRORLEVEL% NEQ 0 pause
%systemroot%\system32\inetsrv\appcmd add app /path:/"<SERVICE_NAME>" /physicalPath:"C:\inetpub\wwwroot\<SERVICE_NAME>" /site.name:"Default Web Site" >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
%systemroot%\system32\inetsrv\appcmd set config "Default Web Site/<SERVICE_NAME>" -commitPath:APPHOST -section:access -sslFlags:Ssl >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
%systemroot%\system32\inetsrv\appcmd set site /site.name:"Default Web Site" /+bindings.[protocol='https',bindingInformation='*:7443:'] >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
%systemroot%\system32\inetsrv\appcmd add apppool /name:<SERVICE_NAME> /managedRuntimeVersion:v2.0 /managedPipelineMode:Integrated >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
%systemroot%\system32\inetsrv\appcmd set app /app.name:"Default Web Site/<SERVICE_NAME>" /applicationPool:"<SERVICE_NAME>" >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
%systemroot%\system32\inetsrv\appcmd set config /section:applicationPools "/[name='<SERVICE_NAME>'].processModel.identityType:LocalSystem" >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
iisreset /noforce localhost >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
DeployCertUtility.exe >> log.txt
IF %ERRORLEVEL% NEQ 0 goto fail
echo "INSTALLATION SUCCESS" >> log.txt
exit 0
:fail
echo "INSTALLATION FAILURE" >> log.txt
exit 1