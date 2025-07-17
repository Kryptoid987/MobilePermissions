using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobilePermissions.iOS.Examples
{
    #if UNITY_IOS
    using CollectiveState = CollectivePermissionsStatus.CollectiveState;
    using PermissionType = PermissionsHelperPlugin.PermissionType;
    #endif

    /*
        Put one of these in your "loading" scene. It will decide if it needs to go to the
        permission granting scene or your real app scene. Right now, it uses LoadScene
        with no loading between - on the assumption that you arrange the layout of your loading
        scene, your permissions scene, and your app start / title scene to match...
     */
    public class PermissionsSceneSelector : MonoBehaviour
    {
        [SerializeField] protected string PermissionsScene;
        [SerializeField] protected string MainScene;

        [SerializeField] protected List< PermissionType> RequiredPermissions;

        #if UNITY_IOS
        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            PermissionsHelperPlugin.Instance.SetRequiredPermissions(RequiredPermissions);
            var status = PermissionsHelperPlugin.Instance.GetCollectiveState();
            MobilePermissions.iOS.Examples.AllAtOncePermissionsButtonHandler.OnAllPermissionsAuthorized += ToMainScene;

            if(status.Equals(CollectiveState.AllAuthorized))
            {
                ToMainScene();
            }
            else
            {
                ToPermissionsScene();
            }
        }
        #endif

        void ToPermissionsScene()
        {
            //listen for our ok from the single button class.
            //TODO: remove this assumption - this scene selector would be useful
            //even in either permission scene styles.
            SceneManager.LoadScene(PermissionsScene,LoadSceneMode.Single);

        }
        void ToMainScene()
        {
            MobilePermissions.iOS.Examples.AllAtOncePermissionsButtonHandler.OnAllPermissionsAuthorized -= ToMainScene;
            SceneManager.LoadScene(MainScene,LoadSceneMode.Single);
        }

    }

}

