using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using log4net;
using GuessingClient.Messages;

namespace GuessingClient.Network
{
  public class UdpCommunicator
  {
    private MessageFactory decoder = new MessageFactory();
    public UdpCommunicator(UInt16 port,  Int32 timeout = 1)
    {
      m_client = new UdpClient(port);
      m_client.Client.SendTimeout = timeout;
      m_client.Client.ReceiveTimeout = timeout;
    }

    public Message read()
    {
      log.DebugFormat("Blocking for {0} to receive UDP packets", m_client.Client.ReceiveTimeout);
      IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
      byte[] readBytes = null;
      try
      {
        readBytes = m_client.Receive(ref remote);
      }
      catch (SocketException ex)
      {
        log.Debug("Caught Socket Exception", ex);
      }
      catch (ObjectDisposedException ex)
      {
        log.Debug("Attempted to read from a disposed UDP socket", ex);
      }
      
      return decoder.decodeMessage(readBytes, (Int16)(readBytes?.Count() ?? 0));
    }

    public Int32 write(Message msg, IPEndPoint destination)
    {
      byte[] data = msg.Encode();
      log.DebugFormat("Sending {0} bytes to {1}", data.Count(), destination.ToString());
      Int32 result = -1;
      try
      {
        result = m_client.Send(data, data.Count(), destination);
      }
      catch (SocketException ex)
      {
        log.Debug("Caught Socket Exception", ex);
      }
      catch (ObjectDisposedException ex)
      {
        log.Debug("Attempted to write to a disposed UDP socket", ex);
      }

      return result;
    }

    private UdpClient m_client;
    private static readonly ILog log = LogManager.GetLogger(typeof(UdpCommunicator));
  }
}
