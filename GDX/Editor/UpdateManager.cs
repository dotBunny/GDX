using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;


namespace GDX.Editor
{
    /// <summary>
    ///
    /// </summary>
    [InitializeOnLoad]
    public class UpdateManager
    {
        /// <summary>
        /// A miniature package definition useful for quickly parsing a remote package definition.
        /// </summary>
        [Serializable]
        public class PackageDefinition
        {
            public string version;
            public string unity;
        }

        private const string LastNotifiedPreferencesKey = "GDX.LastNotifiedVersion";


        public string GetLastNotifiedVersion()
        {
            return EditorPrefs.GetString(LastNotifiedPreferencesKey);
        }
        public void SetLastNotifiedVersion(string versionTag)
        {
            EditorPrefs.SetString(LastNotifiedPreferencesKey, versionTag );
        }

        public static string GetProjectRootPath()
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static InstallationMethod Installation = InstallationMethod.Unidentified;
        /// <summary>
        /// A defined collection of ways that the package could be found in a Unity project.
        /// </summary>
        public enum InstallationMethod
        {
            Unidentified = 0,
            UnityPackageManager = 1,
            UnityAssetStore = 2,
            // ReSharper disable once InconsistentNaming
            OpenUPM = 3,
            LocalPackage = 4,
            GitHubClone = 5
        }

        public UpdateManager()
        {
       //     Installation = GetInstallationMethod();
        //    LatestDefinition = GetMainPackageDefinition();

        // Only pool periodically
        }

        private static PackageDefinition GetLocalPackageDefinition()
        {
            try
            {
                string[] packageGuids = AssetDatabase.FindAssets("package");
                int packageCount = packageGuids.Length;
                for (int i = 0; i < packageCount; i++)
                {
                    string packagePath = Path.Combine(GetProjectRootPath(), AssetDatabase.GUIDToAssetPath(packageGuids[i]));
                    string[] packageContent = File.ReadAllLines(packagePath);

                    if(packageContent.Length > 15 &&
                        packageContent[1].Trim() == "\"name\": \"com.dotbunny.gdx\"," &&
                        packageContent[14].Trim() == "\"name\": \"dotBunny\",")
                    {
                        return JsonUtility.FromJson<PackageDefinition>(File.ReadAllText(packagePath));
                    }

                }

                return null;
            }
            catch (Exception e)
            {
                // Don't go any further if there is an error
                return null;
            }
        }

        private static PackageDefinition GetMainPackageDefinition()
        {
            try
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                using WebClient webClient = new WebClient();

                // Get content of the package definition file
                string packageJsonContent = webClient.DownloadString("https://raw.githubusercontent.com/dotBunny/GDX/main/package.json");

                // Return back the parsed object or null if there was no content.
                return string.IsNullOrEmpty(packageJsonContent) ? null : JsonUtility.FromJson<PackageDefinition>(packageJsonContent);
            }
            catch (Exception e)
            {
                // Don't go any further if there is an error
                return null;
            }
        }

        /// <summary>
        /// Determine the current <see cref="InstallationMethod"/> of the GDX package.
        /// </summary>
        /// <returns>The discovered <see cref="InstallationMethod"/>.</returns>
        private static InstallationMethod GetInstallationMethod()
        {

            // Well we reached this point and don't actually know, so guess we should admit it.
            return InstallationMethod.Unidentified;
        }
    }
}
