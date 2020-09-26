using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solarmax
{
    public interface IDataProvider
    {
        bool IsXML();
        string Path();

        void Load();

        bool Verify();

		void LoadExtraData ();

    }
}
