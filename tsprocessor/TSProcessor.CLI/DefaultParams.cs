using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace TSProcessor.CLI
{
    static class DefaultParams
    {
        public const float sigma = 10f;
        public const float r = 26f;
        public const float b = 2.666f;

        public const float generationStep = 0.05f;
        public const int skipCount = 3000;
        public const int sequenceCount = 13500;//16500

        public static readonly Vector3 Y0 = new Vector3(10, -1, 1);

        public static readonly string seriesPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "numbers.json");

        public static readonly string paintPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "paint.json");

        public static readonly string clustersPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "clusters");
    }
}
