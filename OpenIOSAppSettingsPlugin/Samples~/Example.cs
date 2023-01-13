using UnityEngine;
#if UNITY_IOS
using MobilePermissions.iOS;
#endif

namespace MobilePermissions.iOS.Examples {
    public class Example : MonoBehaviour
    {
        public void OpenSettings()
        {
#if UNITY_IPHONE
            string url = AppSettingsIOSNativeBindings.GetSettingsURL();
            Debug.Log("the settings url is:" + url);
            Application.OpenURL(url);
#endif
        }
    }
}