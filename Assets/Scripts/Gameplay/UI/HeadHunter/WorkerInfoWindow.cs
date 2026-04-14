using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerInfoWindow : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [Tooltip("Text for worker title.")]
    [SerializeField] private TMP_Text _titleText;
    [Tooltip("Text for worker description.")]
    [SerializeField] private TMP_Text _descriptionText;
    [Tooltip("Image for worker icon.")]
    [SerializeField] private Image _iconImage;

    private WorkerData _workerData;

    #endregion


    #region Public Methods

    public void SetData(WorkerData data)
    {
        _workerData = data;
    }

    public void SetUI()
    {
        if (_workerData == null)
        {
            return;
        }

        _iconImage.sprite = _workerData.Icon;
        _titleText.text = _workerData.TitleName;
        _descriptionText.text = $"{_workerData.Description}" +
            $"\n\n<b>HEALTH DETAILS:</b>" +
            $"\nGender: {ParseGenderFromData()}" +
            $"\nAge: {_workerData.Age}" +
            $"\n\n<b>WORKER DETAILS:</b>" +
            $"\nPosition: {ParseWorkerTypeFromData()}" +
            $"\nMovement Speed: {_workerData.MovementSpeed}" +
            $"\nDaily Rate: {_workerData.DaySalary}" +
            $"\nAdvance: {_workerData.InstantPay}" +
            $"\n\n<i>Will start shift next working day</i>";
    }

    #endregion


    #region Private Methods

    private string ParseGenderFromData()
    {
        if (_workerData.Gender == GenderEnum.Male)
        {
            return "Male";
        }
        else
        {
            return "Female";
        }
    }

    private string ParseWorkerTypeFromData()
    {
        switch (_workerData.Type)
        {
            case WorkerTypeEnum.Consultant:
                return "Consultant";
            case WorkerTypeEnum.WarehouseWorker:
                return "Warehouse Worker";
            case WorkerTypeEnum.Cashier:
                return "Cashier";
            default:
                return "Unknown worker type";
        }
    }

    #endregion
}