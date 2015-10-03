 
SET AESDLL=.\c\aes.dll
rem SET AESDLL=.\cs\aes-cs.dll

echo build %AESDLL% before

copy %AESDLL% .\*.*
csc /out:exa.exe aes-example.cs /r:%AESDLL% /debug+