using System;
using System.IO;

namespace ricaun.AppBundleTool.AppBundle
{
    public static class AccessUtils
    {
        public static bool CheckWriteAccess(string path)
        {
            try
            {
                var fileName = Path.Combine(path, Path.GetRandomFileName());
                File.Create(fileName).Close();
                File.Delete(fileName);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
