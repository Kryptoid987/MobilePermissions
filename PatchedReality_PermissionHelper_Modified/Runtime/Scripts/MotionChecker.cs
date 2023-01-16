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

            //Enable and check motion
            InputSystem.EnableDevice(StepCounter.current);
            int maxWait = 20;
            // Wait until service initializes - really? up to 20 seconds?
            while (!StepCounter.current.IsActuated() && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                Debug.Log("Took more than 20 seconds - assume failure...");
                InputSystem.DisableDevice(StepCounter.current);
                failureCallback?.Invoke(PermissionTypeAsString);
                yield break;
            }

            bool haveServicePermissions = false;
            if (StepCounter.current.IsActuated())
            {
                haveServicePermissions = true;
            }

            //in any case, stop.
            InputSystem.DisableDevice(StepCounter.current);

            if (haveServicePermissions)
            {
                Debug.Log("Motion services started");
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
