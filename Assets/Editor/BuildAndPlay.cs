using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.ShortcutManagement;
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
        BuildSummary summary = report.summary;

        UnityEngine.Debug.Log("Total time: " + summary.totalTime.TotalSeconds + " seconds");
        UnityEngine.Debug.Log("Total size: " + (summary.totalSize / 1024 / 1024) + " MB");

        foreach (var step in report.steps)
        {
            UnityEngine.Debug.Log($"Step: {step.name} | Time: {step.duration.TotalSeconds:F2} seconds");
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
        EditorApplication.delayCall += () =>
        {
            EditorApplication.isPlaying = true;
        };
    }

    [Shortcut("Tools/BuildAndPlay", KeyCode.B, ShortcutModifiers.Action | ShortcutModifiers.Shift)]
    public static void BuildAndLaunchAndPlayShortcut()
    {
        BuildAndLaunchAndPlay();
    }
}
