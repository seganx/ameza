using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Plankton.Editor
{
    [System.Serializable]
    public abstract class Plugin
    {
        public bool active = false;

        public virtual string Name => "Base";

        public virtual void OnEditorGUI(BuildTarget platform)
        {
            var rect = EditorGUILayout.GetControlRect();
            rect.y += rect.height - 5;
            rect.height = 5;
            GUI.Box(rect, string.Empty);
            active = EditorGUILayout.ToggleLeft($" {Name}", active);
            EditorGUI.indentLevel = 1;
        }

        public abstract void OnPrebuild(FileManager fileManager);
    }

    [System.Serializable]
    public class Plugin_DebugMode : Plugin
    {
        public override string Name => "Debug Mode";

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.android.launcherManifest.placeholders.Add("<meta-data android:name=\"com.databite.DEBUG_MODE\" android:value=\"true\" />");
        }
    }

    [System.Serializable]
    public abstract class Plugin_AppId : Plugin
    {
        public string appId = string.Empty;

        public virtual string appIdLabel => "App Id";

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);
            if (active) appId = EditorGUILayout.TextField(appIdLabel, appId);
        }
    }

    [System.Serializable]
    public abstract class Plugin_Ad : Plugin_AppId
    {
        public List<Network> mediations = new List<Network>();

        protected void DrawNetworks(BuildTarget platform)
        {
            if (active == false) return;
            var rect = EditorGUILayout.GetControlRect();
            GUI.Box(rect, "Additional Networks", EditorStyles.centeredGreyMiniLabel);
            int i = 0;
            foreach (var network in mediations)
            {
                switch (platform)
                {
                    case BuildTarget.iOS: if (string.IsNullOrEmpty(network.pod)) continue; break;
                    case BuildTarget.Android: if (string.IsNullOrEmpty(network.dependency)) continue; break;
                }
                if (i % 4 == 0)
                {
                    rect = EditorGUILayout.GetControlRect();
                    rect.width /= 4;
                }
                network.active = EditorGUI.ToggleLeft(rect, network.name, network.active);
                rect.x += rect.width;
                i++;
            }
        }

        protected void PerformOnFileManager(FileManager fileManager)
        {
            foreach (var item in mediations)
            {
                if (item.active)
                {
                    if (string.IsNullOrEmpty(item.dependency) == false)
                    {
                        fileManager.android.dependencies.Add(item.dependency);
                        if (string.IsNullOrEmpty(item.maven) == false)
                            fileManager.android.repositories.Add(item.maven);
                    }
                    if (string.IsNullOrEmpty(item.pod) == false)
                    {
                        fileManager.ios.podfile.placeholders.Add(item.pod);
                    }
                }
            }
        }

        protected void UpdateNetworks(List<Network> networks)
        {
            // remove obsoluted items
            mediations.RemoveAll(m => !networks.Exists(n => n.name == m.name));

            // add or update remained networks
            for (int i = 0; i < networks.Count; i++)
            {
                var network = networks[i];
                var item = mediations.Find(x => x.name == network.name);
                if (item == null)
                {
                    mediations.Add(item = new Network());
                }

                item.name = network.name;
                item.pod = network.pod;
                item.dependency = network.dependency;
                item.maven = network.maven;
            }
        }

        [System.Serializable]
        public class Network
        {
            public bool active = false;
            public string name = string.Empty;
            public string pod = string.Empty;
            public string dependency = string.Empty;
            public string maven = string.Empty;
        }
    }

    [System.Serializable]
    public class Plugin_Admob : Plugin_Ad
    {
        public override string Name => "AdMob";

        private readonly List<Network> networks = new List<Network>
        {
                new Network() { name = "AppLovin", pod = "pod 'GoogleMobileAdsMediationAppLovin'", dependency = "implementation 'com.google.ads.mediation:applovin:12.1.0.0'" },
                new Network() { name = "Chartboost", pod = "", dependency = "implementation 'com.google.ads.mediation:chartboost:9.6.0.0'", maven = "maven { url 'https://cboost.jfrog.io/artifactory/chartboost-ads/'; content { includeGroup 'com.chartboost'  } }" },
                new Network() { name = "DT Exchange (Fyber)", pod = "", dependency = "implementation 'com.google.ads.mediation:fyber:8.2.5.0'" },
                new Network() { name = "Meta (Facebook)", pod = "pod 'GoogleMobileAdsMediationFacebook'", dependency = "implementation 'com.google.ads.mediation:facebook:6.16.0.0'" },
                new Network() { name = "ironSource", pod = "pod 'GoogleMobileAdsMediationIronSource'", dependency = "implementation 'com.google.ads.mediation:ironsource:7.2.3.0'", maven = "maven { url 'https://android-sdk.is.com/' }" },
                new Network() { name = "Liftoff (Vungle)", pod = "pod 'GoogleMobileAdsMediationVungle'", dependency = "implementation 'com.google.ads.mediation:vungle:7.1.0.0'" },
                new Network() { name = "Mintegral", pod = "pod 'GoogleMobileAdsMediationMintegral'", dependency = "implementation 'com.google.ads.mediation:mintegral:16.4.81.0'", maven = "maven { url 'https://dl-maven-android.mintegral.com/repository/mbridge_android_sdk_oversea' }" },
                new Network() { name = "Pangle", pod = "pod 'GoogleMobileAdsMediationPangle'", dependency = "implementation 'com.google.ads.mediation:pangle:5.8.0.7.0'", maven = "maven { url 'https://artifact.bytedance.com/repository/pangle/' }" },
                new Network() { name = "Unity Ads", pod = "pod 'GoogleMobileAdsMediationUnity'", dependency = "implementation 'com.unity3d.ads:unity-ads:4.2.1'\n\t\timplementation 'com.google.ads.mediation:unity:4.3.0.0'" },
                new Network() { name = "Premium Ads", pod = "pod 'PremiumAdmobAdapter'", dependency = "implementation 'net.premiumads.sdk:admob-adapter:2.2.4'", maven = "maven { url 'https://repo.premiumads.net/artifactory/mobile-ads-sdk/' }" },
        };

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);
            UpdateNetworks(networks);
            DrawNetworks(platform);
        }

        public override void OnPrebuild(FileManager fileManager)
        {
            UpdateNetworks(networks);
            PerformOnFileManager(fileManager);
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonAdsAdMobSDK'");
            fileManager.ios.planktonConfig.placeholders.Add($"\"GADApplicationIdentifier\": \"{appId}\"");
            fileManager.android.dependencies.Add("implementation 'com.google.android.gms:play-services-ads:22.6.0'");
            fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"{appId}\"/>");
        }
    }

    [System.Serializable]
    public class Plugin_Applovin : Plugin_Ad
    {
        public string adMobAppId = string.Empty;

        public override string Name => "AppLovin MAX";
        public override string appIdLabel => "SDK Key";

        private readonly List<Network> networks = new List<Network>
        {
                new Network() { name = "AdMob", pod = "pod 'AppLovinMediationGoogleAdapter'", dependency = "implementation 'com.applovin.mediation:google-adapter:22.6.0.0'" },
                new Network() { name = "Google Ad Manager", pod = "pod 'AppLovinMediationGoogleAdManagerAdapter'" },
                new Network() { name = "DT Exchange (Fyber)", pod = "", dependency = "implementation 'com.applovin.mediation:fyber-adapter:8.2.5.0'" },
                new Network() { name = "ironSource", pod = "pod 'AppLovinMediationIronSourceAdapter'", dependency = "implementation 'com.applovin.mediation:ironsource-adapter:7.2.3.0.0'", maven = "maven { url 'https://android-sdk.is.com/' }" },
                new Network() { name = "Liftoff (Vungle)", pod = "pod 'AppLovinMediationVungleAdapter'", dependency = "implementation 'com.applovin.mediation:vungle-adapter:7.1.0.0'" },
                new Network() { name = "Meta (Facebook)", pod = "pod 'AppLovinMediationFacebookAdapter'", dependency = "implementation 'com.applovin.mediation:facebook-adapter:6.16.0.2'" },
                new Network() { name = "Mintegral", pod = "pod 'AppLovinMediationMintegralAdapter'", dependency = "implementation 'com.applovin.mediation:mintegral-adapter:16.5.91.2'\n\t\timplementation 'androidx.recyclerview:recyclerview:1.1.0'", maven = "maven { url 'https://dl-maven-android.mintegral.com/repository/mbridge_android_sdk_oversea' }" },
                new Network() { name = "Pangel", pod = "pod 'AppLovinMediationByteDanceAdapter'" },
                new Network() { name = "Unity Ads", pod = "pod 'AppLovinMediationUnityAdsAdapter'", dependency = "implementation 'com.applovin.mediation:unityads-adapter:4.9.2.1'" },
                new Network() { name = "Premium Ads", pod = "pod 'AppLovinMediationGoogleAdManagerAdapter'", dependency = "implementation 'com.applovin.mediation:google-ad-manager-adapter:23.3.0.1'" },
        };

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);

            if (active && NeedAdMobAppId())
                adMobAppId = EditorGUILayout.TextField("AdMob App Id", adMobAppId);
            
            UpdateNetworks(networks);
            DrawNetworks(platform);
        }

        public override void OnPrebuild(FileManager fileManager)
        {
            UpdateNetworks(networks);
            PerformOnFileManager(fileManager);
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonAdsAppLovinSDK'");
            fileManager.ios.planktonConfig.placeholders.Add($"\"AppLovinSdkKey\": \"{appId}\"");
            fileManager.android.dependencies.Add("implementation 'com.applovin:applovin-sdk:12.1.0'");
            fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"applovin.sdk.key\" android:value=\"{appId}\"/>");

            if (NeedAdMobAppId())
            {
                fileManager.ios.planktonConfig.placeholders.Add($"\"GADApplicationIdentifier\": \"{adMobAppId}\"");
                fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"{adMobAppId}\"/>");
            }
        }

        private bool NeedAdMobAppId()
        {
            var adMob = mediations.Find(x => x.name == "AdMob");
            var googleAdMan = mediations.Find(x => x.name == "Google Ad Manager");
            return (adMob != null && adMob.active) || (googleAdMan != null && googleAdMan.active);
        }
    }

    [System.Serializable]
    public class Plugin_Tapsell : Plugin_Ad
    {
        public string adMobKey = string.Empty;
        public string appLovinKey = string.Empty;

        public override string Name => "Tapsell Plus";

#if TAPSELL
        private readonly Mediation[] models = new Mediation[]
        {
                new Mediation() { name = "AdColony", dependency = "implementation 'ir.tapsell.mediation.adapter:adcolony:1.0.1-beta03'" },
                new Mediation() { name = "AdMob", dependency = "implementation 'ir.tapsell.mediation.adapter:admob:1.0.1-beta03'" },
                new Mediation() { name = "AppLovin", dependency = "implementation 'ir.tapsell.mediation.adapter:applovin:1.0.1-beta03'" },
                new Mediation() { name = "ironSource", dependency = "implementation 'ir.tapsell.mediation.adapter:ironsource:1.0.1-beta03'", maven = "maven { url 'https://android-sdk.is.com/' }" },
                new Mediation() { name = "Mintegral", dependency = "implementation 'ir.tapsell.mediation.adapter:mintegral:1.0.1-beta03'", maven = "maven { url 'https://dl-maven-android.mintegral.com/repository/mbridge_android_sdk_oversea' }" },
                new Mediation() { name = "Liftoff (Vungle)", dependency = "implementation 'ir.tapsell.mediation.adapter:liftoff:1.0.1-beta03'" },
                new Mediation() { name = "Unity Ads", dependency = "implementation 'ir.tapsell.mediation.adapter:unityads:1.0.1-beta03'" },
        };
#endif

        public override void OnEditorGUI(BuildTarget platform)
        {
#if TAPSELL
            base.OnEditorGUI(platform);
            if (active == false) return;
            DrawMediations(models, platform);

            var adMob = mediations.Find(x => x.name == "AdMob");
            var appLovin = mediations.Find(x => x.name == "AppLovin");
            var defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;
            if (adMob != null && adMob.active)
                adMobKey = EditorGUILayout.DelayedTextField("AdMob Adapter Signature", adMobKey);
            if (appLovin != null && appLovin.active)
                appLovinKey = EditorGUILayout.DelayedTextField("appLovin Adapter Signature", appLovinKey);
            EditorGUIUtility.labelWidth = defaultWidth;
#endif
        }

        public override void OnPrebuild(FileManager fileManager)
        {
#if TAPSELL
            AddMediations(fileManager, models);
            fileManager.android.dependencies.Add("implementation 'ir.tapsell.mediation:tapsell:1.0.1-beta03'");
            fileManager.android.dependencies.Add("implementation 'ir.tapsell.mediation.adapter:legacy:1.0.1-beta03'");
            fileManager.android.baseProjectTemplate.placeholders.Add("classpath 'com.dipien:bye-bye-jetifier:1.2.1'");
            fileManager.android.settingsTemplate.placeholders.Add("classpath 'com.dipien:bye-bye-jetifier:1.2.1'");
            fileManager.android.launcherTemplate.placeholders.Add($"TapsellMediationAppKey:\"{appId}\"");
#if MYKET
            fileManager.android.launcherTemplate.placeholders.Add($"TapsellMediationAppMarket:\"Myket\"");
#elif BAZAAR
            fileManager.android.launcherTemplate.placeholders.Add($"TapsellMediationAppMarket:\"Bazaar\"");
#elif UNITY_ANDROID
            fileManager.android.launcherTemplate.placeholders.Add($"TapsellMediationAppMarket:\"GooglePlay\"");                
#endif

            var adMob = mediations.Find(x => x.name == "AdMob");
            var appLovin = mediations.Find(x => x.name == "AppLovin");
            if (adMob != null && adMob.active)
                fileManager.android.launcherTemplate.placeholders.Add($"TapsellMediationAdmobAdapterSignature:\"{adMobKey}\"");
            if (appLovin != null && appLovin.active)
                fileManager.android.launcherTemplate.placeholders.Add($"TapsellMediationApplovinAdapterSignature:\"{appLovinKey}\"");
#endif
        }
    }

    [System.Serializable]
    public class Plugin_Firebase : Plugin
    {
        public string ios_plist = string.Empty;
        public string gcm_defaultSenderId = string.Empty;
        public string google_storage_bucket = string.Empty;
        public string project_id = string.Empty;
        public string google_api_key = string.Empty;
        public string google_crash_reporting_api_key = string.Empty;
        public string google_app_id = string.Empty;
        public string default_web_client_id = string.Empty;
        public bool activePushNotification = false;

        public override string Name => "Firebase";

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);
            if (active == false) return;

            var lastIndentLevel = EditorGUI.indentLevel;
            switch (platform)
            {
                case BuildTarget.iOS:
                    {
                        if (string.IsNullOrEmpty(ios_plist))
                        {
                            EditorGUILayout.HelpBox("Firebase configuration NOT found!\nPlease download Firebase config file from your Firebase panel and open it via the button below.", MessageType.Warning);
                        }
                        else
                        {
                            activePushNotification = EditorGUILayout.ToggleLeft("Push Notification", activePushNotification);
                            EditorGUILayout.HelpBox("Firebase configuration found", MessageType.Info);
                        }

                        if (GUILayout.Button("Open Firebase file"))
                        {
                            var inputJsonFilePath = EditorUtility.OpenFilePanel("Choose firebase config file", "", "plist");
                            if (string.IsNullOrEmpty(inputJsonFilePath)) return;
                            ios_plist = System.IO.File.ReadAllText(inputJsonFilePath);
                        }
                    }
                    break;
                case BuildTarget.Android:
                    {
                        if (string.IsNullOrEmpty(gcm_defaultSenderId) || string.IsNullOrEmpty(project_id) || string.IsNullOrEmpty(google_app_id))
                        {
                            EditorGUILayout.HelpBox("Firebase configuration NOT found!\nPlease download Firebase JSON from your Firebase panel and open it via the button below.", MessageType.Warning);
                        }
                        else
                        {
                            activePushNotification = EditorGUILayout.ToggleLeft("Push Notification", activePushNotification);
                            EditorGUILayout.HelpBox("Firebase configuration found", MessageType.Info);
                        }

                        if (GUILayout.Button("Open Firebase Json"))
                        {
                            var inputJsonFilePath = EditorUtility.OpenFilePanel("Choose firebase config file", "", "json");
                            if (string.IsNullOrEmpty(inputJsonFilePath)) return;
                            var inputJsonText = System.IO.File.ReadAllText(inputJsonFilePath);
                            var inputObj = JsonUtility.FromJson<FireBaseJsonObject>(inputJsonText);
                            var client = inputObj.client.Find(x => x.client_info.android_client_info.package_name == Application.identifier);
                            if (client == null)
                            {
                                EditorUtility.DisplayDialog("Error", $"Can't find {Application.identifier} in the specified file!\nPlease select a valid file.", "OK");
                                return;
                            }
                            gcm_defaultSenderId = inputObj.project_info.project_number;
                            google_storage_bucket = inputObj.project_info.storage_bucket;
                            project_id = inputObj.project_info.project_id;
                            google_api_key = client.api_key[0].current_key;
                            google_crash_reporting_api_key = client.api_key[0].current_key;
                            google_app_id = client.client_info.mobilesdk_app_id;
                            default_web_client_id = client.oauth_client.Count > 0 ? client.oauth_client[0].client_id : string.Empty;
                        }
                    }
                    break;
            }
            EditorGUI.indentLevel = lastIndentLevel;
        }

        public override void OnPrebuild(FileManager fileManager)
        {
            if (activePushNotification)
            {
                fileManager.android.dependencies.Add("implementation 'com.google.firebase:firebase-messaging-ktx:23.0.7'");
            }

            fileManager.android.dependencies.Add("implementation 'com.google.firebase:firebase-analytics-ktx:21.1.1'");
            fileManager.android.dependencies.Add("implementation 'com.google.firebase:firebase-config-ktx:21.1.1'");
            fileManager.android.values.placeholders.Add($"<string name=\"gcm_defaultSenderId\" translatable=\"false\">{gcm_defaultSenderId}</string>");
            fileManager.android.values.placeholders.Add($"<string name=\"google_storage_bucket\" translatable=\"false\">{google_storage_bucket}</string>");
            fileManager.android.values.placeholders.Add($"<string name=\"project_id\" translatable=\"false\">{project_id}</string>");
            fileManager.android.values.placeholders.Add($"<string name=\"google_api_key\" translatable=\"false\">{google_api_key}</string>");
            fileManager.android.values.placeholders.Add($"<string name=\"google_crash_reporting_api_key\" translatable=\"false\">{google_crash_reporting_api_key}</string>");
            fileManager.android.values.placeholders.Add($"<string name=\"google_app_id\" translatable=\"false\">{google_app_id}</string>");
            fileManager.android.values.placeholders.Add($"<string name=\"default_web_client_id\" translatable=\"false\">{default_web_client_id}</string>");

            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonAnalyticsFirebaseSDK'");
            fileManager.ios.firebase.content = ios_plist;
        }

        /// <summary>
        /// Classes and fields are named same as the json file du to load correctly.
        /// NOTE: DO NOT change any class name or field name in this class.
        /// </summary>
        [System.Serializable]
        private class FireBaseJsonObject
        {
            public Project_info project_info;
            public List<Client> client;
            public string configuration_version;

            [System.Serializable]
            public class Project_info
            {
                public string project_number;
                public string project_id;
                public string storage_bucket;
            }

            [System.Serializable]
            public class Client
            {
                public Client_info client_info;
                public List<Oauth_client> oauth_client;
                public List<Api_key> api_key;
                public Services services;
                public string admob_app_id;

                [System.Serializable]
                public class Client_info
                {
                    public string mobilesdk_app_id;
                    public Android_client_info android_client_info;

                    [System.Serializable]
                    public class Android_client_info { public string package_name; }
                }

                [System.Serializable]
                public class Oauth_client
                {
                    public string client_id;
                    public int client_type;
                }

                [System.Serializable]
                public class Api_key { public string current_key; }

                [System.Serializable]
                public class Services
                {
                    public Appinvite_service appinvite_service;

                    [System.Serializable]
                    public class Appinvite_service
                    {
                        public List<Other_platform_oauth_client> other_platform_oauth_client;

                        [System.Serializable]
                        public class Other_platform_oauth_client
                        {
                            public string client_id;
                            public int client_type;
                        }
                    }
                }

            }
        }
    }

    [System.Serializable]
    public class Plugin_AppsFlyer : Plugin_AppId
    {
        public string appleAppId = string.Empty;
        public string endpointUrl = string.Empty;

        public override string Name => "AppsFlyer";
        public override string appIdLabel => "Dev Key";

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);

            if (active && platform == BuildTarget.iOS)
            {
                appleAppId = EditorGUILayout.TextField("Apple App Id", appleAppId);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Advertising attribution report endpoint URL");
                endpointUrl = EditorGUILayout.TextField("", endpointUrl);

            }
        }

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonAnalyticsAppsFlyerSDK'");
            fileManager.ios.planktonConfig.placeholders.Add($"\"plankton_appsflyer\": \"{appId}\"");
            fileManager.ios.planktonConfig.placeholders.Add($"\"appId\": \"{appleAppId}\"");
            fileManager.ios.planktonConfig.placeholders.Add($"\"NSAdvertisingAttributionReportEndpoint\": \"{endpointUrl}\"");

            fileManager.android.dependencies.Add("implementation 'com.appsflyer:af-android-sdk:6.12.2'");
            fileManager.android.dependencies.Add("implementation 'com.appsflyer:adrevenue:6.9.0'");
            fileManager.android.dependencies.Add("implementation 'com.appsflyer:purchase-connector:2.0.0'");
            fileManager.android.planktonConfig.placeholders.Add($"\"AppsFlyer\": {{ \"key\": \"{appId}\" }}");
        }
    }

    [System.Serializable]
    public class Plugin_Tenjin : Plugin_AppId
    {
        public override string Name => "Tenjin";
        public override string appIdLabel => "SDK Key";

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonAnalyticsTenjinSDK'");
            fileManager.ios.planktonConfig.placeholders.Add($"\"plankton_tenjin\": \"{appId}\"");
            fileManager.android.dependencies.Add("implementation 'com.tenjin:android-sdk:1.16.3'");
            fileManager.android.planktonConfig.placeholders.Add($"\"Tenjin\": {{ \"key\": \"{appId}\" }}");
        }
    }

    [System.Serializable]
    public class Plugin_AppMetrica : Plugin_AppId
    {
        public override string Name => "AppMetrica";
        public override string appIdLabel => "API Key";

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonAnalyticsAppMetricaSDK'");
            fileManager.ios.planktonConfig.placeholders.Add($"\"plankton_yandex\": \"{appId}\"");
            fileManager.android.dependencies.Add("implementation 'com.yandex.android:mobmetricalib:5.0.0'");
            fileManager.android.planktonConfig.placeholders.Add($"\"Yandex\": {{ \"key\": \"{appId}\" }}");
        }
    }

    [System.Serializable]
    public class Plugin_PlayServices : Plugin_AppId
    {
        public override string Name => "Google Play Services";

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.android.dependencies.Add("implementation('com.google.android.gms:play-services-games:23.1.0@aar') {transitive=true;}");
            fileManager.android.dependencies.Add("implementation 'com.google.android.gms:play-services-auth:20.7.0'");
            fileManager.android.launcherManifest.placeholders.Add("<meta-data android:name=\"com.google.android.gms.games.APP_ID\" android:value=\"@string/app_id\" />");
            fileManager.android.values.placeholders.Add($"<string name=\"app_id\" translatable=\"false\">{appId}</string>");
        }
    }

    [System.Serializable]
    public class Plugin_GameCenter : Plugin
    {
        public override string Name => "Game Center";

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonGameServicesSDK'");
        }
    }

    [System.Serializable]
    public class Plugin_AppTrackingTransparency : Plugin
    {
        public override string Name => "App Tracking Transparency";

        public string description = "We use your information in order to enhance your game experience, by serving you personalized ads and measuring the performance of our game.";

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);
            EditorGUILayout.HelpBox("A message that informs the user why an app is requesting permission to use data for tracking the user or the device. Required for iOS 14.5 and higher", MessageType.Info);
            if (active)
            {
                EditorGUILayout.LabelField("Tracking Usage Description");
                description = EditorGUILayout.TextArea(description, EditorStyles.textArea);
                if (string.IsNullOrEmpty(description))
                    EditorGUILayout.HelpBox("Default description will be used!", MessageType.Warning);
            }
        }

        public override void OnPrebuild(FileManager fileManager)
        {
            if (string.IsNullOrEmpty(description))
                description = "We use your information in order to enhance your game experience, by serving you personalized ads and measuring the performance of our game.";
            fileManager.ios.planktonConfig.placeholders.Add($"\"NSUserTrackingUsageDescription\":\"{description}\"");
        }
    }

    [System.Serializable]
    public class Plugin_Billing : Plugin
    {
        public override string Name => "In-App Purchase";

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);
        }

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonBillingSDK'");
#if MYKET
            fileManager.android.dependencies.Add("implementation 'com.github.myketstore:myket-billing-client:1.6'");
            fileManager.android.defenitions.Add("def marketApplicationId = \"ir.mservices.market\"");
            fileManager.android.defenitions.Add("def marketBindAddress = \"ir.mservices.market.InAppBillingService.BIND\"");
            fileManager.android.launcherTemplate.placeholders.Add("marketApplicationId:\"${marketApplicationId}\"");
            fileManager.android.launcherTemplate.placeholders.Add("marketBindAddress:\"${marketBindAddress}\"");
            fileManager.android.launcherTemplate.placeholders.Add("marketPermission:\"${marketApplicationId}.BILLING\"");
