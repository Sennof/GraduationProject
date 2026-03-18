using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIFeedbackManager : MonoBehaviour
{
    [SerializeField] private Transform _folder;
    [SerializeField] private GameObject _cardPrefab;

    [Inject] private IRatingManager _ratingManager;
    private List<string> _generatedStrings = new();
    private List<GameObject> _generatedCards = new();

    public void GenerateFeedbacks()
    {
        List<string> toGen = new List<string>(_ratingManager.GetFeedbacks());
        if (_generatedStrings == toGen) return;

        foreach(string str in _generatedStrings)
        {
            toGen.Remove(str);
        }

        foreach (string str in toGen)
        {
            GameObject obj = Instantiate(_cardPrefab, _folder);
            obj.GetComponent<UIFeedbackCard>().Intialize(str);

            _generatedCards.Add(obj);
            _generatedStrings.Add(str);
        }
    }
}
