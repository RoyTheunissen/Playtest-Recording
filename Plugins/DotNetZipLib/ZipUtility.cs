using System.IO;

namespace Ionic.Zip
{
    /// <summary>
    /// Helps create a .zip file encompassing a number of files.
    /// </summary>
    public static class ZipUtility 
    {
        private const string Extension = ".zip";

        public static string Zip(string archivePath, params string[] fileNames)
        {
            ZipFile zip = new ZipFile();
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (File.Exists(fileNames[i]))
                    zip.AddFile(fileNames[i], string.Empty);
            }
            zip.Save(archivePath + Extension);
            return archivePath + Extension;
        }
    }
}
