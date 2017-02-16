using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkBehaviour {
	public delegate void NetworkSceneManagerEvent ();
	public static event NetworkSceneManagerEvent OnLevelLoaded;
	private int _currentSceneId = 0;
	private AsyncOperation _nextLevel = null;

	/*private AsyncOperation _nextLevel = null;
	private int _clientFinishedLoading = 0;
	private int _clientLevelChanged = 0;

	const short LoadedMessage = MsgType.Highest + 1;
	const short LevelChangedMessage = MsgType.Highest + 2;
	const short AllowSceneActivationMessage = MsgType.Highest + 3;*/

	void Start()
	{
		_currentSceneId = GameObject.FindObjectsOfType<NetworkIdentity> ().Length;
	}

	[ClientRpc]
	public void RpcLoadLevel (string sceneToLoad, string sceneToUnload, bool allowSceneActivation) {
		Debug.LogError ("Loading Next Level");
		StartCoroutine (ClientLoadLevel (sceneToLoad, sceneToUnload, allowSceneActivation));
	}

	[ClientRpc]
	public void RpcAllowSceneActivation()
	{
		_nextLevel.allowSceneActivation = true;
	}

	public IEnumerator ClientLoadLevel(string sceneToLoad, string sceneToUnload, bool allowSceneActivation = true) {
		if (isServer)
			yield break;
		_nextLevel = SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Single);
		_nextLevel.allowSceneActivation = allowSceneActivation;
		yield return _nextLevel;

		_nextLevel = null;

		var sceneObjects = GameObject.FindObjectsOfType<NetworkIdentity> ();
		foreach (var sceneObject in sceneObjects) 
		{
			sceneObject.ForceSceneId (_currentSceneId);
			_currentSceneId++;
		}

		ClientScene.Ready (CustomNetworkLobbyManager.singleton.client.connection);
		/*
		if (sceneToUnload != null)
			yield return SceneManager.UnloadSceneAsync (sceneToUnload);
		*/
	}

	public IEnumerator ServerLoadLevel (string sceneToLoad, string sceneToUnload, bool allowSceneActivation = true) {
		NetworkServer.SetAllClientsNotReady ();

		_nextLevel = SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Single);
		_nextLevel.allowSceneActivation =  allowSceneActivation;
		yield return _nextLevel;

		_nextLevel = null;

		NetworkServer.SpawnObjects ();

		/*
		if (sceneToUnload != null)
			yield return SceneManager.UnloadSceneAsync (sceneToUnload);
		*/
		OnLevelLoaded ();
	}

	public void PreLoadLevel (string sceneToLoad, string sceneToUnload = null)
	{
		RpcLoadLevel (sceneToLoad, sceneToUnload, false);
		StartCoroutine (ServerLoadLevel (sceneToLoad, sceneToUnload, false));
	}

	public void ActivatePreloadedLevel ()
	{
		if (_nextLevel != null) {
			_nextLevel.allowSceneActivation = true;
			RpcAllowSceneActivation ();
		}
		else
			Debug.LogError ("No level preloaded");
	}

	public void LoadLevel (string sceneToLoad, string sceneToUnload = null)
	{
		RpcLoadLevel (sceneToLoad, sceneToUnload, true);
		StartCoroutine (ServerLoadLevel (sceneToLoad, sceneToUnload));
	}



	/*

	[ClientRpc]
	void RpcStartNextLevel()
	{
		StartCoroutine (LoadNextLevel ());
	}

	IEnumerator LoadNextLevel()
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
	IEnumerator WaitForAllClients()
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
