using Elytopia;
using UnityEditor;

namespace ElytopiaEditor.Editor
{
    [InitializeOnLoad]
    internal static class ElytopiaSDKMenu
    {
        static ElytopiaSDKMenu()
        {
            ElytopiaSDKWebGLTemplateImporter.EditorAutoTryInstallTemplate();
            EditorApplication.delayCall += ElytopiaSDKWebGLTemplateImporter.EditorAutoTryInstallTemplate;
        }

        [MenuItem("Tools/ " + nameof(ElytopiaSDK) + "/Install WebGL Template", priority = 1)]
        public static void ReinstallTemplate() => ElytopiaSDKWebGLTemplateImporter.TryInstallTemplate();

        [MenuItem("Tools/ " + nameof(ElytopiaSDK) + "/Enable Auto Install WebGL Template", priority = 200)]
        public static void EnableAutoInstall()
        {
            ElytopiaSDKWebGLTemplateImporter.IsEnableAutoInstall = true;
            ElytopiaSDKWebGLTemplateImporter.TryInstallTemplate();
        }

        [MenuItem("Tools/ " + nameof(ElytopiaSDK) + "/Disable Auto Install WebGL Template", priority = 201)]
        public static void DisableAutoInstall() => ElytopiaSDKWebGLTemplateImporter.IsEnableAutoInstall = false;
    }
}