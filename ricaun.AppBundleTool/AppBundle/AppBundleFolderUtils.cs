using System;
using System.Collections.Generic;
using System.Linq;

namespace ricaun.AppBundleTool.AppBundle
{
    internal class AppBundleFolderUtils
    {
        public static bool TyrGetAppBundleFolder(string folder, out AppBundleFolder appBundleFolder)
        {
            var specialFolders = Get();
            appBundleFolder = AppBundleFolder.AppData;

            foreach (var specialFolder in specialFolders)
            {
                var environmentFolder = Environment.GetFolderPath(specialFolder.Value);
                if (folder.StartsWith(environmentFolder, StringComparison.InvariantCultureIgnoreCase))
                {
                    appBundleFolder = specialFolder.Key;
                    return true;
                }
            }

            return false;
        }
        public static Environment.SpecialFolder[] GetSpecialFolders() => Get().Values.ToArray();
        public static Dictionary<AppBundleFolder, Environment.SpecialFolder> Get()
        {
            return new Dictionary<AppBundleFolder, Environment.SpecialFolder>
            {
                { AppBundleFolder.AppData, Environment.SpecialFolder.ApplicationData },
                { AppBundleFolder.ProgramData, Environment.SpecialFolder.CommonApplicationData },
                { AppBundleFolder.ProgramFiles, Environment.SpecialFolder.ProgramFiles }
            };
        }
    }
}
