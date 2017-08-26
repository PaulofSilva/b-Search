using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace libbSearch.EncodingDetection
{
    /// <summary>
    /// File detection encoding options.
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public class EncodingOptions
    {
        private const int SampleSize_Speed = 1024;
        private const int SampleSize_Default = 5120;
        private const int SampleSize_Accuracy = 10240;

        /// <summary>
        /// Performance levels that determine what detection methods are used.
        /// </summary>
        public enum Performance
        {
            /// <summary></summary>
            Speed = 1,
            /// <summary></summary>
            Default = 2,
            /// <summary></summary>
            Accuracy = 4
        }

        /// <summary>
        /// Determines whether file encoding detection is enabled.
        /// </summary>
        public bool DetectFileEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the current performance level.
        /// </summary>
        public Performance PerformanceSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets whether the encoding cached is used.
        /// </summary>
        public bool UseEncodingCache
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an instance of the EncodingOptions class.
        /// </summary>
        public EncodingOptions()
        {
            DetectFileEncoding = true;
            PerformanceSetting = Performance.Default;
            UseEncodingCache = true;
        }

        /// <summary>
        /// Creates an instance of the EncodingOptions class.
        /// </summary>
        /// <param name="detectFileEncoding">determines if file encoding detection is used</param>

        public EncodingOptions(bool detectFileEncoding)
            : this()
        {
            DetectFileEncoding = detectFileEncoding;
        }
        /// <summary>
        /// Creates an instance of the EncodingOptions class.
        /// </summary>
        /// <param name="detectFileEncoding">determines if file encoding detection is used</param>
        /// <param name="performanceSetting">current performance setting</param>

        public EncodingOptions(bool detectFileEncoding, Performance performanceSetting)
            : this(detectFileEncoding)
        {
            PerformanceSetting = performanceSetting;
        }

        /// <summary>
        /// Creates an instance of the EncodingOptions class.
        /// </summary>
        /// <param name="performanceSetting">current performance setting</param>

        public EncodingOptions(Performance performanceSetting)
            : this()
        {
            PerformanceSetting = performanceSetting;
        }

        /// <summary>
        /// Retrieves the desired encoding detectors based on the given performance setting.
        /// </summary>
        /// <param name="performanceSetting">current performance setting</param>
        /// <returns>EncodingDetector.Options bit flag representation of selected detectors</returns>

        public static EncodingDetector.Options GetEncodingDetectorOptionsByPerformance(Performance performanceSetting)
        {
            EncodingDetector.Options opts = EncodingDetector.Options.KlerkSoftBom | EncodingDetector.Options.WinMerge | EncodingDetector.Options.MLang;

            switch (performanceSetting)
            {
                case Performance.Speed:
                    opts = EncodingDetector.Options.KlerkSoftBom | EncodingDetector.Options.WinMerge;
                    break;

                case Performance.Default:
                default:
                    opts = EncodingDetector.Options.KlerkSoftBom | EncodingDetector.Options.WinMerge | EncodingDetector.Options.MLang;
                    break;

                case Performance.Accuracy:
                    opts = EncodingDetector.Options.KlerkSoftBom | EncodingDetector.Options.WinMerge | EncodingDetector.Options.MozillaUCD | EncodingDetector.Options.MLang;
                    break;
            }

            return opts;
        }

        /// <summary>
        /// Retrieves the maximum number of bytes for a sample size based on a given performance setting.
        /// </summary>
        /// <param name="performanceSetting">current performance setting</param>
        /// <returns>the maximum number of bytes used in the sample size</returns>

        public static int GetSampleSizeByPerformance(Performance performanceSetting)
        {
            int size = SampleSize_Default;

            switch (performanceSetting)
            {
                case Performance.Speed:
                    size = SampleSize_Speed;
                    break;

                case Performance.Default:
                default:
                    size = SampleSize_Default;
                    break;

                case Performance.Accuracy:
                    size = SampleSize_Accuracy;
                    break;
            }

            return size;
        }
    }
}
