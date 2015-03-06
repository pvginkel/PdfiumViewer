# PdfiumViewer

LGPL License.

[Download from NuGet](http://nuget.org/packages/PdfiumViewer).

## Introduction

PdfiumViewer is a PDF viewer based on the PDFium project.

PdfiumViewer provides a number of components to work with PDF files:

* PdfDocument is the base class used to render PDF documents;

* PdfRenderer is a WinForms control that can render a PdfDocument;

* PdfiumViewer is a WinForms control that hosts a PdfRenderer control and
  adds a toolbar to save the PDF file or print it.

## Compatibility

The PdfiumViewer library has been tested with Windows XP and Windows 8, and
is fully compatible with both. The PDFium library included in this project
is a 32-bit version, so your .NET application need to be 32-bit too. It works
without problems on 64-bit Windows though.

## Using the library

To use the library, you must first add a reference to the NuGet package.

After you've added this reference, two files will be added to your project:

* `x86\pdfium.dll` is the 32-bit version of the Pdfium library;

* `x64\pdfium.dll` is the 64-bit version of the Pdfium library.

You have two options. If your application is 32-bit only or 64-bit only, you can
remove the DLL that won't be used. You can leave this file in the `x86` or `x64`
directory, or move it to the root of your project. PdfiumViewer will find the DLL
in both cases.

When building your project, the `pdfiumdll` library(s) must be placed next to
your application, either in the root or the `x86` or `x64` sub directory.
The easiest way to accomplish this is by changing the properties of that file,
changing the Copy to Output Directory setting to Copy always.

## Building PDFium

To build PDFium for Windows, you need Visual Studio.

The PDFium source code does not have project files for Visual Studio. The full instructions
to get to a project you can open in Visual Studio, you need to follow the steps at
https://code.google.com/p/pdfium/wiki/Build.

One addition to the instructions on the website is that the `build\gyp_pdfium` command is a
Python script. To execute this, execute `python build\gyp_pdfium`. Also, one of the projects
require the Python executable to be in the `PATH`. `depot_tools` has a Python which can be
used for this.

After this, you can open the `all.sln` solution from the `build` directory. The project
is configured to create static libraries. However, we need a dynamic library for PdfiumViewer.
To have it generate a dynamic library, the following changes must be made to the configuration
of the `pdfium` project:

* Before opening the project properties, first set the Configuration to Release;

* In the General tab, Configuration Type must be set to Dynamic Library (.dll);

* In the C/C++ | General tab, the Additional Include Directories must get the extra directory
  `v8`. The full value should become `third_party\freetype\include;v8;v8\include;%(AdditionalIncludeDirectories)`;

* In the C/C++ | Preprocessor tab, the Preprocessor Definition `FPDFSDK_EXPORTS` must be
  added to have the DLL export the correct symbols;

* In the Linker | Input tab, the Additional Dependencies must be set to. This dialog appears
  after you've saved and re-opened the project configuration:

```
kernel32.lib
user32.lib
advapi32.lib
gdi32.lib
winmm.lib
$(OutDir)\lib\fdrm.lib
$(OutDir)\lib\fpdfdoc.lib
$(OutDir)\lib\fpdfapi.lib
$(OutDir)\lib\fpdftext.lib
$(OutDir)\lib\formfiller.lib
$(OutDir)\lib\fxcodec.lib
$(OutDir)\lib\fxcrt.lib
$(OutDir)\lib\fxedit.lib
$(OutDir)\lib\fxge.lib
$(OutDir)\lib\javascript.lib
$(OutDir)\lib\v8_base.lib
$(OutDir)\lib\v8_libbase.lib
$(OutDir)\lib\v8_libplatform.lib
$(OutDir)\lib\icui18n.lib
$(OutDir)\lib\icuuc.lib
$(OutDir)\lib\v8_snapshot.lib
$(OutDir)\lib\jsapi.lib
$(OutDir)\lib\pdfwindow.lib
$(OutDir)\lib\freetype.lib
```

* The platform toolset must be set to support Windows XP. The easiest way to do this is by
  performing a search and replace on all .vcxproj files in the PDFium source directory,
  replacing `<PlatformToolset>v120</PlatformToolset>` with `<PlatformToolset>v120_xp</PlatformToolset>`.
  I believe you only need to do this for Visual Studio 2013, but I don't have access to a
  previous version of Visual Studio, so I cannot verify this;

* A few methods need to be added to allow Pdfium to correctly initialize Pdfium. This file can
  be found in the `Contrib` directory and must be copied to the `pdfium\fpdfsdk\src` directory.
  After the file has been copied, it must be added to the project by right clicking on the `src`
  directory in the `pdfium` project and then choosing Add | Existing item. Select the copied source file
  and add it to the project.

You may have to run the build a few times because the dependencies don't appear to be resolved
correctly. For me, the second time the build completes successfully.

After you've made these changes and compiled the solution, you will get a valid DLL
which can be copied into the "Libraries/Pdfium" directory of the project.

To build the 64-bit version of the Pdfium library, drop down the `Release` option in the toolbar
and open the `Configuration Manager`. There, change the `Active solution platform` to `x64`.
The steps to update the project configuration explained above need to be performed again because
they are dependent on the platform.

## Bugs

Bugs should be reported through github at
[http://github.com/pvginkel/PdfiumViewer/issues](http://github.com/pvginkel/PdfiumViewer/issues).

## License

PdfiumViewer is licensed under the LGPL 3. See the license details for how PDFium is licensed.
