#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// One-click builds: Menu "Detective Game > Build ...". Install the matching Unity
/// build-support module (Windows/Mac/Linux/Android/WebGL) in Unity Hub first.
public static class BuildScript
{
    static void Build(BuildTarget target, BuildTargetGroup group, string path)
    {
        var opts = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = path, target = target, targetGroup = group, options = BuildOptions.None
        };
        var report = BuildPipeline.BuildPlayer(opts);
        Debug.Log($"[Build] {target}: {report.summary.result} -> {path}");
    }

    [MenuItem("Detective Game/Build Windows 64")] static void Win() => Build(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, "Builds/Windows/Marked.exe");
    [MenuItem("Detective Game/Build macOS")] static void Mac() => Build(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, "Builds/macOS/Marked.app");
    [MenuItem("Detective Game/Build Linux 64")] static void Lin() => Build(BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone, "Builds/Linux/Marked.x86_64");
    [MenuItem("Detective Game/Build Android APK")] static void Apk() => Build(BuildTarget.Android, BuildTargetGroup.Android, "Builds/Android/Marked.apk");
    [MenuItem("Detective Game/Build WebGL")] static void Web() => Build(BuildTarget.WebGL, BuildTargetGroup.WebGL, "Builds/WebGL");
}
#endif
