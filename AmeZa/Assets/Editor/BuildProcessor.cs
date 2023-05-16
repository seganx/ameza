using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using SeganX;

public class PreprocessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => -2;

    public void OnPreprocessBuild(BuildReport report)
    {
        GlobalConfig.Instance.version = SeganX.Builder.Builder.Instance.version;
    }
}