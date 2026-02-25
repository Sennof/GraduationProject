using Unity.VisualScripting;
using UnityEngine;

public class ShopUIFactory : UIFactoryBase, IInitializable
{
    public new void Initialize()
    {
        base.Initialize();
        InitializeObjectDatas();
    }

    private void InitializeObjectDatas()
    {
        for(int i = 0; i < _generatedCards.Count; i++)
        {
            _generatedCards[i].GetComponent<UIProductCard>().Initialize((_objectDatas[i] as ProductData));
        }
    }
}
