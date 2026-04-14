using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SearchEngine : MonoBehaviour, IInitializable
{
    #region Fields

    [Header("References")]
    [Tooltip("Switcher for main pages.")]
    [SerializeField] private HomoObjectSwitcher _mainPageSwitcher;

    [Header("Data")]
    [Tooltip("Keywords for search matching.")]
    [SerializeField] private List<string> _keywords = new();

    [Header("UI")]
    [Tooltip("Input field for search query.")]
    [SerializeField] private TMP_InputField _inputField;
    [Tooltip("Placeholder text for hints.")]
    [SerializeField] private TMP_Text _placeHolder;

    #endregion


    #region Public Methods

    public void Initialize() => ResetSearchSpace("Enter query...");

    public void Search()
    {
        if (_inputField.text == "")
        {
            return;
        }

        for (int keysId = 0; keysId < _keywords.Count; keysId++)
        {
            string[] category = _keywords[keysId].Split(" ");
            foreach (string keyword in category)
            {
                if (keyword.Contains(_inputField.text.ToLower()))
                {
                    _mainPageSwitcher.OffCurrent();
                    _mainPageSwitcher.SetOn(keysId);

                    ResetSearchSpace("Enter query...");
                    return;
                }
            }
        }

        if (_inputField.text.ToLower().Contains("sennof"))
        {
            ResetSearchSpace("Hello from the developer!");
        }
        else
        {
            ResetSearchSpace("Nothing found for your query.");
        }
    }

    #endregion


    #region Private Methods

    private void ResetSearchSpace(string placeHolderHint)
    {
        _inputField.text = "";
        _placeHolder.text = placeHolderHint;
    }

    #endregion
}