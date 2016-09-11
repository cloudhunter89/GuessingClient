using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuessingClient.Network;
using GuessingClient.Messages;

namespace GuessingClientTest.Network
{
  [TestClass]
  public class UdpSocketTest
  {
    [TestMethod]
    public void TestReadWrite()
    {
      UInt16 serverPort = 65000;
      UdpCommunicator server = new UdpCommunicator(serverPort, 3);
      UdpCommunicator client = new UdpCommunicator(0, 3);

      AckMessage expectedMsg = new AckMessage();
      expectedMsg.GameId = 12;
      client.write(expectedMsg, new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), serverPort));

      AckMessage receivedData = server.read() as AckMessage;

      Assert.AreEqual(expectedMsg.Type, receivedData.Type);
      Assert.AreEqual(expectedMsg.GameId, receivedData.GameId);
    }
  }
}
