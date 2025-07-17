using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MobilePermissions.iOS.Examples
{
    #if UNITY_IOS
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    using PermissionStatus = PermissionsHelperPlugin.PermissionStatus;
    using CollectiveState = CollectivePermissionsStatus.CollectiveState;
    #endif
    //manage overall screen state for the permissions UI - mainly messaging the player appropriately.
    public class AllAtOncePermissionsUI : MonoBehaviour
    {
        //TODO: this list appears on a couple components. think of a way
        //to just set it in one place..
        [Tooltip("In order list of permissions we will ask for in sequence.")]
        [SerializeField] protected List<PermissionType> PermissionsInOrder;

        [SerializeField] protected GameObject HeaderLogo;
        [SerializeField] protected TMP_Text MessageText;

        #if UNITY_IOS
        [SerializeField] protected AllAtOncePermissionsButtonHandler PermissionsHandler;
        #endif

        [SerializeField] protected string NeedAuthMessage = "In order to work our magic, we'll need you to grant us a few permissions.";

        [SerializeField] protected string NeedSettingsMessage = "In order to complete giving permissions you need to go to phone settings";


        [SerializeField] protected string AllDoneMessage = "Great! You're ready to go. Enjoy!";

        public List<PermissionType> GetOrderedPermissions()
        {
            return PermissionsInOrder;
        }

        #if UNITY_IOS
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            PermissionsHelperPlugin.OnPermissionStatusUpdated+=HandlePermissionChanged;
            UpdateMessages();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            PermissionsHelperPlugin.OnPermissionStatusUpdated-=HandlePermissionChanged;
        }
        #endif

        void HandlePermissionChanged(PermissionType type, bool result)
        {
            UpdateMessages();
        }

        void UpdateMessages()
        {
            #if UNITY_IOS
            var state = PermissionsHelperPlugin.Instance.GetCollectiveState();
            Debug.Log("In update message with state of: " + state.ToString());
            switch(state)
            {
                case CollectiveState.AllAuthorized:
                {

                    MessageText.text = AllDoneMessage;
                    break;
                }
                case CollectiveState.AllUnknown:
                case CollectiveState.SomeUnknown:
                {

                    MessageText.text = NeedAuthMessage;
                    break;
                }
                case CollectiveState.AllAsked:
                {

                    MessageText.text = NeedSettingsMessage;
                    break;
                }
            }
            #endif
        }


    }
}
