using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MobilePermissions.iOS.Examples
{
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    using PermissionStatus = PermissionsHelperPlugin.PermissionStatus;
    public class OneLinePermissionStatusUI : MonoBehaviour
    {
        [System.Serializable]
        public class IconChoice
        {
            public List<PermissionStatus> ForStatuses;
            public Sprite Visual;

            public Color Tint;
        }

        [SerializeField] protected List<IconChoice> IconsForStatus;

        [SerializeField] protected PermissionType MyPermission;
        [SerializeField] protected UnityEngine.UI.Image StatusSymbol;
        [SerializeField] protected UnityEngine.UI.Image Icon;
        [SerializeField] protected GameObject RequestInProgressSymbol;

        [SerializeField] protected bool HideIconWhenStatusSet = true;
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            //update icon appropriately.
            RequestInProgressSymbol.SetActive(false);
            PermissionsHelperPlugin.OnPermissionStatusUpdated += HandlePermissionChanged;
            PermissionsHelperPlugin.OnPermissionRequestStarted += HandlePremissionRequestStarted;
            UpdateIcon();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            PermissionsHelperPlugin.OnPermissionStatusUpdated -= HandlePermissionChanged;
            PermissionsHelperPlugin.OnPermissionRequestStarted -= HandlePremissionRequestStarted;
        }

        void HandlePremissionRequestStarted(PermissionType permission)
        {
            if(MyPermission.Equals(permission))
            {
                Debug.Log("in perms started for perm: " + permission.ToString());
                StatusSymbol.gameObject.SetActive(false);
                if(HideIconWhenStatusSet)
                {
                    Icon.gameObject.SetActive(false);
                }
                RequestInProgressSymbol.SetActive(true);
            }
        }
        void HandlePermissionChanged(PermissionType permission, bool success)
        {
            if (MyPermission.Equals(permission))
            {
                UpdateIcon();
            }
        }

        void UpdateIcon()
        {
            //get status for our perm, and update icon accordingly.
            Debug.Log("in perms updated for perm: " + MyPermission.ToString());
            PermissionStatus status = PermissionsHelperPlugin.Instance.GetPermissionStatus(MyPermission);
             RequestInProgressSymbol.SetActive(false);
            // Debug.Log(
            //     string.Format("Got status {0} for permission {1}",
            //     status.ToString(), MyPermission.ToString()));
            var choice = ChoiceForStatus(status);
            if (choice == null)
            {
                //just hide the icon.
                StatusSymbol.gameObject.SetActive(false);
                Icon.gameObject.SetActive(true);
            }
            else
            {
                StatusSymbol.gameObject.SetActive(true);
                StatusSymbol.sprite = choice.Visual;
                StatusSymbol.color = choice.Tint;
                if (HideIconWhenStatusSet)
                {
                    Icon.gameObject.SetActive(false);
                }
            }

        }

        IconChoice ChoiceForStatus(PermissionStatus status)
        {
            foreach (IconChoice choice in IconsForStatus)
            {
                if (choice.ForStatuses.Contains(status))
                {
                    return choice;
                }
            }

            return null;
        }

    }
}
