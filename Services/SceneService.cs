using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneService
{
    public SceneService(GameService _service)
    {
        gameService = _service;
        LastRoomName = string.Empty;
        additiveScenes = new();
    }

    #region Variables

    private GameService gameService;

    //Services
    private AssetService AssetService => gameService.AssetService;
    private CoroutineService CoroutineService => gameService.CoroutineService;
    private GameDataService GameDataService => gameService.GameDataService;

    //Data
    private AsyncOperationHandle<SceneInstance> currentMainScene;
    private AsyncOperationHandle<SceneInstance> currentRoomScene;
    private Dictionary<string, AsyncOperationHandle<SceneInstance>> additiveScenes;

    public string LastRoomName { get; private set; }

    #endregion

    #region Service Methods

    public void LoadScene(string sceneName, Action callback = null)
    {
        CoroutineService.PlayCoroutine(CoroutineToken.SceneLoad, CO_LoadScene(sceneName, callback));
    }

    public void LoadSceneAdditive(string sceneName, Action callback = null)
    {
        CoroutineService.PlayCoroutine(CoroutineToken.SceneLoad, CO_LoadSceneAdditive(sceneName, callback));
    }

    public void LoadRoom(string sceneName, bool clearLastRoom = false, Action callback = null)
    {
        CoroutineService.PlayCoroutine(CoroutineToken.SceneLoad, CO_LoadRoom(sceneName, clearLastRoom, callback));
    }

    public void SetLastRoomName() => LastRoomName = SceneManager.GetSceneAt(1).name;

    public string GetCurrentRoomName() => SceneManager.GetSceneAt(1).name;

    public RoomSaveInfo GetCurrentRoomSaveInfo() => GameDataService.CurrentData.roomDictionary[GetCurrentRoomName()];

    #endregion

    #region Service Coroutine

    private IEnumerator CO_LoadScene(string sceneName, Action callback = null)
    {
        gameService.EventService.TriggerEvent(new OnSceneStartLoadEvent());

        yield return AssetService.CO_LoadAsset<GameObject>("View - LoadPanel");

        var _panelOperation = Addressables.InstantiateAsync("View - LoadPanel");
        var _loadScreen = _panelOperation.Result.GetComponent<ViewLoadScreen>();

        UnityEngine.Object.DontDestroyOnLoad(_panelOperation.Result);

        yield return _loadScreen.CO_OpenLoadPanel(_panelOperation);

        if (LastRoomName != string.Empty)
        {
            LastRoomName = string.Empty;
            yield return Addressables.UnloadSceneAsync(currentRoomScene);
        }

        currentMainScene = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, false);

        yield return currentMainScene;

        yield return currentMainScene.Result.ActivateAsync();

        callback?.Invoke();
    }

    private IEnumerator CO_LoadSceneAdditive(string sceneName, Action callback = null)
    {
        gameService.EventService.TriggerEvent(new OnSceneStartLoadEvent());

        yield return AssetService.CO_LoadAsset<GameObject>("View - LoadPanel");

        var _panelOperation = Addressables.InstantiateAsync("View - LoadPanel");
        var _loadScreen = _panelOperation.Result.GetComponent<ViewLoadScreen>();

        yield return _loadScreen.CO_OpenLoadPanel(_panelOperation);

        additiveScenes.Add(sceneName, Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive, true));

        yield return additiveScenes[sceneName];

        callback?.Invoke();
    }

    private IEnumerator CO_LoadRoom(string roomName, bool clearLastRoom, Action callback = null)
    {
        gameService.EventService.TriggerEvent(new OnSceneStartLoadEvent());

        yield return AssetService.CO_LoadAsset<GameObject>("View - LoadPanel");

        var _panelOperation = Addressables.InstantiateAsync("View - LoadPanel");
        var _loadScreen = _panelOperation.Result.GetComponent<ViewLoadScreen>();

        yield return _loadScreen.CO_OpenLoadPanel(_panelOperation);

        if (LastRoomName != string.Empty) yield return Addressables.UnloadSceneAsync(currentRoomScene);

        if (clearLastRoom) LastRoomName = string.Empty;

        currentRoomScene = Addressables.LoadSceneAsync(roomName, LoadSceneMode.Additive, true);

        yield return currentRoomScene;

        callback?.Invoke();
    }

    #endregion
}
