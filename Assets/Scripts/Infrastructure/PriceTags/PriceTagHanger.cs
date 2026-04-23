using System.Collections.Generic;
using UnityEngine;

public class PriceTagHanger : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Maximum number of price tags this hanger can store.")]
    [SerializeField] private int _capacity = 10;

    [Header("UI")]
    [Tooltip("Panel displayed when the player interacts with this hanger.")]
    [SerializeField] private UIPriceTagHangerPanel _panel;

    [Header("State")]
    [Tooltip("Tags currently stored in this hanger.")]
    [SerializeField][ReadOnly] private List<PriceTag> _storedTags = new();

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (_panel != null)
            _panel.gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        if (_panel != null)
            _panel.Show(this);
    }

    public bool TryStoreTag(PriceTag tag)
    {
        if (tag == null || _storedTags.Count >= _capacity) return false;

        _storedTags.Add(tag);
        tag.transform.SetParent(transform);
        tag.gameObject.SetActive(false);
        return true;
    }

    public PriceTag TakeTag(int index)
    {
        if (index < 0 || index >= _storedTags.Count) return null;

        PriceTag tag = _storedTags[index];
        _storedTags.RemoveAt(index);
        return tag;
    }

    public List<PriceTag> GetStoredTags() => _storedTags;

    public int GetStoredCount() => _storedTags.Count;

    public int GetCapacity() => _capacity;

    #endregion
}
