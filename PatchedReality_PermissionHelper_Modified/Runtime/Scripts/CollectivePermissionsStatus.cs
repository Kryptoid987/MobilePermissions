using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobilePermissions.iOS
{

    using PermissionType = PermissionsHelperPlugin.PermissionType;
    using PermissionStatus = PermissionsHelperPlugin.PermissionStatus;
    /**
        In various points in a flow, one may want to collect all needed permissions and see what status they have as a group.
        This class helps you do that neatly.
     */
    public class CollectivePermissionsStatus
    {
        public enum CollectiveState
        {
            AllAuthorized = 0,//all perms are in authorized state.
            AllAsked = 1,//all perms have been asked for, but are *not* in authorized state. 
            SomeUnknown,//at least some of the perms are in unknown state, which means you will want to request permission for em.

            AllUnknown,//all perms in unknown state. You should ask about all of em.
        }

        //permssions in this collective status
        protected List<PermissionType> permissions;



        public CollectivePermissionsStatus(List<PermissionType> permissions)
        {
            this.permissions = permissions;
        }

        /**
        Returns a dictionary with status of each perm type in our collective.
        NOTE: we explicitly *dont* want to cache permissions statuses, as these can change outside lifecycle of the app.
        and could cause problem for long lived objects
         */
        public Dictionary<PermissionType, PermissionStatus> GetFullStatuses()
        {

            Dictionary<PermissionType, PermissionStatus> stats = new Dictionary<PermissionType, PermissionStatus>();
            foreach (PermissionType type in permissions)
            {
                stats.Add(type, PermissionsHelperPlugin.Instance.GetPermissionStatus(type));
            }
            return stats;

        }

        //returns enum that describes that status of the group.
        public CollectiveState GetCurrentState()
        {

            var stats = GetFullStatuses();
            int authorizedCount = 0;
            int unknownCount = 0;
            int declinedCount = 0;
            foreach (KeyValuePair<PermissionType, PermissionStatus> entry in stats)
            {
                switch (entry.Value)
                {
                    case PermissionStatus.PRPermissionStatusAuthorized:
                        {
                            authorizedCount++;
                            break;
                        }
                    case PermissionStatus.PRPermissionStatusDenied:
                    case PermissionStatus.PRPermissionStatusRestricted:
                        {
                            declinedCount++;
                            break;
                        }
                    case PermissionStatus.PRPermissionStatusUnknown:
                        {
                            unknownCount++;
                            break;
                        }
                }
            }
            
            if (authorizedCount == stats.Count)
            {
                return CollectiveState.AllAuthorized;
            }
            else if (unknownCount == stats.Count)
            {
                return CollectiveState.AllUnknown;
            }
            else if (authorizedCount + declinedCount == stats.Count)
            {
                return CollectiveState.AllAsked;
            }

            //otherwise, at least some are unknown, but not all.
            return CollectiveState.SomeUnknown;
        }


    }

}
