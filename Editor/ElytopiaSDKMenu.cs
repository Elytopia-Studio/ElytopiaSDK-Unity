using UnityEditor;

namespace ElytopiaEditor.Editor
{
    internal static class ElytopiaSDKMenu
    {
        [MenuItem("Tools/ElytopiaSDK/Install WebGL Template")]
        public static void ReinstallTemplate()
        {
            ElytopiaSDKWebGLTemplateImporter.TryInstallTemplate();
        }
    }
}