#elif BAZAAR
            fileManager.android.dependencies.Add("implementation 'com.github.cafebazaar.Poolakey:poolakey:2.2.0'");
#elif UNITY_ANDROID
            fileManager.android.dependencies.Add("implementation 'com.android.billingclient:billing-ktx:6.0.1'");
#endif
        }
    }

    [System.Serializable]
    public class Plugin_Metrix : Plugin_AppId
    {
        public string apiKey = string.Empty;
        public string sdkSignature = string.Empty;
        public bool developerMode = false;

        public override string Name => "Metrix";

        public override void OnEditorGUI(BuildTarget platform)
        {
#if METRIX
            base.OnEditorGUI(platform);
            if (active)
            {
                apiKey = EditorGUILayout.DelayedTextField("Api Key", apiKey);
                sdkSignature = EditorGUILayout.DelayedTextField("Sdk Signature", sdkSignature);
                developerMode = EditorGUILayout.Toggle("Developer Mode", developerMode);
            }
#endif
        }

        public override void OnPrebuild(FileManager fileManager)
        {
#if METRIX
            fileManager.android.dependencies.Add("implementation 'ir.metrix.attribution:metrix:2.1.0'");
            fileManager.android.dependencies.Add("implementation 'ir.metrix.analytics:metrix:2.1.0'");

            var devmode = developerMode ? "true" : "false";
            fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"ir.metrix.APPLICATION_ID\" android:value=\"{appId}\"/>");
            fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"ir.metrix.API_KEY\" android:value=\"{apiKey}\"/>");
            fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"metrix_developer_mode\" android:value=\"{devmode}\"/>");

            if (sdkSignature.Length > 4)
                fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"ir.metrix.SIGNATURE\" android:value=\"{sdkSignature}\"/>");
