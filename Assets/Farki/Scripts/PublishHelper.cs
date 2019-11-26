using CymaticLabs.Unity3D.Amqp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PublishHelper : MonoBehaviour
{
    [SerializeField] private string _exchangeName, _routingKey;
    public AmqpExchangeTypes ExchangeType = AmqpExchangeTypes.Direct;

    [SerializeField] private CameraController _cameraController;
    public RawImage userImage;

    private User _user;

    private string _jsonString;


    // Start is called before the first frame update
    void Start()
    {
        var subscription = new AmqpExchangeSubscription(_exchangeName, ExchangeType, _routingKey, HandleExchangeMessageReceived);
        AmqpClient.Subscribe(subscription);

        _user = new User();
        if (_cameraController == null)
            _cameraController = FindObjectOfType<CameraController>();
    }

    private void HandleExchangeMessageReceived(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        DeserializeUser(receivedJson);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
            Publish();

        if (Input.GetKeyDown("r"))
            DeserializeUser(_jsonString);
    }

    //private byte[] GetImage()
    //{
    //    byte[] imageArray;

    //    int width = Screen.width;
    //    int height = Screen.height;
    //    Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    tex.Apply();

    //    imageArray = tex.EncodeToPNG();


    //    Destroy(tex);
    //    return imageArray;
    //}

    public void DeserializeUser(string json)
    {
        int width = Screen.width;
        int height = Screen.height;

        _user = JsonUtility.FromJson<User>(json);

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(_user.image);
        userImage.texture = texture;
    }


    public void SerializeUser()
    {
        _user = new User
        {
            id = "Test",
            coordinates = new Coordinates
            {
                latitude = 12.090,
                longitude = 55.665
            },
            image = _cameraController.Screenshot
        };

        //File.WriteAllBytes(Application.dataPath + "/TestImage", GetImage());

        using (StreamWriter stream = new StreamWriter(Application.dataPath + "/TestJson.json"))
        {
            string json = JsonUtility.ToJson(_user, true);
            _jsonString = json;
            stream.Write(json);
        }
    }


    public void Publish()
    {
        var msg = JsonUtility.ToJson(_user);

        AmqpClient.Publish(_exchangeName, _routingKey, msg);
    }
}
