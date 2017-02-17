using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.NetworkSystem;

public class NetworkSceneManager : NetworkBehaviour {
	public delegate void NetworkSceneManagerEvent (string sceneName);
	public static event NetworkSceneManagerEvent OnServerLevelLoaded;
	public static event NetworkSceneManagerEvent OnClientLevelLoaded;
	private int _currentSceneId = 0;
	private AsyncOperation _nextLevel = null;

	public static NetworkSceneManager singleton;

	public class CustomMsgType {
		public static short AllowSceneActivation = MsgType.Highest + 1;
		public static short LoadLevel = MsgType.Highest + 2;
	}

	public class SceneMessage: MessageBase
	{
		public string sceneToLoad;
		public string sceneToUnload;
		public bool allowSceneActivation = true;
	}

	/*private AsyncOperation _nextLevel = null;
	private int _clientFinishedLoading = 0;
	private int _clientLevelChanged = 0;

	const short LoadedMessage = MsgType.Highest + 1;
	const short LevelChangedMessage = MsgType.Highest + 2;
	const short AllowSceneActivationMessage = MsgType.Highest + 3;*/

	void Awake()
	{
		if (singleton == null)
			singleton = this;
		else
			this.enabled = false;
	}

	public void RegisterClientMessages()
	{
		var client = CustomNetworkLobbyManager.singleton.client;
		client.RegisterHandler (CustomMsgType.LoadLevel, ClientLoadLevelHandler);
		client.RegisterHandler (CustomMsgType.AllowSceneActivation, ClientAllowSceneActivationHandler);
	}

	void Start()
	{
		_currentSceneId = GameObject.FindObjectsOfType<NetworkIdentity> ().Length;
	}

	public void ClientLoadLevelHandler (NetworkMessage netMsg) {
		Debug.LogError ("Client Loading Next Level");
		var msg = netMsg.ReadMessage<SceneMessage>();
		Timing.RunCoroutine (ClientLoadLevelCoroutine (msg.sceneToLoad, msg.sceneToUnload, msg.allowSceneActivation));
	}

	public void ClientAllowSceneActivationHandler(NetworkMessage msg)
	{
		Debug.LogError ("AllowSceneActivation");
		_nextLevel.allowSceneActivation = true;
	}

	public IEnumerator<float> ClientLoadLevelCoroutine (string sceneToLoad, string sceneToUnload, bool allowSceneActivation = true) {
		if (isServer)
			yield break;
		_nextLevel = SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);
		_nextLevel.allowSceneActivation = allowSceneActivation;
		while (!_nextLevel.isDone)
			yield return 0.0f;

		_nextLevel = null;

		var sceneObjects = GameObject.FindObjectsOfType<NetworkIdentity> ();
		foreach (var sceneObject in sceneObjects) 
		{
			sceneObject.ForceSceneId (_currentSceneId);
			_currentSceneId++;
		}

