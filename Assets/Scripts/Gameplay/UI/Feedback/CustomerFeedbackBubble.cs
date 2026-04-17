using System.Collections;
using TMPro;
using UnityEngine;

public class CustomerFeedbackBubble : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [Tooltip("Canvas group for fading control.")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [Tooltip("Text component for displaying message.")]
    [SerializeField] private TMP_Text _messageText;
    [Tooltip("Background object (optional).")]
    [SerializeField] private GameObject _background;

    [Header("Settings")]
    [Tooltip("Characters per second for typewriter effect.")]
    [SerializeField] private float _typewriterSpeed = 30f;
    [Tooltip("Time to show message after typing finishes.")]
    [SerializeField] private float _displayDuration = 2.5f;
    [Tooltip("Offset from the agent's pivot (usually above head).")]
    [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 2.2f, 0f);

    [Header("Billboard")]
    [Tooltip("Camera to face (defaults to Camera.main).")]
    [SerializeField] private Camera _targetCamera;

    private Transform _agentTransform;
    private Coroutine _currentAnimation;
    private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        if (_targetCamera == null)
            _targetCamera = Camera.main;

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        if (_background != null)
            _background.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_agentTransform == null) return;

        transform.position = _agentTransform.position + _worldOffset;
        FaceCamera();
    }

    #endregion


    #region Public Methods

    /// <summary>
    /// Initializes the bubble with the agent's transform.
    /// </summary>
    public void Initialize(Transform agentTransform)
    {
        _agentTransform = agentTransform;
        transform.SetParent(null); // Keep in world space
        transform.position = _agentTransform.position + _worldOffset;
    }

    /// <summary>
    /// Shows a feedback message above the agent's head with typewriter effect.
    /// </summary>
    public void ShowMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_currentAnimation != null)
            StopCoroutine(_currentAnimation);

        _currentAnimation = StartCoroutine(AnimateMessage(message));
    }

    /// <summary>
    /// Immediately hides the bubble.
    /// </summary>
    public void HideImmediate()
    {
        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
            _currentAnimation = null;
        }

        _canvasGroup.alpha = 0f;
        if (_background != null) _background.SetActive(false);
    }

    #endregion


    #region Private Methods

    private void FaceCamera()
    {
        if (_targetCamera == null) return;

        Vector3 direction = _targetCamera.transform.position - transform.position;
        direction.y = 0f;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    #endregion


    #region Coroutines

    private IEnumerator AnimateMessage(string fullMessage)
    {
        _messageText.text = "";
        _canvasGroup.alpha = 1f;
        if (_background != null) _background.SetActive(true);

        // Typewriter effect
        int totalChars = fullMessage.Length;
        float charDelay = 1f / _typewriterSpeed;

        for (int i = 0; i <= totalChars; i++)
        {
            _messageText.text = fullMessage.Substring(0, i);
            yield return new WaitForSeconds(charDelay);
        }

        // Hold
        yield return new WaitForSeconds(_displayDuration);

        // Fade out (quick)
        float fadeDuration = 0.2f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        if (_background != null) _background.SetActive(false);

        _currentAnimation = null;
    }

    #endregion
}