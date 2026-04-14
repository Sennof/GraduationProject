using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class UIAccountingManager : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text displaying window type.")]
    [SerializeField] private TMP_Text _windowTypeText;

    [Inject]
    [Tooltip("Money balance service.")]
    [SerializeField] private IMoneyBalance _moneyBalance;

    [Header("Prefabs")]
    [Tooltip("Prefab for accounting line.")]
    [SerializeField] private GameObject _prefab;

    [Header("Folders")]
    [Tooltip("Folder for income lines.")]
    [SerializeField] private Transform _incomeFolder;
    [Tooltip("Folder for outcome lines.")]
    [SerializeField] private Transform _outcomeFolder;

    private List<GameObject> _generatedIncomeCards = new();
    private List<GameObject> _generatedOutcomeCards = new();

    private List<string> _generatedIncomeStrings = new();
    private List<string> _generatedOutcomeStrings = new();

    #endregion


    #region Public Methods

    public void PrepareUI()
    {
        SetIncomeUI();
        SetOutcomeUI();
    }

    public void SetTypeTitle(int id)
    {
        if (id == 1)
        {
            _windowTypeText.text = "Income";
        }
        else
        {
            _windowTypeText.text = "Expenses";
        }
    }

    public void ClearUI()
    {
        foreach (GameObject obj in _generatedIncomeCards)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in _generatedOutcomeCards)
        {
            Destroy(obj);
        }

        _generatedIncomeStrings.Clear();
        _generatedOutcomeStrings.Clear();

        _generatedIncomeCards.Clear();
        _generatedOutcomeCards.Clear();
    }

    #endregion


    #region Private Methods

    private void SetIncomeUI()
    {
        if (_generatedIncomeStrings == GlobalStatsBridge.Instance.GetSummaryDailyEarn())
        {
            return;
        }

        List<string> toMake = new List<string>(GlobalStatsBridge.Instance.GetSummaryDailyEarn());

        foreach (string str in _generatedIncomeStrings)
        {
            toMake.Remove(str);
        }

        foreach (string str in toMake)
        {
            SpawnAccountingLine(_incomeFolder, str);
        }
    }

    private void SetOutcomeUI()
    {
        if (_generatedOutcomeStrings == GlobalStatsBridge.Instance.GetSummaryDailyExpenses())
        {
            return;
        }

        List<string> toMake = new List<string>(GlobalStatsBridge.Instance.GetSummaryDailyExpenses());

        foreach (string str in _generatedOutcomeStrings)
        {
            toMake.Remove(str);
        }

        foreach (string str in toMake)
        {
            SpawnAccountingLine(_outcomeFolder, str);
        }
    }

    private void SpawnAccountingLine(Transform folder, string data)
    {
        string[] parsedData = data.Split();
        string paymentTitle = "";
        for (int i = 0; i < parsedData.Length - 1; i++)
        {
            paymentTitle += parsedData[i] + " ";
        }

        GameObject card = Instantiate(_prefab, folder);
        card.GetComponent<UIAccountingLineSetter>().SetData(paymentTitle, int.Parse(parsedData[parsedData.Length - 1]));

        _generatedIncomeCards.Add(card);
        _generatedIncomeStrings.Add(data);
    }

    #endregion
}