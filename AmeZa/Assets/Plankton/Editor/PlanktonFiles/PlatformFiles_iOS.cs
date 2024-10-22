using System.Collections.Generic;
using UnityEditor;

namespace Plankton.Editor.Files
{
    public class PlatformFiles_iOS : PlatformFiles
    {
        public File planktonConfig = new File()
        {
            sourceFilename = "plankton_config_src.json",
            destinationFilename = "/Plugins/iOS/plankton_config.json",
            placeholderSeparator = ",\n    "
        };

        public File podfile = new File()
        {
            sourceFilename = "Podfile_src.txt",
            destinationFilename = "/Plugins/iOS/Podfile",
            placeholderSeparator = "\n  "
        };

        public File skAdNetworks = new File()
        {
            sourceFilename = "sk_ad_networks.xml",
            destinationFilename = "/Plugins/iOS/sk_ad_networks.xml",
            placeholderSeparator = "\n  "
        };

        public File firebase = new File()
        {
            destinationFilename = "/Plugins/iOS/GoogleService-Info.plist"
        };

        public PlatformFiles_iOS()
        {
            files = new List<File>() { planktonConfig, podfile, skAdNetworks, firebase };
            LoadFiles(BuildTarget.iOS);
        }

#if UNITY_IOS
        public override void SaveFiles(bool generateAar)
        {
            podfile.content = podfile.content.Replace("[MINIMUM_TARGET_PLACEHOLDER]", PlayerSettings.iOS.targetOSVersionString);
            
            if (string.IsNullOrEmpty(firebase.content) || firebase.content.Length < 5)
                files.Remove(firebase);

            base.SaveFiles(generateAar);
        }
#endif
    }
}