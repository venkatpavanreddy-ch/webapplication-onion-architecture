cd "%~dp0"
%~dp0\bin\Debug\net6.0\Build.exe --environment=uat --target=deploy --verbosity=verbose
pause
