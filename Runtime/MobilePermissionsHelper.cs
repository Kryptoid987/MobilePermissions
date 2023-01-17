using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using MobilePermissions.iOS;
using MobilePermissions.OpeniOSSettings;
#endif


namespace MobilePermissions
{
    public enum PermissionType
    {
        Camera,
        Microphone,
        Location,
        MotionUsage,
    }

    public enum AuthStatus
    {
        Authorized,
        Restricted,
        Denied,
        DeniedForever, 
        Unknown
    }

    // NOTE ANDROID: This must be in the android manifest to use
    //<uses-permission android:name="android.permission.ACTIVITY_RECOGNITION"/> //Pedometer
    //<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"/> //Location
    //<uses-permission android:name="android.permission.RECORD_AUDIO"/> //Microphone
    //<uses-permission android:name="android.permission.CAMERA"/> //Camera

    // NOTE ANDROID: Permission prompt on device:
    // Allow Always = 'Allow Always'
    // Allow Once = 'Allow Always' until the app is closed and screen sleeped. *If app is still open when screen is sleeped than the app is still running and doesnt require another permission run
    // Dont Allow = 'Dont Allow' Once, on second Dont Allow it = 'Dont Allow Forever' and wont show permission screene ever again
    // Requesting Permission on a dont allow, or dont allow forever state will still return 'Dont Allow' and 'Dont Allow Forever', use these
    //  accordingly to know how to react. AKA if its dont allow always we know the permission screen never showed up and the user needs to go to settings to allow

    public class MobilePermissionsHelper : MonoBehaviour
    {
#if UNITY_ANDROID
        //Android permissions
        PermissionCallbacks callbacks;
#endif
        private Action<AuthStatus> OnPermissionChangedCallback;
        private bool isRequestingPermission = false;

        public AuthStatus HasPermission(PermissionType permission)
        {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(GetAndroidPermissionString(permission)) ? AuthStatus.Authorized : AuthStatus.Denied;
#elif UNITY_IOS
            switch (PermissionsHelperPlugin.Instance.GetPermissionStatus(GetiOSPermissionEnum(permission)))
            {
                case PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusAuthorized:
                    return AuthStatus.Authorized;
                case PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusDenied:
                    return AuthStatus.Denied;
                case PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusRestricted:
                    return AuthStatus.Restricted;
                case PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusUnknown:
                    return AuthStatus.Unknown;
                case PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusUnknownPermission:
                    return AuthStatus.Unknown;
                default:
                    return AuthStatus.Unknown;
            }
#else
            return AuthStatus.Unknown;
#endif
        }
        public void OpenAppSettings()
        {
#if UNITY_ANDROID
            try
            {
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject =
                       unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    string packageName = currentActivityObject.Call<string>("getPackageName");

                    using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                    using (AndroidJavaObject uriObject =
                           uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                    using (var intentObject = new AndroidJavaObject("android.content.Intent",
                               "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                    {
                        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                        currentActivityObject.Call("startActivity", intentObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
#elif UNITY_IOS
            AppSettingsIOSNativeBindings.OpenSettings();
#endif
        }

        /// <summary>
        /// Returns false if a permission request is already in progress
        /// </summary>
        /// <param name="permissionType"></param>
        /// <param name="OnPermissionChangedCallback"></param>
        /// <returns></returns>
        public bool RequestPermission(PermissionType permissionType, Action<AuthStatus> OnPermissionChangedCallback)
        {
            if (isRequestingPermission)
            {
                Debug.LogWarning("Permission is currently already being requested.");
                return false;
            }

#if UNITY_ANDROID || UNITY_IOS
            this.OnPermissionChangedCallback = OnPermissionChangedCallback;
            isRequestingPermission = true;
#endif

#if UNITY_ANDROID
            callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += Callbacks_AndroidPermissionGranted;
            callbacks.PermissionDenied += Callbacks_AndroidPermissionDenied;
            callbacks.PermissionDeniedAndDontAskAgain += Callbacks_PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission(GetAndroidPermissionString(permissionType), callbacks);
#elif UNITY_IOS
            PermissionsHelperPlugin.OnPermissionStatusUpdated += OniOSPermissionUpdated;
            PermissionsHelperPlugin.Instance.RequestPermission(GetiOSPermissionEnum(permissionType));
#else
            OnPermissionChangedCallback?.Invoke(AuthStatus.Unknown);
#endif
            return true;
        }

        #region Android
        private static string GetAndroidPermissionString(PermissionType type)
        {
            return type switch
            {
                PermissionType.Camera => "android.permission.CAMERA",
                PermissionType.MotionUsage => "android.permission.ACTIVITY_RECOGNITION",
                PermissionType.Microphone => "android.permission.RECORD_AUDIO",
                PermissionType.Location => "android.permission.ACCESS_FINE_LOCATION",
                _ => throw new ArgumentOutOfRangeException(type.ToString() + " not a proper type"),
            };
        }

        //Enable pedometer after being granted the proper android permissions
        private void Callbacks_AndroidPermissionGranted(string obj)
        {
            isRequestingPermission = false;
            OnPermissionChangedCallback?.Invoke(AuthStatus.Authorized);
            UnsubAndroidCallbacks();
        }

        //Log error and possibly react to needed permissions being denied;
        private void Callbacks_AndroidPermissionDenied(string obj)
        {
            isRequestingPermission = false;
            OnPermissionChangedCallback?.Invoke(AuthStatus.Denied);
            UnsubAndroidCallbacks();
        }

        private void Callbacks_PermissionDeniedAndDontAskAgain(string obj)
        {
            isRequestingPermission = false;
            OnPermissionChangedCallback?.Invoke(AuthStatus.DeniedForever);
            UnsubAndroidCallbacks();
        }

        private void UnsubAndroidCallbacks()
        {
#if UNITY_ANDROID
            if (callbacks != null)
            {
                callbacks.PermissionGranted -= Callbacks_AndroidPermissionGranted;
                callbacks.PermissionDenied -= Callbacks_AndroidPermissionDenied;
                callbacks.PermissionDeniedAndDontAskAgain -= Callbacks_PermissionDeniedAndDontAskAgain;
            }
#endif
        }
#endregion

        #region iOS
#if UNITY_IOS
        private void OniOSPermissionUpdated(PermissionsHelperPlugin.PermissionType permisson, bool success)
        {
            PermissionsHelperPlugin.OnPermissionStatusUpdated -= OniOSPermissionUpdated;
            isRequestingPermission = false;
            OnPermissionChangedCallback?.Invoke(success ? AuthStatus.Authorized : AuthStatus.DeniedForever);
        }

        private static PermissionsHelperPlugin.PermissionType GetiOSPermissionEnum(PermissionType permissionType)
        {
            return permissionType switch
            {
                PermissionType.Camera => PermissionsHelperPlugin.PermissionType.PRCameraPermissions,
                PermissionType.Pedometer => PermissionsHelperPlugin.PermissionType.PRMotionUsagePermissions,
                PermissionType.Microphone => PermissionsHelperPlugin.PermissionType.PRMicrophonePermissions,
                PermissionType.Location => PermissionsHelperPlugin.PermissionType.PRLocationWhileUsingPermissions,
                _ => throw new ArgumentOutOfRangeException(permissionType.ToString() + " not a proper type"),
            };
        }
#endif
#endregion

        private void OnDestroy()
        {
            UnsubAndroidCallbacks();
#if UNITY_IOS
            PermissionsHelperPlugin.OnPermissionStatusUpdated -= OniOSPermissionUpdated;
#endif
        }
    }
}