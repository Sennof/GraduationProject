using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIFactoryBase : MonoBehaviour, IInitializable
{
    [SerializeField] private string _dataPath;

    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _targetFolder;

    [SerializeField] protected List<GameObject> _generatedCards = new();
    protected UIBaseData[] _objectDatas;

    public void Initialize()
    {
        if (_dataPath == null)
        {
            Debug.LogError($"Some nessessary data is missing | UIFactory\n{gameObject.name}");
            return;
        }

        DestroyUI();
        GenerateUI();
    }

    private void GenerateUI()
    {
        _objectDatas = Resources.LoadAll<UIBaseData>(_dataPath);

        if(_objectDatas == null)
        {
            Debug.LogError($"Failed to load data | UIFactory\n{gameObject.name}");
            return;
        }

        for(int i = 0; i < _objectDatas.Length; i++)
        {
            GameObject obj = Instantiate(_prefab, _targetFolder);
            _generatedCards.Add(obj);
        }
    }

    private void DestroyUI()
    {
        foreach (GameObject obj in _generatedCards)
            Destroy(obj);


        _generatedCards.Clear();
    }
}
