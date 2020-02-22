:: Quickly run the tests.
:: Will crash if the packages were not restored before.
::
:: Examples:
:: > qtest --filter Category=XXXX
:: > qtest --filter Priority!=XXX -v q

@echo off
@setlocal

@call dotnet test .\play\ %* -c Release --no-restore

@endlocal
@exit /b %ERRORLEVEL%
