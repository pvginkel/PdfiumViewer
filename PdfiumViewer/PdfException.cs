using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PdfiumViewer
{
    public class PdfException : Exception
    {
        public PdfError Error { get; private set; }

        public PdfException()
        {
        }

        public PdfException(PdfError error)
            : this(GetMessage(error))
        {
            Error = error;
        }

        private static string GetMessage(PdfError error)
        {
            switch (error)
            {
                case PdfError.Success:
                    return "No error";
                case PdfError.CannotOpenFile:
                    return "File not found or could not be opened";
                case PdfError.InvalidFormat:
                    return "File not in PDF format or corrupted";
                case PdfError.PasswordProtected:
                    return "Password required or incorrect password";
                case PdfError.UnsupportedSecurityScheme:
                    return "Unsupported security scheme";
                case PdfError.PageNotFound:
                    return "Page not found or content error";
                default:
                    return "Unknown error";
            }
        }

        public PdfException(string message)
            : base(message)
        {
        }

        public PdfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PdfException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
