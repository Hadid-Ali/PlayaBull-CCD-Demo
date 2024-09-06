using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SplashMenu : MonoBehaviour
{
    [SerializeField] private AssetLabelReference _MainMenuLabel;

    private AsyncOperationHandle _downloadStatus;

    void OnEnable()
    {
        StartCoroutine(CheckAssets());
    }

    IEnumerator CheckAssets()
    {
        var downloadSizeHandle = Addressables.GetDownloadSizeAsync(_MainMenuLabel);


        yield return downloadSizeHandle;

        if (downloadSizeHandle.Status != AsyncOperationStatus.Succeeded)
            yield break;

        if (downloadSizeHandle.Result > 0)
        {
            Addressables.Release(downloadSizeHandle);
            StartCoroutine(InitiateDownload());
        }
        else
        {
            Addressables.Release(downloadSizeHandle);
            StartCoroutine(CheckUpdate());
        }
    }

    IEnumerator CheckUpdate()
    {
        List<string> catalogsToUpdate = new List<string>();
        AsyncOperationHandle<List<string>> checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
        checkForUpdateHandle.Completed += op => { catalogsToUpdate.AddRange(op.Result); };
        yield return checkForUpdateHandle;
        if (catalogsToUpdate.Count > 0)
        {
            AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate);
            yield return updateHandle;
        }

        StartCoroutine(LoadLevel(1f));
    }

    IEnumerator InitiateDownload()
    {
        _downloadStatus = default;

        _downloadStatus = Addressables.DownloadDependenciesAsync(_MainMenuLabel);
        _downloadStatus.Completed += OnDownloadComplete;

        while (!_downloadStatus.IsDone)
        {
            GameEvents.AssetLoadingEvents.AssetLoadProgressUpdated.Raise(_downloadStatus.PercentComplete);
            yield return null;
        }
    }

    void OnDownloadComplete(AsyncOperationHandle handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(handle);
            StartCoroutine(LoadLevel(1f));
        }
        else
        {
            Addressables.Release(handle);
        }
    }

    IEnumerator LoadLevel(float time = 0f)
    {
        yield return new WaitForSeconds(time);

        GameEvents.AssetLoadingEvents.AssetLoaded.Raise();
        AsyncOperationHandle<SceneInstance> loadHandle = default;

        loadHandle = Addressables.LoadSceneAsync(_MainMenuLabel);
        
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
        }
    }
}