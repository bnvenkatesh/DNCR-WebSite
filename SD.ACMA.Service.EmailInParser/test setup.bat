
sc delete "ACMA EmailInParser.LOCAL"
sc create "ACMA EmailInParser.LOCAL" binPath= "%CD%\bin\Debug\SD.ACMA.Service.EmailInParser.exe"
sc description "ACMA EmailInParser.LOCAL" "EmailInQueuePusher service for LOCAL testing only"
REM DONT FORGET TO UPDATE THE USER RUNNING THE SERVICE TO THE CORRECT ACCOUNT (e.g. for local testing, probably SALMAT\[youruser]
PAUSE