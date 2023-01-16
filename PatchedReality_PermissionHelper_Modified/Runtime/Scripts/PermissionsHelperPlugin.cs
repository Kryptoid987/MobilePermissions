using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MobilePermissions.iOS
{
    //todo: should be made into a singleton...
    public class PermissionsHelperPlugin : Singleton<PermissionsHelperPlugin>
    {
        public delegate void PermissionRequestStartDelegate(PermissionType permission);
        public static PermissionRequestStartDelegate OnPermissionRequestStarted;
        public delegate void PermissionStatusUpdatedDelegate(PermissionType permission, bool success);
        public static PermissionStatusUpdatedDelegate OnPermissionStatusUpdated;

        public enum PermissionType
        {
            PRCameraPermissions = 0,
            PRMicrophonePermissions = 1,
            PRLocationWhileUsingPermissions = 2,
            PRSpeechRecognitionPermissions = 3,
            PRMotionUsagePermissions = 4,
            PRPermissionTypeUnknown = 255
        }
        public enum PermissionStatus
        {
            PRPermissionStatusUnknown = 0,
            PRPermissionStatusAuthorized = 1,
            PRPermissionStatusDenied = 2,
            PRPermissionStatusRestricted = 3,
            PRPermissionStatusUnknownPermission = 255
        }

        protected PermissionsHelperPlugin() { }

        public List<PermissionType> RequiredPermissions
        {
            get
            {
                return requiredPermissions;
            }
        }
        protected List<PermissionType> requiredPermissions = new List<PermissionType>();

        /*
            Use this method to specify which permissions are required, in order.
            used in various high level apis to get collective state, 
            request all permissions, etc. Usually good to call this in the boot strap of your
            app.
         */
        public void SetRequiredPermissions(List<PermissionType> inOrderList)
        {
            requiredPermissions.Clear();
            requiredPermissions.AddRange(inOrderList);
        }

        public CollectivePermissionsStatus.CollectiveState GetCollectiveState()
        {
            //returns collective state for required permissions.
            return (new CollectivePermissionsStatus(requiredPermissions)).GetCurrentState();
            
        }

        
        public void RequestPermission(PermissionType permission)
        {
            OnPermissionRequestStarted?.Invoke(permission);
            //note: location is special, because requesting it a a bit fancy, we have a special object to handle that.
            if (permission.Equals(PermissionType.PRLocationWhileUsingPermissions))
            {
                RequestLocationPermissions();
                return;
            }
            //note: same deal with motion usage
            else if (permission.Equals(PermissionType.PRLocationWhileUsingPermissions))
            {
                //var test = Input.acceleration.magnitude;

                //if stepcounter.current = null, not supported, return denied. Do error not supported

                UnityEngine.InputSystem.InputSystem.EnableDevice(UnityEngine.InputSystem.StepCounter.current);


            }

            _requestPermission((int)permission, this.gameObject.name, "PermissionRequestSuccess", "PermissionRequestFailure");
        }

        public void OpenSettings()
        {
            _openSettings();
        }

        public PermissionStatus GetPermissionStatus(PermissionType permission)
        {
            return (PermissionStatus)_getPermissionStatus((int)permission);
        }

        void RequestLocationPermissions()
        {
            PermissionStatus currStatus = this.GetPermissionStatus(PermissionType.PRLocationWhileUsingPermissions);
            if (currStatus.Equals(PermissionStatus.PRPermissionStatusDenied) || currStatus.Equals(PermissionStatus.PRPermissionStatusRestricted))
            {
                //fail immediately, no reason to bother checking.
                this.PermissionRequestFailure(((int)PermissionType.PRLocationWhileUsingPermissions).ToString());
                return;
            }
            else if (currStatus.Equals(PermissionStatus.PRPermissionStatusAuthorized))
            {
                //succeed immediately, they have already granted what we need.
                this.PermissionRequestSuccess(((int)PermissionType.PRLocationWhileUsingPermissions).ToString());
                return;
            }

            //Location servies only can be initiated with the old input system, we need to breifly turn on location services to force the location permission request prompt.
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Debug.LogError("New Input System currently cannot enable location services which is required to get the permission request prompt to actually show up for location. Returning failure.");
            this.PermissionRequestFailure(((int)PermissionType.PRLocationWhileUsingPermissions).ToString());
            return;
#endif

            //otherwise, have our little location helper find out whats what.
            LocationHelper.RequestLocationPermissions(this.PermissionRequestSuccess, this.PermissionRequestFailure);
        }
        void PermissionRequestSuccess(string permissionType)
        {
            int permAsInt;
            PermissionType type = PermissionType.PRPermissionTypeUnknown;
            if (int.TryParse(permissionType, out permAsInt))
            {
                type = (PermissionType)permAsInt;
            }

            Debug.Log("Got success callback from native with: " + type.ToString());
            OnPermissionStatusUpdated?.Invoke(type, true);
        }

        void PermissionRequestFailure(string permissionType)
        {
            int permAsInt;
            PermissionType type = PermissionType.PRPermissionTypeUnknown;
            if (int.TryParse(permissionType, out permAsInt))
            {
                type = (PermissionType)permAsInt;
            }
            Debug.Log("Got failure callback from native with: " + type.ToString());
            OnPermissionStatusUpdated?.Invoke(type, false);
        }

        LocationChecker LocationHelper
        {
            get
            {
                if (locationHelper == null)
                {
                    locationHelper = GetComponentInChildren<LocationChecker>();
                    if (locationHelper == null)
                    {
                        //then make it.
                        GameObject go = new GameObject("LocationPermissionChecker");
                        go.transform.SetParent(this.transform);
                        locationHelper = go.AddComponent<LocationChecker>();

                    }
                }
                return locationHelper;
            }
        }
        LocationChecker locationHelper;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport ("__Internal")]
        private static extern void _requestPermission (int permissionType, string gameObject, string successCallback, string failureCallback);

        [DllImport ("__Internal")]
        private static extern int _getPermissionStatus(int permissionType);
        
        [DllImport ("__Internal")]
        private static extern void _openSettings();
#else
        private static void _requestPermission(int permissionType, string gameObject, string successCallback, string failureCallback)
        {
            Debug.Log("request started " + permissionType.ToString());
            //simulate the request taking time...
            GameObject go = GameObject.Find(gameObject);
            if (go != null)
            {
                go.SendMessage(failureCallback, permissionType.ToString());
            }
        
            
        }

        private static int _getPermissionStatus(int permissionType)
        {
            return (int)PermissionStatus.PRPermissionStatusUnknownPermission;
        }


        private static void _openSettings()
        {
            UnityEngine.Debug.LogWarning("Open Settings  - platform unsupported");
        }

        
#endif

    }
}
