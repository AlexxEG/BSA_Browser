using System.IO;

namespace SharpBSABA2.Utils
{
    public static class PathUtils
    {
        /// <summary>
        /// Returns the path with the directory separator character normalized to the platform's default.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        public static string NormalizePath(string path) => NormalizePath(path, Path.DirectorySeparatorChar);

        /// <summary>
        /// Returns the path with the directory separator character normalized to the specified character.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <param name="directorySeparatorChar">The character to normalize the directory separator to.</param>
        public static string NormalizePath(string path, char directorySeparatorChar)
        {
            // Replace forward slashes with backward slashes
            path = path.Replace('/', directorySeparatorChar);

            // Replace backward slashes with forward slashes
            path = path.Replace('\\', directorySeparatorChar);

            return path;
        }
    }
}
