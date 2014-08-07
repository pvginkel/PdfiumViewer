using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfiumViewer
{
    internal class PdfLibrary : IDisposable
    {
        private bool _disposed;
        private NativeMethods.UNSUPPORT_INFO _unsupportedInfo;
        private GCHandle _unsupportedInfoHandle;
        private GCHandle _unsupportedHandlerHandle;

        public PdfLibrary()
        {
            NativeMethods.FPDF_InitLibrary(IntPtr.Zero);


            _unsupportedInfo = new NativeMethods.UNSUPPORT_INFO();
            _unsupportedInfoHandle = GCHandle.Alloc(_unsupportedInfo);
            _unsupportedInfo.version = 1;
            _unsupportedInfo.FSDK_UnSupport_Handler = Unsupported_Handler;
            _unsupportedHandlerHandle = GCHandle.Alloc(_unsupportedInfo.FSDK_UnSupport_Handler);

            NativeMethods.FSDK_SetUnSpObjProcessHandler(_unsupportedInfo);
        }

        private void Unsupported_Handler(NativeMethods.UNSUPPORT_INFO pThis, NativeMethods.FPDF_UNSP nType)
        {
            string type;

            switch (nType)
            {
                case NativeMethods.FPDF_UNSP.DOC_XFAFORM:
                    type = "XFA";
                    break;
                case NativeMethods.FPDF_UNSP.DOC_PORTABLECOLLECTION:
                    type = "Portfolios_Packages";
                    break;
                case NativeMethods.FPDF_UNSP.DOC_ATTACHMENT:
                case NativeMethods.FPDF_UNSP.ANNOT_ATTACHMENT:
                    type = "Attachment";
                    break;
                case NativeMethods.FPDF_UNSP.DOC_SECURITY:
                    type = "Rights_Management";
                    break;
                case NativeMethods.FPDF_UNSP.DOC_SHAREDREVIEW:
                    type = "Shared_Review";
                    break;
                case NativeMethods.FPDF_UNSP.DOC_SHAREDFORM_ACROBAT:
                case NativeMethods.FPDF_UNSP.DOC_SHAREDFORM_FILESYSTEM:
                case NativeMethods.FPDF_UNSP.DOC_SHAREDFORM_EMAIL:
                    type = "Shared_Form";
                    break;
                case NativeMethods.FPDF_UNSP.ANNOT_3DANNOT:
                    type = "3D";
                    break;
                case NativeMethods.FPDF_UNSP.ANNOT_MOVIE:
                    type = "Movie";
                    break;
                case NativeMethods.FPDF_UNSP.ANNOT_SOUND:
                    type = "Sound";
                    break;
                case NativeMethods.FPDF_UNSP.ANNOT_SCREEN_MEDIA:
                case NativeMethods.FPDF_UNSP.ANNOT_SCREEN_RICHMEDIA:
                    type = "Screen";
                    break;
                case NativeMethods.FPDF_UNSP.ANNOT_SIG:
                    type = "Digital_Signature";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("nType");
            }

            throw new PdfException("Unsupported feature " + type);
        }

        ~PdfLibrary()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                NativeMethods.FPDF_DestroyLibrary();

                if (_unsupportedInfoHandle.IsAllocated)
                    _unsupportedInfoHandle.Free();

                if (_unsupportedHandlerHandle.IsAllocated)
                    _unsupportedHandlerHandle.Free();

                _disposed = true;
            }
        }
    }
}
