using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class PermissionHandler : MonoBehaviour
{
    GameObject dialog = null;

    private bool isLocation = false;
    private bool permissionsGranted;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        isLocation = true;
#endif
    }


    private void Update()
    {
        if (Input.location.isEnabledByUser && !isLocation)
            isLocation = true;
    }


    public void PromptLocationPermissionRequest()
    {
#if PLATFORM_ANDROID
        if (!Input.location.isEnabledByUser)
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            isLocation = true;
        }
#endif
    }

    
    void OnGUI()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            dialog.AddComponent<PermissionsRationaleDialog>();
            return;
        }
        else if (dialog != null)
        {
            Destroy(dialog);
        }
#endif
    }
}