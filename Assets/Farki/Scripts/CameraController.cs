using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public RawImage cameraDisplayImage, confirmCaptureImage;
    public GameObject mainPanel, capturePanel, confirmCapPanel;
    int width = Screen.width;
    int height = Screen.height;
    private byte[] _screenshot;
    string _path;


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
        _path = Path.Combine( Application.persistentDataPath , "screenshot");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOnCamera()
    {
        WebCamTexture webCamTexture = new WebCamTexture(width, height);

        capturePanel.SetActive(true);
        mainPanel.SetActive(false);
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
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }

        ScreenCapture.CaptureScreenshot( _path);


        StartCoroutine(CheckSS());
    }

    IEnumerator CheckSS()
    {
        yield return new WaitUntil(() => File.Exists(_path));
        Screenshot = File.ReadAllBytes(_path);

        capturePanel.SetActive(false);
        confirmCapPanel.SetActive(true);
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(Screenshot);
        confirmCaptureImage.texture = texture;
    }
 
    public void OnAcceptButton()
    {
        PublishHelper _publishHelper = FindObjectOfType<PublishHelper>();

        _publishHelper.SerializeUser();

        mainPanel.SetActive(true);
        confirmCapPanel.SetActive(false);
    }


    public void OnDeclineButton()
    {
        capturePanel.SetActive(true);
        foreach (Transform t in cameraDisplayImage.transform)
        {
            t.gameObject.SetActive(true);
        }

        confirmCapPanel.SetActive(false);
    }

}
