del /Q *.exe
rem C#
csc /out:t1.exe aes-test.cs /r:aes-cs.dll
rem C
csc /out:t2.exe aes-test.cs /r:aes.dll