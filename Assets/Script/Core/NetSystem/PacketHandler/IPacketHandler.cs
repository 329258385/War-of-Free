﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solarmax
{
    public interface IPacketHandler
    {
        int GetPacketType();

        bool OnPacketHandler(Byte[] data);
    }
}
