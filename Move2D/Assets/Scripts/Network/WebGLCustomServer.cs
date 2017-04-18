using UnityEngine;
using UnityEngine.Networking;

namespace Move2D
{
	public class WebGLCustomServer : MonoBehaviour
	{
		public static int hostId;
		public NetworkServerSimple webGLServer = new NetworkServerSimple();
		bool _active;

		public void Initialize()
		{
			webGLServer = new NetworkServerSimple ();
			webGLServer.SetNetworkConnectionClass<WebGLCustomNetworkConnection>();
			webGLServer.useWebSockets = true;
			webGLServer.RegisterHandler(MsgType.Connect, this.OnWebGLConnected);
			webGLServer.RegisterHandler(MsgType.Disconnect, this.OnWebGLDisconnected);
			webGLServer.Listen (7777);

			hostId = webGLServer.serverHostId;
			_active = true;
		}

		public void Stop()
		{
			webGLServer.DisconnectAllConnections ();
			webGLServer.Stop ();
			_active = false;
		}

		void Update()
		{
			if (_active) {
				webGLServer.Update ();
			}
		}
			
		void OnWebGLConnected(NetworkMessage netMsg)
		{
			Debug.Log ("Connected");
			NetworkServer.AddExternalConnection (netMsg.conn);
		}

		void OnWebGLDisconnected(NetworkMessage netMsg)
		{
			Debug.Log ("Disconnected");
			NetworkServer.RemoveExternalConnection (netMsg.conn.connectionId);
		}
	}
}

