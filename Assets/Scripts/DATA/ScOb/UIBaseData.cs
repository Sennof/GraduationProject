using UnityEngine;

[CreateAssetMenu(fileName = "UIBaseData", menuName = "UIBaseData", order = 0)]
public class UIBaseData : ScriptableObject
{
    #region UI
    [Header("UI")]

    [Tooltip("Main text")]
    public string TitleName;
    [Tooltip("Additional text")]
    public string Description;
    [Tooltip("Displayed icon")]
    public Sprite Icon;
    #endregion
}
