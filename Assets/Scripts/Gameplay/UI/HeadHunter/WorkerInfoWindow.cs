using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerInfoWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _iconImage;

    private WorkerData _workerData;

    public void SetData(WorkerData data)
    {
        _workerData = data;
    }

    public void SetUI()
    {
        if (_workerData == null) return;

        _iconImage.sprite = _workerData.Icon;
        _titleText.text = _workerData.TitleName;
        _descriptionText.text = $"{_workerData.Description}" +
            $"\n\n<b>СВЕДЕНИЯ ЗДОРОВЬЯ:</b>" +
            $"\nПол: {ParseGenderFromData()}" +
            $"\nВозраст: {_workerData.Age}" +
            $"\n\n<b>СВЕДЕНИЯ РАБОТНИКА:</b>" +
            $"\nДолжность: {ParseWorkerTypeFromData()}" +
            $"\nСкорость передвижения: {_workerData.MovementSpeed}" +
            $"\nДневная ставка: {_workerData.DaySalary}" +
            $"\nАванс: {_workerData.InstantPay}" +
            $"\n\n<i>Выйдет на смену на следующий рабочий день</i>";
    }

    private string ParseGenderFromData()
    {
        if (_workerData.Gender == GenderEnum.Male) return "Мужской";
        else return "Женский";
    }

    private string ParseWorkerTypeFromData()
    {
        switch (_workerData.Type)
        {
            case WorkerTypeEnum.Consultant:
                return "Консультант";
            case WorkerTypeEnum.WarehouseWorker:
                return "Работник склада";
            case WorkerTypeEnum.Cashier:
                return "Кассир";
            default:
                return "Unknown worker type";
        }
    }
}
