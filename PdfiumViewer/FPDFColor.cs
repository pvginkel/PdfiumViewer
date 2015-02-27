using System.Runtime.InteropServices;

namespace PdfiumViewer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FPDFColor
    {
        public uint Raw
        {
            get { return rawValue; }
            set { rawValue = value; }
        }
        public byte A
        {
            get { return (byte)((rawValue & 0xFF000000) >> 24); }
        }
        public byte R
        {
            get { return (byte)((rawValue & 0x00FF0000) >> 16); }
        }
        public byte G
        {
            get { return (byte)((rawValue & 0x0000FF00) >> 8); }
        }
        public byte B
        {
            get { return (byte)(rawValue & 0x000000FF); }
        }
        private uint rawValue;

        public FPDFColor(uint rawValue)
        {
            this.rawValue = rawValue;
        }
    }
}
