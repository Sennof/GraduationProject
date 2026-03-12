using System.Collections;
using UnityEngine;

public class SunAngleSetter : MonoBehaviour
{
    [SerializeField] private Transform _sun;
    [Space(15)]
    [SerializeField] private Vector3 _defaultAngle; 
    [SerializeField] private Vector3 _sunsetAngle;
    [Space(15)]
    [SerializeField] private int _rotationSteps = 100;
    [Tooltip("in miliseconds")]
    [SerializeField] private int _cooldown = 10;

    private Coroutine _sunsettingCor = null;

    public void Sunrise()
    {
        _sun.eulerAngles = _defaultAngle;
    }

    public void Sunset()
    {
        if (_sunsettingCor != null)
        {
            StopCoroutine(_sunsettingCor);
            _sunsettingCor = null;
        }

        _sunsettingCor = StartCoroutine(Sunsetting());
    }

    private IEnumerator Sunsetting()
    {
        int stepsLeft = _rotationSteps;
        Vector3 needToRotate = _sunsetAngle - _sun.eulerAngles;

        while(stepsLeft > 0)
        {
            _sun.eulerAngles += needToRotate / _rotationSteps;
            stepsLeft--;
            yield return new WaitForSeconds(_cooldown / 100);
        }

        _sunsettingCor = null;
    }
}
