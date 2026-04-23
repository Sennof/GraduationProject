using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPriceTagHangerPanel : MonoBehaviour
{
    #region Fields

    [Header("UI References")]
    [Tooltip("Parent transform where tag entry rows are instantiated.")]
    [SerializeField] private Transform _listContainer;
    [Tooltip("Prefab for a single tag entry row; requires a Button and TextMeshProUGUI child.")]
    [SerializeField] private GameObject _tagEntryPrefab;
    [Tooltip("Button that transfers the selected tag to the held PriceTagMaker.")]
    [SerializeField] private Button _takeButton;
    [Tooltip("Button that closes the panel without taking a tag.")]
    [SerializeField] private Button _closeButton;
    [Tooltip("Label showing how many tags are stored vs capacity.")]
    [SerializeField] private TextMeshProUGUI _headerText;

    [Header("Events")]
    [Tooltip("Invoked when the panel becomes visible.")]
    [SerializeField] private UnityEvent _onShow;
    [Tooltip("Invoked when the panel is hidden.")]
    [SerializeField] private UnityEvent _onHide;

    private PriceTagHanger _currentHanger;
    private int _selectedIndex = -1;
    private List<GameObject> _entryObjects = new();

    #endregion


    #region Public Methods

    public void Show(PriceTagHanger hanger)
    {
        _currentHanger = hanger;
        _selectedIndex = -1;
        RefreshList();
        gameObject.SetActive(true);
        _onShow?.Invoke();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _currentHanger = null;
        _onHide?.Invoke();
    }

    public void TakeSelectedTag()
    {
        if (_selectedIndex < 0 || _currentHanger == null) return;

        PriceTagMaker maker = FindFirstObjectByType<PriceTagMaker>();
        if (maker == null)
        {
            Debug.Log("[UIPriceTagHangerPanel] No PriceTagMaker found in scene.");
            return;
        }

        PriceTag tag = _currentHanger.TakeTag(_selectedIndex);
        if (tag != null)
        {
            maker.AddConfiguredTagFromHanger(tag);
            _selectedIndex = -1;
            RefreshList();
        }
    }

    public void SelectEntry(int index)
    {
        _selectedIndex = index;
    }

    #endregion


    #region Private Methods

    private void RefreshList()
    {
        foreach (GameObject entry in _entryObjects)
            Destroy(entry);
        _entryObjects.Clear();

        if (_currentHanger == null || _tagEntryPrefab == null) return;

        List<PriceTag> tags = _currentHanger.GetStoredTags();

        if (_headerText != null)
            _headerText.text = $"Tags: {tags.Count} / {_currentHanger.GetCapacity()}";

        for (int i = 0; i < tags.Count; i++)
        {
            GameObject entry = Instantiate(_tagEntryPrefab, _listContainer);
            _entryObjects.Add(entry);

            TextMeshProUGUI label = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && tags[i].GetTargetProduct() != null)
            {
                label.text = $"{tags[i].GetTargetProduct().TitleName}  " +
                             $"+{Mathf.RoundToInt(tags[i].GetMarkup() * 100)}%  " +
                             $"({tags[i].GetEffectivePrice()} $)";
            }

            int captured = i;
            Button btn = entry.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => SelectEntry(captured));
        }
    }

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (_takeButton != null)
            _takeButton.onClick.AddListener(TakeSelectedTag);
        if (_closeButton != null)
            _closeButton.onClick.AddListener(Hide);

        gameObject.SetActive(false);
    }

    #endregion
}
