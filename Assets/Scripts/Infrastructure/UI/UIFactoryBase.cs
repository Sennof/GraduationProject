using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIFactoryBase : MonoBehaviour, IInitializable
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Path to resources folder.")]
    [SerializeField] private string _dataPath;

    [Header("Prefabs")]
    [Tooltip("Prefab for UI card.")]
    [SerializeField] private GameObject _prefab;

    [Header("Folders")]
    [Tooltip("Target folder for generated cards.")]
    [SerializeField] private Transform _targetFolder;

    [SerializeField] protected List<GameObject> _generatedCards = new();
    protected UIBaseData[] _objectDatas;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (_dataPath == null)
        {
            Debug.LogError($"Missing data path | UIFactory\n{gameObject.name}");
            return;
        }

        DestroyUI();
        GenerateUI();
    }

    #endregion


    #region Private Methods

    private void GenerateUI()
    {
        _objectDatas = Resources.LoadAll<UIBaseData>(_dataPath);

        if (_objectDatas == null)
        {
            Debug.LogError($"Failed to load data | UIFactory\n{gameObject.name}");
            return;
        }

        for (int i = 0; i < _objectDatas.Length; i++)
        {
            GameObject obj = Instantiate(_prefab, _targetFolder);
            _generatedCards.Add(obj);
        }
    }

    private void DestroyUI()
    {
        foreach (GameObject obj in _generatedCards)
        {
            Destroy(obj);
        }

        _generatedCards.Clear();
    }

    #endregion
}