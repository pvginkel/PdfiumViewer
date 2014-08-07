@echo off

del bin\release\pdfium.dll

..\Libraries\NuGet\nuget.exe pack -prop configuration=release
