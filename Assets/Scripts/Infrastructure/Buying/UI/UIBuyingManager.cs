using TMPro;
using UnityEngine;

public class UIBuyingManager : MonoBehaviour
{
    #region Fields

    [Header("UI References")]
    [Tooltip("Folder for UI cards.")]
    [SerializeField] private Transform _uiCardsFolder;
    [Tooltip("Prefab for buying card.")]
    [SerializeField] private GameObject _uiCardPrefab;
    [Tooltip("Text for total price sum.")]
    [SerializeField] private TMP_Text _priceSumText;

    #endregion


    #region Public Methods

    public void Initialize(ProductData[] products)
    {
        foreach (Transform child in _uiCardsFolder)
        {
            Destroy(child.gameObject);
        }

        foreach (ProductData data in products)
        {
            GameObject card = Instantiate(_uiCardPrefab, _uiCardsFolder);
            if (card.TryGetComponent(out UIBuyingCard uiCard))
            {
                uiCard.Initialize(data, this);
            }
        }
    }

    public void SetPriceText(int price) => _priceSumText.text = $"Total: {price}.";

    #endregion
}