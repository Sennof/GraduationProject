using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIPauseMenuSettings : MonoBehaviour
{
    [Header("Page Special")]
    [Inject]
    [SerializeField] private IMoneyBalance _moneyBalance;

    [SerializeField] private Image _cheatsStateSprite;

    public void ChangeState()
    {
        _moneyBalance.ChangeCheatsState();

        if (!_moneyBalance.GetCheatsEnabledState()) _cheatsStateSprite.color = Color.red;
        else _cheatsStateSprite.color = Color.green;
    }
}
