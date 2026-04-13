using UnityEngine;

public class MenuEntryPoint : BaseEntryPoint
{
    private void Awake()
    {
        InitializeAll<HomoObjectSwitcher>();

        InitializeAll<UIVersionSetter>();
    }
}
