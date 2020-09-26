using System;
using System.Collections.Generic;
using NetMessage;
using UnityEngine;
using System.IO;

namespace Solarmax
{
	public delegate void MessageHandler(int msgId, PacketEvent message);

	public class PacketHandler : IPacketHandler
    {
		public int iPacketType;
		public MessageHandler mHandler;


		public int GetPacketType()
		{
			return iPacketType;
		}

		public bool OnPacketHandler(byte[] data)
		{
            using (MemoryStream stream = new MemoryStream(data, 0, data.Length ))
            {
                PacketEvent proto = new PacketEvent(iPacketType, stream);
                mHandler(iPacketType, proto);
                //Debug.Log("Protol Recv Mssage ID " + iPacketType.ToString() );
            }
            return true;
		}
	}

    public class PacketHandlerManager : IPacketHandlerManager
    {
		private Dictionary<int, IPacketHandler> mHandlerDict = null;

        public PacketHandlerManager()
        {
			mHandlerDict = new Dictionary<int, IPacketHandler> ();
        }

        public bool Init()
        {

            //RegisterHandler(typeof(XMessage.SC_HelloWorldResult), new SCHelloWorldResultHandler());
            //RegisterHandler(typeof(XMessage.SC_PingResult), new SCPingResultHandler());

            return true;
        }

        public void Tick(float interval)
        {
            // don't need
        }

        public void Destroy()
        {
            mHandlerDict.Clear();
        }

		public void RegisterHandler(int packetType, MessageHandler handler)
        {
			if (null != handler)
			{
				PacketHandler packethandler = new PacketHandler();
				packethandler.iPacketType = packetType;
				packethandler.mHandler = handler;
				mHandlerDict.Add (packetType, packethandler);
			}
		}

        public bool DispatchHandler(int type, byte[] data)
        {
            if (data != null && mHandlerDict.ContainsKey(type))
            {
                IPacketHandler handler = mHandlerDict[type];
                if (null != handler)
     			{
					return handler.OnPacketHandler (data);
                }
            }

            return false;
        }

    }
}
