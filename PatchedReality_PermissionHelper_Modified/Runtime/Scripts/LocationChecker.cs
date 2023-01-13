using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MobilePermissions.iOS
{
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    public class LocationChecker : MonoBehaviour
    {
        System.Action<string> successCallback;
        System.Action<string> failureCallback;

        /**
            will cause lcoation permissions to be asked for if not already asked for successfully.
         */
        public void RequestLocationPermissions( System.Action<string> successCallback,  
                                                System.Action<string> failureCallback)
        {
            this.successCallback = successCallback;
            this.failureCallback = failureCallback;
            StopAllCoroutines();
            StartCoroutine(StartLocationServicesIfPossible());
        }

        string PermissionTypeAsString
        {
            get
            {
                return ((int)PermissionType.PRLocationWhileUsingPermissions).ToString();
            }
        }

        IEnumerator StartLocationServicesIfPossible()
        {
            Debug.Log("in start location services..");
           
            //following based on unity example docs...seems like this is the way they want us to check this stuff..
            //so...
            Input.location.Start();
              // Wait until service initializes - really? up to 20 seconds?
            int maxWait = 20;
            Debug.Log("Started services - will check for 20 seconds and see if they init");
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                Debug.Log("Took more than 20 seconds - assume failure...");
                failureCallback?.Invoke(PermissionTypeAsString);
                yield break;
            }

            bool haveServicePermissions = false;
            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Running)
            {
                haveServicePermissions = true;
            }

             //in any case, stop.
            Input.location.Stop();

            if(haveServicePermissions)
            {
                 Debug.Log("We have access to services. Hurray! " + Input.location.status.ToString() + " -> " + 
                        Input.location.lastData.latitude.ToString() + ", " + Input.location.lastData.longitude.ToString());
                successCallback?.Invoke(PermissionTypeAsString);

            }
            else
            {
                Debug.Log("Location services could not be started, assuming failure.");
                failureCallback?.Invoke(PermissionTypeAsString);
            }
           
           
            
        }
    }
}
