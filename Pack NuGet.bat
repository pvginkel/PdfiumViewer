@echo off

pushd "%~dp0"

cd PdfiumViewer
..\Libraries\NuGet\NuGet.exe pack -Prop "configuration=release;platform=anycpu"

pause

popd
