using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public RawImage cameraDisplayImage, confirmCaptureImage;
    public GameObject confirmCapPanel;
    int width = Screen.width;
    int height = Screen.height;
    private byte[] _screenshot;


    #region Properties

    public byte[] Screenshot
    {
        get { return _screenshot; }

        set { _screenshot = value; }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cameraDisplayImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOnCamera()
    {
        WebCamTexture webCamTexture = new WebCamTexture(width, height);

        cameraDisplayImage.gameObject.SetActive(true);
        cameraDisplayImage.material.mainTexture = webCamTexture;
        webCamTexture.Play();
    }

    public void OnCaptureButton()
    {
        //disable elements for screencapture

        foreach(Transform t in cameraDisplayImage.transform)
        {
            t.gameObject.SetActive(false);
        }

        // Capture
        CaptureScreenshot();
    }

    private void CaptureScreenshot()
    {
        byte[] imageArray;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        imageArray = tex.EncodeToPNG();


        Destroy(tex);
        Screenshot = imageArray;

        cameraDisplayImage.gameObject.SetActive(false);
        confirmCapPanel.SetActive(true);
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(Screenshot);
    }




}
