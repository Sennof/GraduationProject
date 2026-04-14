using UnityEngine;

[CreateAssetMenu(fileName = "WorkerData", menuName = "BaseInGameData/WorkerData", order = 20)]
public class WorkerData : InGameBaseData
{
    #region WorkerData

    [Header("WorkerData")]
    [Tooltip("Gender displayed in UI.")]
    public GenderEnum Gender;

    [Tooltip("Age displayed in UI.")]
    [Range(18, 120)] public int Age;

    [Tooltip("Movement speed of the worker.")]
    [Range(0.1f, 10f)] public float MovementSpeed;

    [Tooltip("Type of worker (affects position).")]
    public WorkerTypeEnum Type;

    [Tooltip("Salary paid per day.")]
    public int DaySalary;

    [Tooltip("One-time payment upon hiring.")]
    public int InstantPay;

    #endregion
}