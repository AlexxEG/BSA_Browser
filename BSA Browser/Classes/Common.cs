using System;
using System.Collections.Generic;

namespace BSA_Browser.Classes
{
    public class Common
    {
        /// <summary>
        /// Formats file size into a human readable string.
        /// </summary>
        /// <param name="bytes">The file size to format.</param>
        private string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

        /// <summary>
        /// Formats TimeSpan into a human readable string.
        /// </summary>
        /// <param name="time">The TimeSpan to format.</param>
        public static string FormatTimeRemaining(TimeSpan time)
        {
            List<string> ss = new List<string>();

            if (time.Hours > 0) ss.Add(time.Hours == 1 ? "1 hour" : $"{time.Hours} hours");
            if (time.Minutes > 0) ss.Add(time.Minutes == 1 ? "1 minute" : $"{time.Minutes} minutes");
            if (time.Seconds > 0) ss.Add(time.Seconds == 1 ? "1 second" : $"{time.Seconds} seconds");

            switch (ss.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return $"About {ss[0]} remaining";
                case 2:
                    return $"About {ss[0]} and {ss[1]} remaining";
                case 3:
                    return $"About {ss[0]}, {ss[1]} and {ss[2]} remaining";
                default:
                    throw new Exception();
            }
        }
    }
}
