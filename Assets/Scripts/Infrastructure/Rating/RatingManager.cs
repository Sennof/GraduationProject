using System.Collections.Generic;
using UnityEngine;

public class RatingManager : MonoBehaviour, IRatingManager, IInitializeable
{
    [SerializeField] private UIRatingManager _ui;
    [SerializeField] private List<string> _feedbacks = new();
    [Range(0, 5)] private float _rating = 0f;

    public void Initialize()
    {
        //SAVINGSYS (probaply just get from GameData)
        _rating = GlobalStatsBridge.Instance.GetRating();
        _ui.SetText(_rating);
    }

    public void AddRating(float value)
    {
        _rating += value;
        _ui.SetText(_rating);
        CheckRating();

        GlobalStatsBridge.Instance.SetRating(_rating);
    }

    public void ReduceRating(float value)
    {
        _rating -= value;
        _ui.SetText(_rating);
        CheckRating();

        GlobalStatsBridge.Instance.SetRating(_rating);
    }

    public void SetRating(float value)
    {
        _rating = value;
        _ui.SetText(_rating);
        CheckRating();

        GlobalStatsBridge.Instance.SetRating(_rating);
    }

    public void AddFeedback(string feedback) => _feedbacks.Add(feedback); 

    public List<string> GetFeedbacks() => _feedbacks;

    public float GetRating() => _rating;

    private void CheckRating()
    {
        if (_rating > 5) _rating = 5;
        else if (_rating < 0) _rating = 0;

        if (_rating >= 0 && _rating < 1)
            EventBus<OnRatingLevelChange>.Raise(new OnRatingLevelChange { Level = LevelsEnum.Level0 });
        else if (_rating >= 1 && _rating < 2)
            EventBus<OnRatingLevelChange>.Raise(new OnRatingLevelChange { Level = LevelsEnum.Level1 });
        else if (_rating >= 2 && _rating < 3)
            EventBus<OnRatingLevelChange>.Raise(new OnRatingLevelChange { Level = LevelsEnum.Level2 });
        else if (_rating >= 3 && _rating < 4)
            EventBus<OnRatingLevelChange>.Raise(new OnRatingLevelChange { Level = LevelsEnum.Level3 });
        else if (_rating >= 4 && _rating < 5)
            EventBus<OnRatingLevelChange>.Raise(new OnRatingLevelChange { Level = LevelsEnum.Level4 });
        else if (_rating >= 5)
            EventBus<OnRatingLevelChange>.Raise(new OnRatingLevelChange { Level = LevelsEnum.Level5 });
    }

}
