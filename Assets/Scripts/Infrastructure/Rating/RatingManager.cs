using System.Collections.Generic;
using UnityEngine;

public class RatingManager : MonoBehaviour, IRatingManager, IInitializeable
{
    #region Fields

    [Header("UI")]
    [Tooltip("UI component displaying rating.")]
    [SerializeField] private UIRatingManager _ui;

    [Tooltip("List of customer feedback messages.")]
    [SerializeField] private List<string> _feedbacks = new();

    private float _rating = 0f;

    public static RatingManager Instance { get; private set; }

    #endregion


    #region Public Methods

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        _rating = GlobalStatsBridge.Instance.GetRating();
        _rating = Mathf.Round(_rating * 100f) / 100f;
        _ui.SetText(_rating);
    }

    public void AddRating(float value)
    {
        _rating += value;
        ClampAndRoundRating();
        _ui.SetText(_rating);
        GlobalStatsBridge.Instance.SetRating(_rating);
    }

    public void ReduceRating(float value)
    {
        _rating -= value;
        ClampAndRoundRating();
        _ui.SetText(_rating);
        GlobalStatsBridge.Instance.SetRating(_rating);
    }

    public void SetRating(float value)
    {
        _rating = value;
        ClampAndRoundRating();
        _ui.SetText(_rating);
        GlobalStatsBridge.Instance.SetRating(_rating);
    }

    public void AddFeedback(string feedback) => _feedbacks.Add(feedback);

    public List<string> GetFeedbacks() => _feedbacks;

    public float GetRating() => _rating;

    /// <summary>
    /// Applies a cumulative rating change and feedback from a customer session.
    /// </summary>
    public void ApplySessionFeedback(float delta, string feedback)
    {
        if (Mathf.Abs(delta) > 0.001f)
        {
            _rating += delta;
            ClampAndRoundRating();
            _ui.SetText(_rating);
            GlobalStatsBridge.Instance.SetRating(_rating);
        }

        if (!string.IsNullOrEmpty(feedback))
        {
            _feedbacks.Add(feedback);
        }

        CheckRatingLevel();
    }

    #endregion


    #region Private Methods

    private void ClampAndRoundRating()
    {
        _rating = Mathf.Clamp(_rating, 0f, 5f);
        _rating = Mathf.Round(_rating * 100f) / 100f;
    }

    private void CheckRatingLevel()
    {
        LevelsEnum level;
        if (_rating < 1f) level = LevelsEnum.Level0;
        else if (_rating < 2f) level = LevelsEnum.Level1;
        else if (_rating < 3f) level = LevelsEnum.Level2;
        else if (_rating < 4f) level = LevelsEnum.Level3;
        else if (_rating < 5f) level = LevelsEnum.Level4;
        else level = LevelsEnum.Level5;

        EventBus<OnRatingLevelChange>.Raise(new OnRatingLevelChange { Level = level });
    }

    #endregion
}