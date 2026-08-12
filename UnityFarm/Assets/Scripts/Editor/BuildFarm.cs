#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UnityFarm.EditorTools
{
    /// <summary>
    /// 打包 Windows 可执行文件（验收用）。
    /// 运行：Tuanjie.exe -batchmode -projectPath ... -executeMethod UnityFarm.EditorTools.BuildFarm.Build -quit
    /// </summary>
    public static class BuildFarm
    {
        public static void Build()
        {
            const string scenePath = "Assets/Scenes/MainFarm.unity";
            if (!File.Exists(scenePath))
            {
                Debug.LogError("场景不存在，请先运行 UnityFarm → Build Prototype Scene");
                EditorApplication.Exit(1);
                return;
            }

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };

            var opts = new BuildPlayerOptions
            {
                scenes = new[] { scenePath },
                locationPathName = "Build/Farm.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(opts);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("Build failed: " + report.summary.result + " errors=" + report.summary.totalErrors);
                EditorApplication.Exit(1);
            }

            Debug.Log("BUILD_SUCCEEDED: " + report.summary.outputPath + " size=" + report.summary.totalSize);
        }
    }
}
#endif
