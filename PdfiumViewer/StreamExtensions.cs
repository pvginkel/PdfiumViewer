using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfiumViewer
{
    internal static class StreamExtensions
    {
        public static byte[] ToByteArray(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var memoryStream = stream as MemoryStream;

            if (memoryStream != null)
                return memoryStream.ToArray();

            if (stream.CanSeek)
                return ReadBytesFast(stream);
            else
                return ReadBytesSlow(stream);
        }

        private static byte[] ReadBytesFast(Stream stream)
        {
            byte[] data = new byte[stream.Length];
            int offset = 0;

            while (offset < data.Length)
            {
                int read = stream.Read(data, offset, data.Length - offset);

                if (read <= 0)
                    break;

                offset += read;
            }

            if (offset < data.Length)
                throw new InvalidOperationException("Incorrect length reported");

            return data;
        }

        private static byte[] ReadBytesSlow(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                CopyStream(stream, memoryStream);

                return memoryStream.ToArray();
            }
        }

        public static void CopyStream(Stream from, Stream to)
        {
            if (@from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");

            var buffer = new byte[4096];

            while (true)
            {
                int read = from.Read(buffer, 0, buffer.Length);

                if (read == 0)
                    return;

                to.Write(buffer, 0, read);
            }
        }
    }
}
