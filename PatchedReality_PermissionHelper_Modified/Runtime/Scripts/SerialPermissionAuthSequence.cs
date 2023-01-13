using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobilePermissions.iOS
{
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    using PermissionStatus = PermissionsHelperPlugin.PermissionStatus;
    /*
        A lil helper class that walks through a sequence of permissions, requesting them 
        if need be/possible, and then reports when sequence is done. The result, ffrom the user
        perspective will be a series of OS dialogs. 
     */
    public class SerialPermissionAuthSequence
    {
        protected List<PermissionType> permissionsInOrder;
        protected Dictionary<PermissionType, PermissionStatus> currentPermissions;
        protected System.Action flowCompleteAction;

        protected bool flowActive = false;
        public SerialPermissionAuthSequence(List<PermissionType> permissionsInOrder)
        {
            this.permissionsInOrder = permissionsInOrder;
        }

        ~SerialPermissionAuthSequence()
        {
            if (flowActive)
            {
                PermissionsHelperPlugin.OnPermissionStatusUpdated -= HandlePermissionUpdated;
            }
        }
        public void Start(System.Action flowCompleteAction)
        {
            Debug.Log("Starting sequence with perms: " + permissionsInOrder.Count.ToString());
            this.flowCompleteAction = flowCompleteAction;
            currentPermissions = (new CollectivePermissionsStatus(permissionsInOrder)).GetFullStatuses();
            PermissionsHelperPlugin.OnPermissionStatusUpdated += HandlePermissionUpdated;
            flowActive = true;
            NextPermission();
        }

        void NextPermission()
        {
            Debug.Log("In trigger next perm: ");
            for(int i=0; i < permissionsInOrder.Count; i++)
            {
                var permission = permissionsInOrder[i];
                if (currentPermissions[permission] ==
                    PermissionStatus.PRPermissionStatusUnknown || 
                    currentPermissions[permission] == PermissionStatus.PRPermissionStatusUnknownPermission)
                {
                    Debug.Log("Choosing perm: " + permission.ToString());
                    //increment current and ask for it.
                    PermissionsHelperPlugin.Instance.RequestPermission(permission);
                    return;
                }
            }

            //if we get down here, we are all done.
            flowCompleteAction?.Invoke();
            
        }
        void HandlePermissionUpdated(PermissionType permission, bool success)
        {
            //update our dictionary, if need be.
            if (currentPermissions.ContainsKey(permission))
            {
                currentPermissions[permission] = success ? PermissionStatus.PRPermissionStatusAuthorized :
                                                                PermissionStatus.PRPermissionStatusDenied;
            }

            Debug.Log("Got perm callback for: " + permission.ToString() + " with result: " + success.ToString());
            NextPermission();
        }

    }

}
