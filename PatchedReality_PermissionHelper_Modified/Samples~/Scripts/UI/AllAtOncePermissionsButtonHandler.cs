using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MobilePermissions.iOS.Examples
{
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    using PermissionStatus = PermissionsHelperPlugin.PermissionStatus;
    using CollectiveState = CollectivePermissionsStatus.CollectiveState;
    /**
        Control label and behaviour of the single button in the all at once UI.!-- 
        Goal of this UI is as few clicks as possible. Thus a single button who's label changes based on 
        circumstances.
     */

    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class AllAtOncePermissionsButtonHandler : MonoBehaviour
    {
        public delegate void PermissionSequenceChangeHandler();
        public static PermissionSequenceChangeHandler OnAllPermissionsAuthorized;


        [SerializeField] protected TMP_Text Label;

        [Tooltip("Label when user never auth'd or has some perms that are unknown.")]
        [SerializeField] protected string LabelNeedAuth = "Let's Go!";

        [Tooltip("Label when user finishes and still didn't auth, or starts in a state where all perms have been asked and at least some are declined.")]
        [SerializeField] protected string LabelNeedSettings = "Go to Settings";

        [Tooltip("Label when user finishes successfully, and are ready to move on to main activity")]
        [SerializeField] protected string LabelAllDone = "Next";

        [Tooltip("Label we set when button is inactive (during perms dialogs/settings). Set to blank to keep label as is")]
        [SerializeField] protected string LabelWaitingForOS = ".....";

        //button context reflects state of user flow through permissions process.
        public enum ButtonContext
        {
            Beginning,
            Processing,
            Completed
        }

        protected ButtonContext context = ButtonContext.Beginning;

        private SerialPermissionAuthSequence authSequence;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            MyButton.onClick.AddListener(HandleClick);
        }


        /// <summary>
        ///  sets us to the beginning context, and figures out how to label us.
        /// </summary>
        void OnEnable()
        {
            var state = PermissionsHelperPlugin.Instance.GetCollectiveState();
            MyButton.interactable = true;
            switch (state)
            {
                case CollectiveState.AllAsked:
                case CollectiveState.AllAuthorized:
                    {
                        //we are done here...
                        context = ButtonContext.Completed;
                        break;
                    }
                case CollectiveState.SomeUnknown:
                case CollectiveState.AllUnknown:
                    {
                        //we have some questions to ask.
                        context = ButtonContext.Beginning;
                        break;
                    }
            }
            UpdateLabel();
        }

        void HandleClick()
        {
            //figure out what to do based on collective state and context.
            switch (context)
            {
                case ButtonContext.Beginning:
                    {
                        //start a permission flow. disable our button.
                        context = ButtonContext.Processing;
                        authSequence = new SerialPermissionAuthSequence(PermissionsHelperPlugin.Instance.RequiredPermissions);
                        UpdateLabel();
                        MyButton.interactable = false;
                        authSequence.Start(HandleFlowDone);
                        break;
                    }
                case ButtonContext.Processing:
                    {
                        //a no-op, just warn cause it shouldn't happen.
                        Debug.LogWarning("Got button click in processing. Should be inactive");
                        break;
                    }
                case ButtonContext.Completed:
                    {

                        var state = PermissionsHelperPlugin.Instance.GetCollectiveState();
                        if (state == CollectiveState.AllAuthorized || Application.isEditor)
                        {
                            //trigger our delegate, which will handle doing the right thing.
                            OnAllPermissionsAuthorized?.Invoke();
                        }
                        else
                        {
                            //not all happy, need to go to settings.
                            PermissionsHelperPlugin.Instance.OpenSettings();
                        }

                        break;
                    }
            }
        }

        void HandleFlowDone()
        {
            Debug.Log("Auth flow done!");
            ///when flow is done, go to the complete state, and update label.!--
            authSequence = null;
            MyButton.interactable = true;
            context = ButtonContext.Completed;
            UpdateLabel();
        }

        void UpdateLabel()
        {

            CollectivePermissionsStatus.CollectiveState state = PermissionsHelperPlugin.Instance.GetCollectiveState();
            switch (context)
            {
                case ButtonContext.Beginning:
                case ButtonContext.Completed:
                    {
                        if (state == CollectiveState.AllAsked)
                        {
                            //user is going to need to go to settings. 
                            Label.text = LabelNeedSettings;
                        }
                        else if (state == CollectiveState.AllUnknown ||
                                        state == CollectiveState.SomeUnknown)
                        {
                            //there are at least some perms that need to/can be requested. 
                            Label.text = LabelNeedAuth;
                        }
                        else
                        {
                            Label.text = LabelAllDone;
                        }
                        break;
                    }
                case ButtonContext.Processing:
                    {
                        if (!string.IsNullOrEmpty(LabelWaitingForOS))
                        {
                            Label.text = LabelWaitingForOS;
                        }
                        break;
                    }
            }
        }


        UnityEngine.UI.Button MyButton
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

