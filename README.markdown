# PdfiumViewer

Apache 2.0 License.

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
is fully compatible with both. However, do not that the standard DLL's won't
work on Window XP. To build for Windows XP, you need:

* The DLL's from the directory `Libraries\Pdfium\Without V8`;
* Also, you need to distribute a recent version of `dbghelp.dll`. These are
  also located in the `Libraries\Pdfium\Without V8` directory. Note that
  these are not part of the standard distribution and won't be loaded
  automatically.

## Using the library

To use the library, you must first add a reference to the NuGet package.

After you've added this reference, two files will be added to your project:

* `x86\pdfium.dll` is the 32-bit version of the Pdfium library;

* `x64\pdfium.dll` is the 64-bit version of the Pdfium library.

You have two options. If your application is 32-bit only or 64-bit only, you can
remove the DLL that won't be used. You can leave this file in the `x86` or `x64`
directory, or move it to the root of your project. PdfiumViewer will find the DLL
in both cases.

When building your project, the `pdfium.dll` library(s) must be placed next to
your application, either in the root or the `x86` or `x64` sub directory.
The easiest way to accomplish this is by changing the properties of that file,
changing the Copy to Output Directory setting to Copy always.

Note that in the directory `Libraries\Pdfium\Without V8`, there are versions
of the Pdfium DLL's without V8 support. These are a lot smaller and work on
Windows XP. See Compatibility above for instructions on how to use these.

## Note on the `PdfViewer` control

The PdfiumViewer library primarily consists out of three components:

* The `PdfViewer` control. This control provides a host for the `PdfRenderer`
  control and has a default toolbar with limited functionality;
* The `PdfRenderer` control. This control implements the raw PDF renderer.
  This control displays a PDF document, provides zooming and scrolling
  functionality and exposes methods to perform more advanced actions;
* The `PdfDocument` class provides access to the PDF document and wraps
  the Pdfium library.

The `PdfViewer` control should only be used when you have a very simple use
case and where the buttons available on the toolbar provide enough functionality
for you. This toolbar will not be extended with new buttons or with functionality
to hide buttons. The reason for this is that the `PdfViewer` control is just
meant to get you started. If you need more advanced functionality, you should
create your own control with your own toolbar, e.g. by starting out with
the `PdfViewer` control. Also, because people currently are already using the
`PdfViewer` control, adding more functionality to this toolbar would be
a breaking change. See [issue #41](https://github.com/pvginkel/PdfiumViewer/issues/41)
for more information.

## Building PDFium

Instructions to build the PDFium library can be found on the [Building PDFium](https://github.com/pvginkel/PdfiumViewer/wiki/Building-PDFium)
wiki page. However, if you are just looking to use the PdfiumViewer component
or looking for a compiled version of PDFium, these steps are not required.
A compiled version of the PDFium library is included in the NuGet package
and precompiled libraries can be found at [https://github.com/pvginkel/PdfiumViewer/tree/master/Libraries/Pdfium](https://github.com/pvginkel/PdfiumViewer/tree/master/Libraries/Pdfium).

## Bugs

Bugs should be reported through github at
[http://github.com/pvginkel/PdfiumViewer/issues](http://github.com/pvginkel/PdfiumViewer/issues).

## License

PdfiumViewer is licensed under the Apache 2.0 license. See the license details for how PDFium is licensed.
