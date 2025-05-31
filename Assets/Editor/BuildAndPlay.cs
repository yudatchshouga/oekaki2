using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildAndPlay
{
    [MenuItem("Tools/BuildAndPlay")]
    public static void BuildAndLaunchAndPlay()
    {
        string exePath = Path.GetFullPath("Builds/MyGame.exe");
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        // ビルド
        BuildReport report = BuildPipeline.BuildPlayer(scenes, exePath, BuildTarget.StandaloneWindows64, BuildOptions.Development);
        if (report.summary.result != BuildResult.Succeeded)
        {
            UnityEngine.Debug.LogError("ビルド失敗");
            return;
        }

        // ビルドされた.exeを起動（1人目）
        if (File.Exists(exePath))
        {
            Process.Start(exePath);
        }
        else
        {
            UnityEngine.Debug.LogError("ビルドされた .exe が見つかりません");
        }

        // エディタ上で Play 開始（2人目）
        EditorApplication.isPlaying = true;
    }
}
