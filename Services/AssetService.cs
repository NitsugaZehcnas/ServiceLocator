using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections;

public class AssetService : IDisposable
{
	public AssetService(GameService _service)  => gameService = _service;

	#region Variables

	private GameService gameService;

	public Dictionary<string, AsyncOperationHandle> AssetDictionary => assetDictionary;
	private Dictionary<string, AsyncOperationHandle> assetDictionary = new();

	#endregion

	#region Load Asset Methods

	public IEnumerator CO_LoadAsset<T>(string assetName)
	{
		if (assetDictionary.TryGetValue(assetName, out AsyncOperationHandle result)) yield break;

        AsyncOperationHandle operation = Addressables.LoadAssetAsync<T>(assetName);
		assetDictionary.Add(assetName, operation);

		yield return operation;
	}

	#endregion

	#region Release Asset Methods

    public void ReleaseAsset(string assetName)
    {
        if (!assetDictionary.TryGetValue(assetName, out AsyncOperationHandle result)) return;

        assetDictionary.Remove(assetName);
        Addressables.Release(result);
    }

	public void ReleaseAllAssets()
	{
		foreach (var item in assetDictionary) Addressables.Release(item.Value);

		assetDictionary.Clear();
	}

    #endregion

    public void Dispose()
    {
		ReleaseAllAssets();
    }
}
