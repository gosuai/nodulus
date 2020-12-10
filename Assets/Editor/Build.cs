using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// 
/// Put me inside an Editor folder
/// 
/// Add a Build menu on the toolbar to automate multiple build for different platform
/// 
/// Use #define BUILD in your code if you have build specification 
/// Specify all your Target to build All
/// 
/// Install to Android device using adb install -r "pathOfApk"
/// 
public class BuildCommand : MonoBehaviour
{
    private const string AndroidKeystorePass = "123qwe";
    private const string AndroidKeyaliasName = "key0";
    private const string AndroidKeyaliasPass = "123qwe";
    private const string BuildPathRoot = "build";

    private static readonly BuildTarget[] TargetToBuildAll =
    {
        BuildTarget.Android,
        BuildTarget.StandaloneLinux64,
    };

    private static string ProductName => PlayerSettings.productName;

    private static int AndroidLastBuildVersionCode
    {
        get => PlayerPrefs.GetInt("LastVersionCode", -1);
        set => PlayerPrefs.SetInt("LastVersionCode", value);
    }

    private static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneOSX:
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
                return BuildTargetGroup.Standalone;
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;
            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;
            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;
            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;
            default:
                return BuildTargetGroup.Standalone;
        }
    }

    private static string GetExtension(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneOSX:
                break;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            case BuildTarget.iOS:
                break;
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.WebGL:
                break;
            case BuildTarget.StandaloneLinux64:
                break;
            case BuildTarget.XboxOne:
                break;
            case BuildTarget.tvOS:
                break;
            case BuildTarget.NoTarget:
                break;
        }

        return ".unknown";
    }

    private static BuildPlayerOptions GetDefaultPlayerOptions()
    {
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = (from s in EditorBuildSettings.scenes where s.enabled select s.path).ToArray(),
            options = BuildOptions.None
        };


        // To define
        // buildPlayerOptions.locationPathName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\LightGunBuild\\Android\\LightGunMouseArcadeRoom.apk";
        // buildPlayerOptions.target = BuildTarget.Android;

        return buildPlayerOptions;
    }

    private static void DefaultBuild(BuildTarget buildTarget)
    {
        var targetGroup = ConvertBuildTarget(buildTarget);

        var path = Path.Combine(Path.Combine(BuildPathRoot, targetGroup.ToString()));
        var name = ProductName + GetExtension(buildTarget);


        var defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbol + ";BUILD");

        PlayerSettings.Android.keystorePass = AndroidKeystorePass;
        PlayerSettings.Android.keyaliasName = AndroidKeyaliasName;
        PlayerSettings.Android.keyaliasPass = AndroidKeyaliasPass;

        var buildPlayerOptions = GetDefaultPlayerOptions();

        buildPlayerOptions.locationPathName = Path.Combine(path, name);
        buildPlayerOptions.target = buildTarget;

        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);

        var result = buildPlayerOptions.locationPathName + ": " + BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log(result);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbol);

        if (buildTarget == BuildTarget.Android)
            AndroidLastBuildVersionCode = PlayerSettings.Android.bundleVersionCode;

        EditorUtility.RevealInFinder(path);
    }

    [MenuItem("Build/Build Specific/Build Android")]
    private static void BuildAndroid()
    {
        DefaultBuild(BuildTarget.Android);
    }

    [MenuItem("Build/Build Specific/Build Win32")]
    private static void BuildWin32()
    {
        DefaultBuild(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Build/Build Specific/Build Win64")]
    private static void BuildWin64()
    {
        DefaultBuild(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Get Build Number")]
    private static void BuildNumber()
    {
        Debug.Log("Current/Last: " + PlayerSettings.Android.bundleVersionCode + "/" + AndroidLastBuildVersionCode);
    }

    [MenuItem("Build/Build Number/Up Build Number")]
    private static void BuildNumberUp()
    {
        PlayerSettings.Android.bundleVersionCode++;
        BuildNumber();
    }

    [MenuItem("Build/Build Number/Down Build Number")]
    private static void BuildNumberDown()
    {
        PlayerSettings.Android.bundleVersionCode--;
        BuildNumber();
    }

    [MenuItem("Build/Build All")]
    private static void BuildAll()
    {
        List<BuildTarget> buildTargetLeft = new List<BuildTarget>(TargetToBuildAll);

        if (buildTargetLeft.Contains(EditorUserBuildSettings.activeBuildTarget))
        {
            DefaultBuild(EditorUserBuildSettings.activeBuildTarget);
            buildTargetLeft.Remove(EditorUserBuildSettings.activeBuildTarget);
        }

        foreach(var b in buildTargetLeft)
        {
            DefaultBuild(b);
        }
    }
}
