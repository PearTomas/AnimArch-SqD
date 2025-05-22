using System;
using System.IO;

namespace Assets.Scripts.Util.IO
{

    /// <summary>
    /// Simple execute around a temporary directory.
    /// Creates the directory on instantiation and deletes it on destructor.
    /// </summary>
    public class TempDirectory
    {
        private readonly string _path;

        /// <param name="root">Base path for the temp folder (will be created if missing).</param>
        public TempDirectory(string rootPath)
        {
            _path = rootPath ?? throw new ArgumentNullException("rootPath");
            Directory.CreateDirectory(_path);
        }
        
        ~TempDirectory()
        {
            DeletePath();
        }
        
        /// <summary>
        /// delete the temp directory with all its content.
        /// </summary>
        private void DeletePath()
        {
            if (Directory.Exists(_path))
                Directory.Delete(_path, recursive: true);
        }
    }
}