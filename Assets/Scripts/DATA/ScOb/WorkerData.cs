using UnityEngine;

[CreateAssetMenu(fileName = "WorkerData", menuName = "BaseInGameData/WorkerData", order = 20)]
public class WorkerData : InGameBaseData
{
    [Header("WorkerData")]
    [Tooltip("The gender displayed in the UI")]
    public GenderEnum Gender;
    [Tooltip("The age displayed in the UI")]
    [Range(18, 120)] public int Age;
    [Tooltip("The speed of movement of the employee")]
    [Range(0.1f, 10f)] public float MovementSpeed;
    [Tooltip("Type of worker (affects the position held)")]
    public WorkerTypeEnum Type;
    [Tooltip("The money that the player will have to pay the worker for the shift")]
    public int DaySalary;
    [Tooltip("Prepaid expense")] //when youre hire(just hiring, one time pay) a worker you pay him\her some money
    public int InstantPay;
}
