using CymaticLabs.Unity3D.Amqp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public string ExchangeName, RoutingKey;
    public AmqpExchangeTypes ExchangeType = AmqpExchangeTypes.Topic;




    // Start is called before the first frame update
    void Start()
    {
        var subscription = new AmqpExchangeSubscription(ExchangeName, ExchangeType, RoutingKey, HandleMessageReceived);
        AmqpClient.Subscribe(subscription);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
            Publish();
    }

    public void HandleMessageReceived(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);

        Debug.Log("MESSAGE RECEIVED!!!!!: " + receivedJson);
    }

    public void HandleQueueMessage(AmqpQueueReceivedMessage received)
    {
         
    }

    public void HandleMessageFromQueue(AmqpQueueSubscription subscription, IAmqpReceivedMessage message)
    {

        string inHuman = System.Text.Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);
       // var consumer = 
        Debug.Log("HANDLE MESSAGE FROM QUEUE: " + inHuman);
        subscription.UseAck = true;
    }

    public void HandleMessageFromExchange(AmqpExchangeSubscription subscription, IAmqpReceivedMessage message)
    {
        Debug.Log("HANDLE MESSAGE FROM Exchange: " + message);
    }

    public void Publish()
    {
        string str = "\"id\":\"cube1\", \"posx\":2, \"posY\":0.5f, \"posZ\"-0.2f";
        var message = str;
        AmqpClient.Publish(ExchangeName, RoutingKey, message);
    }
}
