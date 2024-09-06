using System;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LoadingUIHandler : MonoBehaviour
{
    [SerializeField] private Image _progressBar;
    [SerializeField] private Text _statusText;
    [SerializeField] private GameObject _loadingPanel;

    private StringBuilder _statusTextStringBuilder = new();

    private void Start()
    {
        AssertValues();
    }
    
    private void OnEnable()
    {
        GameEvents.AssetLoadingEvents.AssetLoadProgressUpdated.Register(OnAssetLoadProgressUpdated);
        GameEvents.AssetLoadingEvents.AssetLoaded.Register(OnAssetLoaded);
    }

    private void OnDisable()
    {
        GameEvents.AssetLoadingEvents.AssetLoadProgressUpdated.UnRegister(OnAssetLoadProgressUpdated);
        GameEvents.AssetLoadingEvents.AssetLoaded.Unregister(OnAssetLoaded);
    }

    private void AssertValues()
    {
        Assert.IsNull(_progressBar,"Progress Bar Has a Missing Reference");
        Assert.IsNull(_statusText,"Status Text Has a Missing Reference");
        Assert.IsNull(_loadingPanel,"Loading Panel Has a Missing Reference");
    }
    
    private void OnAssetLoadProgressUpdated(float progress)
    {
        int currentValue = (int)(1f + progress * 100);
        
        _statusTextStringBuilder.Clear();
        _statusTextStringBuilder.Append($"Loading.... {currentValue}");
        
        _progressBar.fillAmount = progress;
        _statusText.text = _statusTextStringBuilder.ToString();
    }

    private void OnAssetLoaded()
    {
        if (_loadingPanel == null)
        {
            return;
        }
        _loadingPanel.SetActive(false);
    }
}
