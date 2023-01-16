using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace MobilePermissions.iOS
{
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    public class MotionChecker : MonoBehaviour
    {
        System.Action<string> successCallback;
        System.Action<string> failureCallback;

        /**
            will cause lcoation permissions to be asked for if not already asked for successfully.
         */
        public void RequestMotionPermissions(System.Action<string> successCallback,
                                                System.Action<string> failureCallback)
        {
            this.successCallback = successCallback;
            this.failureCallback = failureCallback;
            StopAllCoroutines();
            StartCoroutine(StartMotionUsageIfPossible());
        }

        string PermissionTypeAsString
        {
            get
            {
                return ((int)PermissionType.PRMotionUsagePermissions).ToString();
            }
        }

        IEnumerator StartMotionUsageIfPossible()
        {
            Debug.Log("in start motion services..");

            //Return failed rigth away if motion unsupported
            if(StepCounter.current == null)
            {
                Debug.LogError("System does not support motion usage");
                failureCallback?.Invoke(PermissionTypeAsString);
                yield break;
            }

            //Get current motion permission status, return rigth away if authorized or denied
            var currentPermissionStatus = PermissionsHelperPlugin.Instance.GetPermissionStatus(PermissionType.PRMotionUsagePermissions);
            if(currentPermissionStatus == PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusAuthorized)
            {
                successCallback?.Invoke(PermissionTypeAsString);
                yield break;
            }
            else if(currentPermissionStatus == PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusDenied || currentPermissionStatus == PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusRestricted)
            {
                failureCallback?.Invoke(PermissionTypeAsString);
                yield break;
            }

            //Enable and check motion
            InputSystem.EnableDevice(StepCounter.current);
            int maxWait = 30;
            while ((PermissionsHelperPlugin.Instance.GetPermissionStatus(PermissionType.PRMotionUsagePermissions) == PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusUnknown ||
                PermissionsHelperPlugin.Instance.GetPermissionStatus(PermissionType.PRMotionUsagePermissions) == PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusUnknown) && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            //in any case, stop.
            InputSystem.DisableDevice(StepCounter.current);

            if(PermissionsHelperPlugin.Instance.GetPermissionStatus(PermissionType.PRMotionUsagePermissions) == PermissionsHelperPlugin.PermissionStatus.PRPermissionStatusAuthorized)
            {
                Debug.Log("Motion services authorized");
                successCallback?.Invoke(PermissionTypeAsString);
            }
            else
            {
                Debug.Log("Motion services could not be started, assuming failure.");
                failureCallback?.Invoke(PermissionTypeAsString);
            }
        }
    }
}
