using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnvironmentSpawner : MonoBehaviour
{
    [SerializeField] private AssetLabelReference _environmentLabel;
    
    private AsyncOperationHandle<GameObject> lowResHandle;
    private GameObject _spawnedEnvironment;

    private void Start()
    {
        // Start loading the low-resolution texture
        StartCoroutine(LoadEnvironment());
    }

    private void OnDestroy()
    {
        Addressables.ReleaseInstance(_spawnedEnvironment);
    }

    IEnumerator LoadEnvironment()
    {
        lowResHandle = Addressables.LoadAssetAsync<GameObject>(_environmentLabel);
        lowResHandle.Completed += OnEnvironmentLoaded;
        

        while (!lowResHandle.IsDone)
        {
            GameEvents.AssetLoadingEvents.AssetLoadProgressUpdated.Raise(lowResHandle.PercentComplete);
            yield return null;
        }
    }

    private void OnEnvironmentLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _spawnedEnvironment = handle.Result;
            Addressables.InstantiateAsync(_environmentLabel);
            
            GameEvents.AssetLoadingEvents.AssetLoaded.Raise();
        }
        else
        {
            Debug.LogError("Failed to load low-resolution texture.");
        }
    }
}
