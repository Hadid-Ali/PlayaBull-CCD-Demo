using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    private void OnEnable()
    {
        GameEvents.AssetLoadingEvents.AssetLoaded.Register(OnAssetLoaded);
    }
    private void OnDisable()
    {
        GameEvents.AssetLoadingEvents.AssetLoaded.Unregister(OnAssetLoaded);
    }

    void OnAssetLoaded()
    {
        _player.SetActive(true);
    }
}
