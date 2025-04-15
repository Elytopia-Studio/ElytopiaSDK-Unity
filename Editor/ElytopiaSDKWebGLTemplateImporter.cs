using UnityEditor;
using UnityEngine;
using System.IO;
using Elytopia;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace ElytopiaEditor.Editor
{
    [InitializeOnLoad]
    public class ElytopiaSDKWebGLTemplateImporter
    {
        private const string SourcePath = "Packages/elytopia.world/Editor/WebGL Template";
        private const string TargetRoot = "Assets/WebGLTemplates";
        private const string PackageAnchor = "Packages/elytopia.world/Runtime/Elytopia.asmdef";
        public static bool IsEnableAutoInstall = true;

        static ElytopiaSDKWebGLTemplateImporter()
        {
            EditorApplication.delayCall += EditorAutoTryInstallTemplate;
        }

        private static void EditorAutoTryInstallTemplate()
        {
            if (IsEnableAutoInstall == false)
                return;

            if (IsPackageFound(out var packageInfo) == false)
                return;

            if (IsInstalledTemplateAbsent(packageInfo) == false)
                return;

            if (IsSourceTemplateMissing())
                return;

            InstallWebGlTemplate(packageInfo);
        }

        public static void TryInstallTemplate()
        {
            if (IsPackageFound(out var packageInfo) == false)
                return;

            if (IsInstalledTemplateAbsent(packageInfo) == false)
            {
                Debug.Log($"[{nameof(ElytopiaSDK)}] WebGL Template {packageInfo.version} already install.");
                return;
            }

            if (IsSourceTemplateMissing())
                return;

            InstallWebGlTemplate(packageInfo);
        }

        private static bool IsPackageFound(out PackageInfo packageInfo)
        {
            packageInfo = PackageInfo.FindForAssetPath(PackageAnchor);
            if (packageInfo == null)
            {
                Debug.LogWarning($"[{nameof(ElytopiaSDK)}] Package not found in path: " + PackageAnchor);
            }

            return packageInfo != null;
        }

        private static bool IsSourceTemplateMissing()
        {
            if (!Directory.Exists(SourcePath))
            {
                Debug.LogWarning($"[{nameof(ElytopiaSDK)}] Source WebGL Template not found in: {SourcePath}");
                return true;
            }

            return false;
        }

        private static bool IsInstalledTemplateAbsent(PackageInfo packageInfo)
        {
            var version = packageInfo.version;
            var templateName = GetTemplateName(version);
            var targetPath = Path.Combine(TargetRoot, templateName);
            return !Directory.Exists(targetPath);
        }

        private static void InstallWebGlTemplate(PackageInfo packageInfo)
        {
            var version = packageInfo.version;
            var templateName = GetTemplateName(version);
            var targetPath = Path.Combine(TargetRoot, templateName);

            Debug.Log($"[{nameof(ElytopiaSDK)}] WebGL Template {version} install in {targetPath}");

            CopyDirectory(SourcePath, targetPath);
            AssetDatabase.Refresh();
        }

        private static string GetTemplateName(string version)
        {
            return $"{nameof(ElytopiaSDK)} {version}";
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                if (file.EndsWith(".meta")) continue;

                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }
    }
}