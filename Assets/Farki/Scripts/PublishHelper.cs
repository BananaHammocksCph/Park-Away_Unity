using CymaticLabs.Unity3D.Amqp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PublishHelper : MonoBehaviour
{
    [Header("Exchanges")]
    [SerializeField] private string _exchangeName, _routingKey;
    public AmqpExchangeTypes ExchangeType = AmqpExchangeTypes.Direct;
  
    [Header("Queue Subscriptions")]
    public UnityAmqpQueueSubscription[] QueueSubscriptions;
    List<AmqpQueueSubscription> queueSubscriptions;

    [SerializeField] private CameraController _cameraController;

    private LocationHandler _locationHandler;

    public RawImage userImage;

    private User _user;

    private string _jsonString;

    private void Awake()
    {
        queueSubscriptions = new List<AmqpQueueSubscription>();

        if (QueueSubscriptions != null && QueueSubscriptions.Length > 0)
        {
            queueSubscriptions.AddRange(QueueSubscriptions);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //var subscription = new AmqpExchangeSubscription(_exchangeName, ExchangeType, _routingKey, HandleExchangeMessageReceived);
        //AmqpClient.Subscribe(subscription);
    

        _locationHandler = FindObjectOfType<LocationHandler>();

        _user = new User();
        if (_cameraController == null)
            _cameraController = FindObjectOfType<CameraController>();
    }

    //private void HandleExchangeMessageReceived(AmqpExchangeReceivedMessage received)
    //{
    //    var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
    //    DeserializeUser(receivedJson);
    //}

    // Update is called once per frame
    void Update()
    {
   //     if (Input.GetKeyDown("p"))
           // Publish();

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

        User user = new User();
        user = JsonUtility.FromJson<User>(json);

        Debug.Log("Message from Queue: " + user.id);
        Debug.Log(user.coordinates.latitude);

        //Texture2D texture = new Texture2D(width, height);
        //texture.LoadImage(_user.image);
        //userImage.texture = texture;
    }


    public void SerializeUser()
    {
        User user = new User
        {
            type = "park",
            id = "5ddfb4cf8cd0ad2750f50c13",
            coordinates = new Coordinates
            {
                latitude = _locationHandler.Latitude,
                longitude = _locationHandler.Longitude
            },
            image = _cameraController.Screenshot
        };

        Debug.Log(user.id);
        Debug.Log(user.coordinates.latitude);
        //File.WriteAllBytes(Application.dataPath + "/TestImage", GetImage());

        //using (StreamWriter stream = new StreamWriter(Application.dataPath + "/TestJson.json"))
        //{
        //    string json = JsonUtility.ToJson(_user, true);
        //    _jsonString = json;
        //    stream.Write(json);
        //}

        Publish(user);
    }

    public void QueueMessage(AmqpQueueSubscription subscription, IAmqpReceivedMessage msg)
    {
        
        string inHuman = System.Text.Encoding.UTF8.GetString(msg.Body, 0, msg.Body.Length);
        DeserializeUser(inHuman);

        Debug.Log(msg.Properties.CorrelationId);
    }


    private IAmqpMessageProperties GetProperties()
    {
        MessageProperties properties = new MessageProperties("utf-8", "application/json", "CORRID");
        return properties;
    }

    public void Publish(User user)
    {
        var msg = JsonUtility.ToJson(user);

        //  string g = Guid.NewGuid().ToString();

        Debug.Log("PROPERTIES: " + GetProperties().CorrelationId);

        AmqpClient.Publish(_exchangeName, _routingKey, msg);
    }

} 