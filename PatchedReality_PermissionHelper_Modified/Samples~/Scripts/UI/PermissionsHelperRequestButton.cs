using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobilePermissions.iOS.Examples
{
#if UNITY_IOS
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    using PermissionStatus = PermissionsHelperPlugin.PermissionStatus;
#endif
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class PermissionsHelperRequestButton : MonoBehaviour
    {

        [SerializeField] protected PermissionType Permission;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            Button.onClick.AddListener(HandleButtonClick);
        }

#if UNITY_IOS
        void OnEnable()
        {
            PermissionsHelperPlugin.Instance.SetRequiredPermissions(new List<PermissionType> { PermissionType.PRSpeechRecognitionPermissions });

            PermissionsHelperPlugin.OnPermissionStatusUpdated += HandlePermissionRequestStatusChange;
            PermissionsHelperPlugin.Instance.GetPermissionStatus(PermissionType.PRCameraPermissions);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            PermissionsHelperPlugin.OnPermissionStatusUpdated -= HandlePermissionRequestStatusChange;
        }
#endif
        void HandlePermissionRequestStatusChange(PermissionType permission, bool result)
        {
            Debug.Log(permission + "   " + result);
            if (Permission.Equals(permission))
            {
                UpdateButtonState(result);
            }
        }

        void UpdateButtonState(bool enabled)
        {
            this.Button.interactable = !enabled;
        }

        void HandleButtonClick()
        {
#if UNITY_IOS
            PermissionsHelperPlugin.Instance.RequestPermission(this.Permission);
            //PermissionsHelperPlugin.Instance.
#endif
        }

        UnityEngine.UI.Button Button
        {
            get
            {
                if (button == null)
                {
                    button = GetComponent<UnityEngine.UI.Button>();
                }
                return button;
            }
        }
        UnityEngine.UI.Button button;
    }

}