		ClientScene.Ready (CustomNetworkLobbyManager.singleton.client.connection);
		if (sceneToUnload != null) {
			var async = SceneManager.UnloadSceneAsync (sceneToUnload);
			while (!async.isDone)
				yield return 0.0f;
		}
		if (OnClientLevelLoaded != null)
			OnClientLevelLoaded (sceneToLoad);
	}

	public IEnumerator<float> ServerLoadLevel (string sceneToLoad, string sceneToUnload, bool allowSceneActivation = true) {
		if (allowSceneActivation)
			NetworkServer.SetAllClientsNotReady ();

		_nextLevel = SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);
		_nextLevel.allowSceneActivation =  allowSceneActivation;
		while (!_nextLevel.isDone)
			yield return 0.0f;

		_nextLevel = null;

		NetworkServer.SpawnObjects ();

		if (sceneToUnload != null) {
			var async = SceneManager.UnloadSceneAsync (sceneToUnload);
			while (!async.isDone)
				yield return 0.0f;
		}
		if (OnServerLevelLoaded != null)
			OnServerLevelLoaded (sceneToLoad);
	}

	public void PreLoadLevel (string sceneToLoad, string sceneToUnload = null)
	{
		LoadLevel (sceneToLoad, sceneToUnload, false);
	}

	public void ActivatePreloadedLevel ()
	{
		if (_nextLevel != null) {
			NetworkServer.SetAllClientsNotReady ();
			_nextLevel.allowSceneActivation = true;
			NetworkServer.SendToAll (CustomMsgType.AllowSceneActivation, new EmptyMessage());
		}
		else
			Debug.LogError ("No level preloaded");
	}

	public void LoadLevel (string sceneToLoad, string sceneToUnload = null, bool allowSceneActivation = true)
	{
		var msg = new SceneMessage ();
		msg.sceneToLoad = sceneToLoad;
		msg.sceneToUnload = sceneToUnload;
		msg.allowSceneActivation = allowSceneActivation;
		NetworkServer.SendToAll (CustomMsgType.LoadLevel, msg);
		Timing.RunCoroutine (ServerLoadLevel (sceneToLoad, sceneToUnload, allowSceneActivation));
	}



	/*

	[ClientRpc]
	void RpcStartNextLevel()
	{
		Timing.RunCoroutine (LoadNextLevel ());
	}

	IEnumerator<float><float> LoadNextLevel()
	{
		if (this.currentLevelIndex + 1 < this.levels.Length)
		{
			Debug.Log ("Coroutine");
			var sceneName = this.levels [this.currentLevelIndex + 1].sceneName;
			_nextLevel = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Single);
			_nextLevel.allowSceneActivation = false;

			while (_nextLevel.progress < 0.9f) {
				Debug.Log (_nextLevel.progress);
				yield return null;
			}

			Debug.Log ("NextLevel.progress == 0.9f");
			CustomNetworkLobbyManager.singleton.client.Send (LoadedMessage, new EmptyMessage ());

			while (!_nextLevel.isDone)
				yield return null;

			NetworkIdentity[] sceneObjects = GameObject.FindObjectsOfType<NetworkIdentity> ();
			var currentSceneId = sceneObjects.Length + 1;
			foreach (NetworkIdentity sceneObject in sceneObjects) {
				sceneObject.ForceSceneId (currentSceneId);
				currentSceneId++;
			}

			CustomNetworkLobbyManager.singleton.client.Send (LevelChangedMessage, new EmptyMessage ());
			ClientScene.Ready (this.connectionToServer);
		}
	}
	*/

	/*

	void OnClientLevelChanged(NetworkMessage msg)
	{
		_clientLevelChanged++;
		Debug.Log (_clientLevelChanged);
	}

	void OnClientLevelFinishedLoading(NetworkMessage msg)
	{
		_clientFinishedLoading++;
		Debug.Log (_clientFinishedLoading);
	}

	[Command]
	void CmdInTheLevel()
	{
		_clientLevelChanged++;
		Debug.Log (_clientLevelChanged);
	}
	*/


	/*

	[Server]
	IEnumerator<float><float> WaitForAllClients()
	{
		Debug.Log ("wait for all clients");
		Debug.Log (NetworkServer.connections.Count);
		NetworkServer.SetAllClientsNotReady ();
		while (_clientFinishedLoading < NetworkServer.connections.Count) {
			yield return null;
		}
		Debug.Log (_clientFinishedLoading);
		_clientFinishedLoading = 0;
		_nextLevel.allowSceneActivation = true;
		NetworkServer.SendToAll (AllowSceneActivationMessage, new EmptyMessage ());
		while (_clientLevelChanged < NetworkServer.connections.Count) {
			yield return null;
		}
		paused = true;
		this.currentLevelIndex++;
		_clientLevelChanged = 0;
		var sceneName = this.levels[this.currentLevelIndex].sceneName;
		yield return SceneManager.LoadSceneAsync (sceneName);
		NetworkIdentity[] sceneObjects = GameObject.FindObjectsOfType<NetworkIdentity> ();
		var currentSceneId = sceneObjects.Length + 1;
		foreach (NetworkIdentity sceneObject in sceneObjects) {
			sceneObject.ForceSceneId (currentSceneId);
			currentSceneId++;
		}
		NetworkServer.SpawnObjects ();
		StartLevel ();
	}
	*/

}
