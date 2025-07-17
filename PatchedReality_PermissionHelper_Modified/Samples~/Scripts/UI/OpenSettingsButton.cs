using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobilePermissions.iOS.Examples
{

    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class OpenSettingsButton : MonoBehaviour
    {
        void Start()
        {
            Button.onClick.AddListener(HandleButtonClick);
        }

        void HandleButtonClick()
        {
#if UNITY_IOS
            PermissionsHelperPlugin.Instance.OpenSettings();
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