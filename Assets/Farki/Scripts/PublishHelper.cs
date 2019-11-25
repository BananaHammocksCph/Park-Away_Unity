using CymaticLabs.Unity3D.Amqp;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PublishHelper : MonoBehaviour
{
    [SerializeField] private string _exchangeName, _routingKey;
    public AmqpExchangeTypes ExchangeType = AmqpExchangeTypes.Direct;

    public RawImage userImage;

    private User _user;

    private string _jsonString;


    // Start is called before the first frame update
    void Start()
    {
        _user = new User();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
            SerializeUser();

        if (Input.GetKeyDown("r"))
            DeserializeUser(_jsonString);
    }

    private byte[] GetImage()
    {
        byte[] imageArray;

        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        imageArray = tex.EncodeToPNG();


        Destroy(tex);
        return imageArray;
    }

    public void DeserializeUser(string json)
    {
        int width = Screen.width;
        int height = Screen.height;

        using (StreamReader reader = new StreamReader(Application.dataPath + "/TestJson.json"))
        {
            string j = reader.ReadToEnd();
            _user = JsonUtility.FromJson<User>(j);
        }

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(_user.image);
        userImage.texture = texture;
    }


    public void SerializeUser()
    {
        User user = new User
        {
            id = "Test",
            coordinates = new Coordinates
            {
                latitude = 12.090,
                longitude = 55.665
            },
            image = GetImage()
        };

        File.WriteAllBytes(Application.dataPath + "/TestImage", GetImage());

        using (StreamWriter stream = new StreamWriter(Application.dataPath + "/TestJson.json"))
        {
            string json = JsonUtility.ToJson(user, true);
            _jsonString = json;
            stream.Write(json);
        }

        Debug.Log("PATH : " + Application.dataPath);
    }


    public void Publish()
    {
        string str = "\"id\":\"cube1\", \"posx\":2, \"posY\":0.5f, \"posZ\"-0.2f";
        var message = str;
        AmqpClient.Publish(_exchangeName, _routingKey, message);
    }
}
