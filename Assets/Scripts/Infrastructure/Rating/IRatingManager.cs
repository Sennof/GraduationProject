using System.Collections.Generic;

public interface IRatingManager
{
    public void AddRating(float value);

    public void ReduceRating(float value);

    public void SetRating(float value);

    public float GetRating();

    public void AddFeedback(string feedback);

    public List<string> GetFeedbacks();
}