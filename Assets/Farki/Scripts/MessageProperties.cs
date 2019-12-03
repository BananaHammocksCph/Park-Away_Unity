using CymaticLabs.Unity3D.Amqp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageProperties : IAmqpMessageProperties
{
    public string AppId { get ; set ; }
    public string ClusterId { get ; set ; }
    public string ContentEncoding { get ; set ; }
    public string ContentType { get ; set ; }
    public string CorrelationId { get; set; }
    public byte DeliveryMode { get ; set ; }
    public string Expiration { get ; set ; }
    public IDictionary<string, object> Headers { get ; set ; }
    public string MessageId { get ; set ; }
    public bool Persistent { get ; set ; }
    public byte Priority { get ; set ; }

    public int ProtocolClassId { get; set; }

    public string ProtocolClassName { get; set; }

    public string ReplyTo { get ; set ; }
    public string ReplyToAddress { get ; set ; }
    public long Timestamp { get ; set ; }
    public string Type { get ; set ; }
    public string UserId { get ; set ; }


    private string _correlationId;

    public MessageProperties(string contentEncoding, string contentType, string correlationId)
    {
        ContentEncoding = contentEncoding;
        ContentType = contentType;
        CorrelationId = correlationId;
    }
}
