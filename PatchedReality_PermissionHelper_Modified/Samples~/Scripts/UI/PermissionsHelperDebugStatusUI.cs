using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobilePermissions.iOS.Examples
{
    #if UNITY_IOS
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    using PermissionStatus = PermissionsHelperPlugin.PermissionStatus;
    #endif

    /**
    Utility UI to show status of a permission in associated ui text field.
    */
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class PermissionsHelperDebugStatusUI : MonoBehaviour
    {
        [SerializeField] protected PermissionType Permission;

#if UNITY_IOS
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            PermissionsHelperPlugin.OnPermissionStatusUpdated += HandlePermissionRequestStatusChange;
            UpdateTextStatusFromPlugin();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            PermissionsHelperPlugin.OnPermissionStatusUpdated -= HandlePermissionRequestStatusChange;
        }
#endif
        void OnApplicationFocus(bool hasFocus)
        {
            //ask plugin manager about our permissions, since it might have changed.
            //TODO: This seems to trigger a crash when coming back from settings...but only when we have changed a status.
            //Investigate!
            UpdateTextStatusFromPlugin();
        }

        void HandlePermissionRequestStatusChange(PermissionType permission, bool result)
        {
            if (permission.Equals(Permission))
            {
                //query plugin to get actual status - really only needed when result is negative,
                //but cleaner to do it every time could be declined or restricted.
                UpdateTextStatusFromPlugin();
            }
        }

        void UpdateTextStatusFromPlugin()
        {
#if UNITY_IOS
            PermissionStatus status = PermissionsHelperPlugin.Instance.GetPermissionStatus(Permission);
            TextField.text = string.Format("{0}:\n{1}", Permission.ToString(), status.ToString());
#endif
        }

        UnityEngine.UI.Text TextField
        {
            get
            {
                if (textField == null)
                {
                    textField = GetComponent<UnityEngine.UI.Text>();
                }
                return textField;
            }
        }
        UnityEngine.UI.Text textField;
    }

}
