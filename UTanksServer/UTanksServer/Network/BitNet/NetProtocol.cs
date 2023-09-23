using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.Network.Simple.Net
{
	public enum PROTOCOL : short
	{
		BEGIN = 0,

		Server = 1,
		Client = 2,

		END
	}
}
