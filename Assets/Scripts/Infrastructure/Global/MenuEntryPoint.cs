using UnityEngine;

public class MenuEntryPoint : BaseEntryPoint
{
    #region Unity Methods

    private void Awake()
    {
        InitializeAll<HomoObjectSwitcher>();
        InitializeAll<UIVersionSetter>();
    }

    #endregion
}