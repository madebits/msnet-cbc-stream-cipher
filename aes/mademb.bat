REM MONO
REM mcs /t:exe -out:ma.exe mbaes.cs cs\AesFactory.cs cs\Aes.cs ..\sc\*.cs

REM .NET
csc /out:mbaes.exe mbaes.cs cs\AesFactory.cs cs\Aes.cs ..\sc\*.cs