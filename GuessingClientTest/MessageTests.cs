using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuessingClient.Messages;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GuessingClientTest
{
  [TestClass]
  public class MessageTests
  {
    MessageFactory decoder = new MessageFactory();

    [TestMethod]
    public void NewGame()
    {
      byte[] testBytes = {
        0x00, 0x01, // Message Type (1 - NewGame)
        0x00, 0x12, // A# Unicode length (18) // First Name Unicode Length (6)  // First Name (Jon)   // Last Name Unicode Length (10)  // Last Name (Koyle)
        0x00, 0x41, 0x00, 0x30, 0x00, 0x30, 0x00, 0x39, 0x00, 0x33, 0x00, 0x38, 0x00, 0x34, 0x00, 0x37, 0x00, 0x32, // A# (A00938472)
        0x00, 0x06, 
        0x00, 0x4A, 0x00, 0x4F, 0x00, 0x4E, 
        0x00, 0x0A,
        0x00, 0x4B, 0x00, 0x4F, 0x00, 0x59, 0x00, 0x4C, 0x00, 0x45
      };

      NewGameMessage decoded = new NewGameMessage();
      decoded.Decode(testBytes);

      Assert.AreEqual(1, decoded.Type);
      Assert.AreEqual("A00938472", decoded.ANumber);
      Assert.AreEqual("JON", decoded.FirstName);
      Assert.AreEqual("KOYLE", decoded.LastName);

      byte[] encodedBytes = decoded.Encode();

      for (Int32 index = 0; index < 42; index++)
      {
        Assert.AreEqual(testBytes[index], encodedBytes[index]);
      }
    }
    
    [TestMethod]
    public void MessageDecoder_Decode()
    {
      GameDefMessage gameDef = new GameDefMessage();
      gameDef.Type = MessageType.GameDef;

      List<Message> expectedMessages = new List<Message>();
      List<Message> decodedMessages = new List<Message>();

      for (MessageType msgType = MessageType.NewGame; msgType < MessageType.InvalidMessageType; msgType++)
      {
        Message msg = decoder.createMessage(msgType);
        expectedMessages.Add(msg);
        decodedMessages.Add(decoder.decodeMessage(msg.Encode(), (Int16)msg.Encode().Count()));
      }

      for (int i = 0; i < expectedMessages.Count(); ++i)
      {
        Type expectedType = expectedMessages[i].GetType();
        Type actualType = decodedMessages[i].GetType();
        Assert.AreEqual(expectedType, actualType);
        foreach (PropertyInfo property in expectedType.GetProperties())
        {
          MethodInfo getter = property.GetMethod;
          object expected = invokeGetter(expectedMessages[i], getter);
          object actual = invokeGetter(decodedMessages[i], getter);
          Assert.AreEqual(expected, actual);
        }
      }
    }

    object invokeGetter(object instance, MethodInfo getter)
    {
      return getter.Invoke(instance, null);
    }
  }
}
