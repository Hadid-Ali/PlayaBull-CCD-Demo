
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TextureHDLoader : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer; // The target renderer where the texture will be applied
    [SerializeField] private AssetLabelReference lowResTextureKey; // Addressable key for the low-resolution texture
    [SerializeField] private AssetLabelReference highResTextureKey; // Addressable key for the high-resolution texture

    private AsyncOperationHandle<Texture> lowResHandle;
    private AsyncOperationHandle<Texture> highResHandle;

    private void Start()
    {
        // Start loading the low-resolution texture
        LoadLowResTexture();
    }

    private void LoadLowResTexture()
    {
        lowResHandle = Addressables.LoadAssetAsync<Texture>(lowResTextureKey);
        lowResHandle.Completed += OnLowResTextureLoaded;
    }

    private void OnLowResTextureLoaded(AsyncOperationHandle<Texture> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // Apply the low-resolution texture to the target material
            targetRenderer.material.mainTexture = handle.Result;

            // Start loading the high-resolution texture after the low-resolution texture is applied
            LoadHighResTexture();
        }
        else
        {
            Debug.LogError("Failed to load low-resolution texture.");
        }
    }

    private void LoadHighResTexture()
    {
        highResHandle = Addressables.LoadAssetAsync<Texture>(highResTextureKey);
        highResHandle.Completed += OnHighResTextureLoaded;
    }

    private void OnHighResTextureLoaded(AsyncOperationHandle<Texture> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // Apply the high-resolution texture to the target material
            Addressables.Release(lowResHandle);
            targetRenderer.material.mainTexture = handle.Result;
        }
        else
        {
            Debug.LogError("Failed to load high-resolution texture.");
        }
    }

    private void OnDestroy()
    {
        // Release the handles to free up memory
        if (lowResHandle.IsValid())
        {
            Addressables.Release(lowResHandle);
        }

        if (highResHandle.IsValid())
        {
            Addressables.Release(highResHandle);
        }
    }
}
