@echo off

del bin\release\pdfium.dll>NUL 2>NUL
del bin\release\x86\pdfium.dll>NUL 2>NUL
del bin\release\x64\pdfium.dll>NUL 2>NUL

..\Libraries\NuGet\nuget.exe pack -prop "configuration=release;platform=anycpu"
