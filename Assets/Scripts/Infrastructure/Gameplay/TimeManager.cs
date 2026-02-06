using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public void SetSpeedModifier(int modifier)
    {
        Time.timeScale = modifier;
    }
}
