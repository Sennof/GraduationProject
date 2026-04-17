using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShelfConfig : MonoBehaviour, IInitializeable
{
    #region Singleton

    public static UIShelfConfig Instance { get; private set; }

    #endregion


    #region Fields

    [Header("UI Elements")]
    [Tooltip("Main panel.")]
    [SerializeField] private GameObject _panel;
    [Tooltip("Button to delete shelf.")]
    [SerializeField] private Button _deleteButton;
    [Tooltip("Button to relocate shelf.")]
    [SerializeField] private Button _relocateButton;
    [Tooltip("Button to toggle visitability.")]
    [SerializeField] private Button _toggleVisitableButton;
    [Tooltip("Text on toggle button.")]
    [SerializeField] private TMP_Text _toggleButtonText;
    [Tooltip("Button to close menu.")]
    [SerializeField] private Button _closeButton;

    private Shelf _currentShelf;
    private ShelfConfigurator _configurator;

    private bool _isInitialized = false;

    #endregion


    #region IInitializeable

    public void Initialize()
    {
        if (_isInitialized) return;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _deleteButton.onClick.AddListener(OnDeleteClicked);
        _relocateButton.onClick.AddListener(OnRelocateClicked);
        _toggleVisitableButton.onClick.AddListener(OnToggleVisitableClicked);
        _closeButton.onClick.AddListener(OnCloseClicked);
        _panel.SetActive(false);

        _isInitialized = true;
    }

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (!_isInitialized)
            Initialize();
    }

    private void Update()
    {
        if (_panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OnCloseClicked();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    #endregion


    #region Public Methods

    public void Show(Shelf shelf, ShelfConfigurator configurator)
    {
        if (shelf == null || configurator == null)
        {
            Debug.LogError("UIShelfConfig.Show: shelf or configurator is null");
            return;
        }

        _currentShelf = shelf;
        _configurator = configurator;
        UpdateButtonsState();
        UpdateToggleText();
        _panel.SetActive(true);

        if (CursorManager.Instance != null)
            CursorManager.Instance.ShowCursor();
        if (PlayerServicesManager.Instance != null)
            PlayerServicesManager.Instance.SetOffTotal();
    }

    public void Hide()
    {
        _panel.SetActive(false);
        if (CursorManager.Instance != null)
            CursorManager.Instance.HideCursor();
        if (PlayerServicesManager.Instance != null)
            PlayerServicesManager.Instance.SetOnTotal();

        _currentShelf = null;
        _configurator = null;
    }

    #endregion


    #region Private Methods

    private void UpdateButtonsState()
    {
        bool shopClosed = !GlobalStatsBridge.Instance.GetShopOpenClosed();
        bool hasValidTarget = _currentShelf != null && _configurator != null;

        _deleteButton.interactable = shopClosed && hasValidTarget;
        _relocateButton.interactable = shopClosed && hasValidTarget;
        _toggleVisitableButton.interactable = hasValidTarget;
    }

    private void UpdateToggleText()
    {
        if (_currentShelf != null)
        {
            _toggleButtonText.text = _currentShelf.IsVisitable() ? "Make Private" : "Make Public";
        }
    }

    private void OnDeleteClicked()
    {
        if (_currentShelf == null || _configurator == null)
        {
            Debug.LogError("Cannot delete: shelf or configurator is null");
            Hide();
            return;
        }

        ShelfConfigurator configurator = _configurator;
        Hide();
        configurator.DeleteShelf();
    }

    private void OnRelocateClicked()
    {
        if (_currentShelf == null || _configurator == null)
        {
            Debug.LogError("Cannot relocate: shelf or configurator is null");
            Hide();
            return;
        }

        ShelfConfigurator configurator = _configurator;
        Hide();
        configurator.StartRelocation();
    }

    private void OnToggleVisitableClicked()
    {
        if (_currentShelf != null)
        {
            _currentShelf.SetVisitable(!_currentShelf.IsVisitable());
            UpdateToggleText();
        }
    }

    private void OnCloseClicked()
    {
        Hide();
    }

    #endregion
}