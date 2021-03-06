﻿using System;
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
        public const int sequenceCount = 13500;
        public const int testsCount = 1000;

        public static readonly Vector3 Y0 = new Vector3(10, -1, 1);

        public static readonly string seriesPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "learning.json");

        public static readonly string testsPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "tests.json");

        public static readonly string paintPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "paint.json");

        public static readonly string forecastPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "forecast.json");

        public static readonly string clustersPath
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "clusters");
    }
}
