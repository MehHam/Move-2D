using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	public class WebGLCustomNetworkConnection : NetworkConnection
	{   
		private int _offset;

		public override void Initialize(string networkAddress, int networkHostId, int networkConnectionId, HostTopology hostTopology)
		{
			_offset = 10;
			base.Initialize(networkAddress, networkHostId, networkConnectionId + _offset, hostTopology);
		}

		public override bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
		{
			return NetworkTransport.Send(WebGLCustomServer.hostId, connectionId - _offset, channelId, bytes, numBytes, out error);
		}
	}
}

