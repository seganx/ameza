using System;
using UnityEngine;
using Plankton.Proxy;

namespace Plankton
{
    public static class GameServices
    {
        private const string logName = "[Plankton] [GameServices]";
        private static ProxyGameServices proxyGameServices = new ProxyGameServices();

        public static bool IsSignedIn { get; private set; } = false;

        public static void SignIn(Action<SignResult> callback, bool androidForceInteractive = false, string androidServerClientId = "")
        {
            Debug.Log($"{logName} Signing in play services!");
            Callbacks.onPlayServiceSignInResult = json =>
            {
                try
                {
                    var result = JsonUtility.FromJson<SignResult>(json);
                    IsSignedIn = result.success;
                    Debug.Log($"{logName} Signing in play services result: success={result.success}, userId={result.user_id}, email={result.email}, displayName={result.displayName}, serverAuthCode={result.serverAuthCode}, IdToken={result.token}, iosPublicKeyUrl={result.iosPublicKeyUrl}, iosTimestamp={result.iosTimestamp}, iosSalt={result.iosSalt}, iosSignature={result.iosSignature}");
                    callback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    callback?.Invoke(new SignResult());
                }
            };
            proxyGameServices.SignIn(androidForceInteractive, androidServerClientId);
#if UNITY_EDITOR
            callback?.Invoke(new SignResult());
#endif
        }

        public static void SignOut(Action<bool> callback)
        {
            Debug.Log($"{logName} Signing out play services!");
            Callbacks.onPlayServiceSignOutResult = json =>
            {
                try
                {
                    var result = JsonUtility.FromJson<SignResult>(json);
                    IsSignedIn = result.success != true;
                    Debug.Log($"{logName} Signing out play services result: {json}");
                    callback?.Invoke(result.success);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    callback?.Invoke(false);
                }
            };
            proxyGameServices.SignOut();
#if UNITY_EDITOR
            callback?.Invoke(false);
#endif
        }

        public static void Load(string fileName, Action<string> onCompleted)
        {
            Debug.Log($"{logName} Loading from play services...");

            Callbacks.onPlayServiceLoadData = json =>
            {
                Debug.Log($"{logName} Data loaded from play services. json: {json}");
                try
                {
                    onCompleted?.Invoke(json);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    onCompleted?.Invoke(null);
                }
            };

            if (IsSignedIn)
            {
                proxyGameServices.Load(fileName);
#if UNITY_EDITOR
                onCompleted?.Invoke(string.Empty);
#endif
            }
            else onCompleted?.Invoke(string.Empty);
        }

        public static void Save(string fileName, string data, string description, Action<bool> callback)
        {
            Debug.Log($"{logName} Save data to play services. signedIn = {IsSignedIn}");

            Callbacks.onPlayServiceSaveResult = success =>
            {
                Debug.Log($"{logName} Save data to play services result = {success}");
                callback?.Invoke(success);
            };

            if (IsSignedIn)
            {
                proxyGameServices.Save(fileName, data, "Automated saved data");
#if UNITY_EDITOR
                callback?.Invoke(true);
#endif
            }
            else callback?.Invoke(false);
        }

        //////////////////////////////////////////////////////
        /// HELPER CLASSES
        //////////////////////////////////////////////////////
        public class SignResult
        {
            public bool success = false;
            public string token = string.Empty;
            public string serverAuthCode = string.Empty;
            public string user_id = string.Empty;
            public string email = string.Empty;
            public string displayName = string.Empty;
            public string iosPublicKeyUrl = string.Empty;
            public string iosSignature = string.Empty;
            public string iosSalt = string.Empty;
            public long iosTimestamp = 0;
        }
    }
}