#endif
        }
    }

    [System.Serializable]
    public class Plugin_GameAnalytics : Plugin
    {
        public override string Name => "Game Analytics";

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonGameAnalyticsSDK'");
            fileManager.android.dependencies.Add("implementation 'com.gameanalytics.sdk:gameanalytics-android:6.4.2'");
            fileManager.android.repositories.Add("maven { url 'https://maven.gameanalytics.com/release' }");
        }
    }

    [System.Serializable]
    public class Plugin_UMP : Plugin
    {
        public string adMobAppId = string.Empty;

        public override string Name => "Google UMP";

        public override void OnEditorGUI(BuildTarget platform)
        {
            base.OnEditorGUI(platform);

            if (active)
                adMobAppId = EditorGUILayout.TextField("AdMob App Id", adMobAppId);
        }

        public override void OnPrebuild(FileManager fileManager)
        {
            fileManager.ios.podfile.placeholders.Add("pod 'PlanktonConsentManagerSDK'");
            fileManager.ios.planktonConfig.placeholders.Add($"\"GADApplicationIdentifier\": \"{adMobAppId}\"");
            fileManager.android.launcherManifest.placeholders.Add($"<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"{adMobAppId}\"/>");
            fileManager.android.dependencies.Add("implementation('com.google.android.ump:user-messaging-platform:2.1.0@aar') {transitive=true;}");
        }
    }
}