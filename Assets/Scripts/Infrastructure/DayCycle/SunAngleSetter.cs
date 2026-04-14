using System.Collections;
using UnityEngine;

public class SunAngleSetter : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [Tooltip("Transform of the sun.")]
    [SerializeField] private Transform _sun;

    [Space(15)]
    [Header("Angles")]
    [Tooltip("Default sun rotation (sunrise).")]
    [SerializeField] private Vector3 _defaultAngle;
    [Tooltip("Sunset sun rotation.")]
    [SerializeField] private Vector3 _sunsetAngle;

    [Space(15)]
    [Header("Animation")]
    [Tooltip("Number of steps for sunset transition.")]
    [SerializeField] private int _rotationSteps = 100;
    [Tooltip("Cooldown between steps in milliseconds.")]
    [SerializeField] private int _cooldown = 10;

    private Coroutine _sunsettingCoroutine = null;

    #endregion


    #region Public Methods

    public void Sunrise()
    {
        _sun.eulerAngles = _defaultAngle;
    }

    public void Sunset()
    {
        if (_sunsettingCoroutine != null)
        {
            StopCoroutine(_sunsettingCoroutine);
            _sunsettingCoroutine = null;
        }

        _sunsettingCoroutine = StartCoroutine(Sunsetting());
    }

    #endregion


    #region Coroutines

    private IEnumerator Sunsetting()
    {
        int stepsLeft = _rotationSteps;
        Vector3 needToRotate = _sunsetAngle - _sun.eulerAngles;

        while (stepsLeft > 0)
        {
            _sun.eulerAngles += needToRotate / _rotationSteps;
            stepsLeft--;
            yield return new WaitForSeconds(_cooldown / 100f);
        }

        _sunsettingCoroutine = null;
    }

    #endregion
}