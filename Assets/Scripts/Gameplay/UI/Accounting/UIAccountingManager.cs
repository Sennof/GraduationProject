using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class UIAccountingManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _windowTypeText;
    
    [Inject]
    [SerializeField] private IMoneyBalance _moneyBalance;

    [SerializeField] private GameObject _prefab;

    [SerializeField] private Transform _incomeFolder;
    [SerializeField] private Transform _outcomeFolder;

    private List<GameObject> _generatedIncomeCards = new();
    private List<GameObject> _generatedOutcomeCards = new();

    private List<string> _generatedIncomeStrings = new();
    private List<string> _generatedOutcomeStrings = new();

    public void PrepareUI()
    {
        SetIncomeUI();
        SetOutcomeUI();
    }

    public void SetTypeTitle(int id)
    {
        if (id == 1) _windowTypeText.text = "Доходы";
        else _windowTypeText.text = "Расходы";
    }

    public void ClearUI()
    {
        foreach (GameObject obj in _generatedIncomeCards)
            Destroy(obj);
        foreach (GameObject obj in _generatedOutcomeCards)
            Destroy(obj);

        _generatedIncomeStrings.Clear();
        _generatedOutcomeStrings.Clear();

        _generatedIncomeCards.Clear();
        _generatedOutcomeCards.Clear();
    }

    private void SetIncomeUI()
    {
        if (_generatedIncomeStrings == GlobalStatistic.SummaryDailyEarn)
            return;
        List<string> toMake = new List<string>(GlobalStatistic.SummaryDailyEarn);

        foreach(string str in _generatedIncomeStrings)
        {
            toMake.Remove(str);
        }

        foreach(string str in toMake)
        {
            SpawnAccountingLine(_incomeFolder, str);
        }
    }

    private void SetOutcomeUI()
    {
        if (_generatedOutcomeStrings == GlobalStatistic.SummaryDailyExpenses)
            return;
        List<string> toMake = new List<string>(GlobalStatistic.SummaryDailyExpenses);

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
        for(int i = 0; i < parsedData.Count() - 1; i++)
        {
            paymentTitle += parsedData[i];
        }

        GameObject card = Instantiate(_prefab, folder);
        card.GetComponent<UIAccountingLineSetter>().SetData(paymentTitle, Int32.Parse(parsedData[parsedData.Length - 1]));

        _generatedIncomeCards.Add(card);
        _generatedIncomeStrings.Add(data);
    }
}
