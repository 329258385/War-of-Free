using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solarmax
{

    public interface IPacket
    {

        int GetPacketType();

        Byte[] GetData();
    }


    public class Packet
    {
        public int      MsgId { get; private set; }
        public object   Msg { get; private set; }

        public Packet(int msgId, object msg)
        {
            this.MsgId  = msgId;
            this.Msg    = msg;
        }
    }
}
