using System;
using System.IO;

namespace Assets.Scripts.Util.IO
{

    /// <summary>
    /// Simple execute around a temporary directory.
    /// Creates the directory on instantiation and deletes it on Dispose().
    /// </summary>
    public class TempDirectory : IDisposable
    {
        private readonly string _path;

        /// <param name="root">Base path for the temp folder (will be created if missing).</param>
        public TempDirectory(string root)
        {
            _path = root ?? throw new ArgumentNullException("root");
            Directory.CreateDirectory(_path);
        }

        public void Dispose()
        {
            DeletePath();
        }

        private void DeletePath()
        {
            if (Directory.Exists(_path))
                Directory.Delete(_path, recursive: true);
        }
    }
}