These are the compiled Pdfium libraries. The default libraries used,
the ones that are here in the x86 and x64 folders, are the ones that
include V8. These are not compatible with Windows XP.

If, alternatively, you want smaller Pdfium DLL's, or require Windows XP
support, you can alternatively use the Without V8 versions. For Windows XP,
you also need to distribute updated versions of the `dbghelp.dll` DLL's,
which are also located in the appropriate folders. Please note that
these are not part of the standard distribution and are not automatically
loaded or copied into your target folder. You have to add these yourself.