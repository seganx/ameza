using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using SeganX;
using UnityEditor;

public class PreprocessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => -2;

    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_IOS
#if GOOGLE || MYKET || BAZAAR || IRGOOGLE || SIBAPP
        throw new BuildFailedException("iOS version couldn't have any symbol of GOOGLE || MYKET || BAZAAR || IRGOOGLE || SIBAPP");        
#endif
#else
        GlobalConfig.Instance.version = SeganX.Builder.Builder.Instance.version;
        PlayerSettings.iOS.buildNumber = SeganX.Builder.Builder.Instance.bundleVersionCode.ToString();
#endif
    }
}