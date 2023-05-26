using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineService : MonoBehaviour
{
    #region Variables

    private GameService gameService;

    //Coroutine dictionary
    private Dictionary<string, Coroutine> coroDictionary = new();

    #endregion

    #region Base Methods

    public void Initialize(GameService _gameService)
    {
        gameService = _gameService;
    }

    #endregion

    #region Service Methods

    public void PlayCoroutine(string key, IEnumerator coro)
    {
        if (coroDictionary.TryGetValue(key, out Coroutine result))
        {
            StopCoroutine(result);
            coroDictionary.Remove(key);
            coroDictionary.Add(key, StartCoroutine(CO_PlayCoroutine(key, coro)));
        }
        else
        {
            coroDictionary.Add(key, StartCoroutine(CO_PlayCoroutine(key, coro)));
        }
    }

    private IEnumerator CO_PlayCoroutine(string key, IEnumerator coro)
    {
        yield return coro;

        coroDictionary.Remove(key);
    }

    public void CancelCoroutine(string key)
    {
        if (coroDictionary.TryGetValue(key, out Coroutine result))
        {
            StopCoroutine(result);
            coroDictionary.Remove(key);
        }
    }

    #endregion
}

public static class CoroutineToken
{
    public static string SceneLoad = "SCENE-LOAD_";
    public static string AssetLoad = "ASSET-LOAD_";
    public static string UIPanel = "UI-PANEL_";
    public static string UISlider = "UI-SLIDER_";
}