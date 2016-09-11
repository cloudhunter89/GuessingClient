using System;
using System.Linq;
using System.Text;
using log4net;

namespace GuessingClient.Messages
{
  abstract public class Message
  {
    public MessageType Type { get; set; }

    public abstract void Decode(byte[] buffer);

    public virtual byte[] Encode()
    {
      byte[] type = System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((Int16)Type));
      return type;
    }

    public static MessageType getType(byte[] buffer, Int32 bufferSize)
    {
      MessageType type = MessageType.InvalidMessageType;
      if (bufferSize > 2)
      {
        Int16 typeValue = getShort(buffer, 0);
        if (typeValue < (Int16)MessageType.InvalidMessageType)
          type = (MessageType)typeValue;
      }

      return type;
    }

    static protected byte[] encodeString(string toEncode)
    {
      log.Debug("Begin encodeString param: " + toEncode);
      Int16 stringLength = (Int16)System.Text.Encoding.BigEndianUnicode.GetByteCount(toEncode);
      byte[] encoded = new byte[stringLength + 2];
      stringLength = System.Net.IPAddress.HostToNetworkOrder(stringLength);
      BitConverter.GetBytes(stringLength).CopyTo(encoded, 0); 
      Encoding.BigEndianUnicode.GetBytes(toEncode).CopyTo(encoded, 2);
      log.InfoFormat("Created a byte array of length {0} for string of length {1} (Two additional bytes for Int16 length)", encoded.Count(), toEncode.Length);
      log.DebugFormat("Resulting bytes {0}", BitConverter.ToString(encoded));
      return encoded;
    }

    static protected string decodeString(byte[] toDecode, ref Int16 startOffset)
    {
      log.DebugFormat("Begin decodeString() at buffer offset: {0}, buffer length: {1}", startOffset, toDecode.Count());
      Int16 stringLength = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt16(toDecode, startOffset));
      startOffset += sizeof(Int16);
      string decoded = null;
      if (stringLength == 0)
      {
        log.Debug("String length is 0 returning string.Empty");
        decoded = string.Empty;
      }
      else if (toDecode.Count() >= (startOffset + stringLength))
      {
        log.DebugFormat("Converting {0} bytes to a string", stringLength);
        decoded = Encoding.BigEndianUnicode.GetString(toDecode, startOffset, stringLength);
        startOffset = (Int16)(startOffset + stringLength);
        log.Info("Conversion result: " + decoded);
      }
      else
      {
        log.ErrorFormat("Unable to decode a string of length {0} bytes at offset {1} from buffer of length {2}", stringLength, startOffset, toDecode.Count());
      }
      return decoded;
    }

    static protected byte[] getBytes(Int16 value)
    {      
      byte[] shortBytes = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(value));
      log.DebugFormat("Converted {0} to Network Order byte[] {1}", value, shortBytes);
      return shortBytes;
    }

    static protected Int16 getShort(byte[] buffer, Int16 offset)
    {
      Int16 converted = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));
      log.DebugFormat("Converted Network Order byte[]: {0} to Host Order value: {1}", BitConverter.ToString(buffer, offset, sizeof(Int16)), converted);
      return converted;
    }

    protected static readonly ILog log = LogManager.GetLogger(typeof(Message));
  }
}
