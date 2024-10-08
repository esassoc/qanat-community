/*-----------------------------------------------------------------------
<copyright file="DisposableTempDirectory.cs" company="Sitka Technology Group">
Copyright (c) Sitka Technology Group. All rights reserved.
<author>Sitka Technology Group</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/

namespace Qanat.Common
{
    public class DisposableTempDirectory : IDisposable
    {
        private DirectoryInfo _directoryInfo;

        private bool _isDisposed;

        public DisposableTempDirectory()
            : this(Path.GetTempFileName())
        {
        }

        public DisposableTempDirectory(string tempFileName)
        {
            _directoryInfo = new DirectoryInfo(tempFileName);
        }

        public static DisposableTempDirectory MakeDisposableTempDirectoryEndingIn(string directoryEnding)
        {
            var tempFileName = Path.GetTempFileName();
            File.Delete(tempFileName); // we need to delete this right away once we get the path; Path.GetTempFileName() creates a zero byte file on disk
            var tempPath = tempFileName + directoryEnding;
            return new DisposableTempDirectory(tempPath);
        }

        public DirectoryInfo DirectoryInfo
        {
            get
            {
                Check.Require(!_isDisposed, "Object is already disposed");
                return _directoryInfo;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_directoryInfo != null)
                {
                    var fileFullName = _directoryInfo.FullName;
                    if (Directory.Exists(fileFullName))
                    {
                        Directory.Delete(fileFullName, true);
                    }
                    _directoryInfo = null;
                }
                _isDisposed = true;
            }
        }

        ~DisposableTempDirectory()
        {
            Dispose();
        }
    }
}
