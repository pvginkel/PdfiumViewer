using System;
using System.Collections.Generic;
using System.Text;

namespace PdfiumViewer
{
    internal static class MathEx
    {
        // Taken from https://en.wikipedia.org/wiki/Smoothstep.
        public static double SmoothStep(double min, double max, double x)
        {
            // Scale, bias and saturate x to 0..1 range.
            x = Clamp((x - min) / (max - min), 0.0, 1.0);
            // Evaluate polynomial.
            return x * x * (3 - 2 * x);
        }

        public static double Clamp(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }
}
