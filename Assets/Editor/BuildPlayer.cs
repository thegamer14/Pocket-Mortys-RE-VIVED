using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace PocketMortys.EditorTools
{
    public static class BuildPlayer
    {
        public static void Build()
        {
            string root = Directory.GetParent(UnityEngine.Application.dataPath).FullName;
            string outDir = Path.Combine(root, "Build/Windows");
            Directory.CreateDirectory(outDir);

            string[] scenes = EditorBuildSettings.scenes
                .Where(x => x.enabled)
                .Select(x => x.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                UnityEngine.Debug.LogError("[Build] no enabled scenes");
                EditorApplication.Exit(2);
                return;
            }

            UnityEngine.Debug.Log("[Build] scenes: " + string.Join(", ", scenes));

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = Path.Combine(outDir, "PocketMortysOffline.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            UnityEngine.Debug.Log(string.Format(
                "[Build] result={0} size={1}MB errors={2} warnings={3}",
                summary.result,
                (summary.totalSize / (1024f * 1024f)).ToString("F0"),
                summary.totalErrors,
                summary.totalWarnings));

            EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
        }
    }
}
