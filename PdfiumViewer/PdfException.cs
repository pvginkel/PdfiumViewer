using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PdfiumViewer
{
    public class PdfException : Exception
    {
        public PdfException()
        {
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
