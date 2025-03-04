namespace LibraryManager.Models;

/// <summary>
/// Represents a message object used for communication between components via
/// the WeakReferenceMessenger. This class encapsulates details of a message,
/// including the sender identifier and the message content.
/// </summary>
/// <author>YR 2025-03-04</author>
public class ReferenceMessage
{
    /// <summary>
    /// nameof(sender_object)
    /// </summary>
    public string SenderID { get; } 
    /// <summary>
    /// Message text
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Represents a message object used for communication between components via
    /// the WeakReferenceMessenger. This class encapsulates details of a message,
    /// including the sender identifier and the message content.
    /// </summary>
    /// <param name="senderId">nameof(sender_object)</param>
    /// <param name="value">data</param>
    public ReferenceMessage(string senderId, string value)
    {
        SenderID = senderId;
        Value = value;
    }
}