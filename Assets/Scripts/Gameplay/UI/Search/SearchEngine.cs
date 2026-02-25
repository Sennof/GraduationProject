using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//script for browser
public class SearchEngine : MonoBehaviour, IInitializable
{
    [SerializeField] private HomoObjectSwitcher _mainPageSwitcher;
    [SerializeField] private List<string> _keywords = new();

    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Text _placeHolder; //for hints

    public void Initialize() => ResetSearchSpace("Введите запрос...");

    public void Search()
    {
        if (_inputField.text == "")
            return;

        for (int keysId = 0; keysId < _keywords.Count; keysId++)
        {
            string[] category = _keywords[keysId].Split(" ");
            foreach(string keyword in category)
            {
                if (keyword.Contains(_inputField.text.ToLower()))
                {
                    _mainPageSwitcher.OffCurrent();
                    _mainPageSwitcher.SetOn(keysId);

                    ResetSearchSpace("Введите запрос...");
                    return;
                }
            }
        }

        if (_inputField.text.ToLower().Contains("sennof")) ResetSearchSpace("Привет от разработчка!");
        else ResetSearchSpace("По Вашему запросу ничего не найдено.");
    }

    private void ResetSearchSpace(string placeHolderHint)
    {
        _inputField.text = "";
        _placeHolder.text = placeHolderHint;
    }
}
