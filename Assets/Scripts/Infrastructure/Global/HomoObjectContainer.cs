using UnityEngine;

public class HomoObjectContainer : MonoBehaviour
{
    // the child object which used as a container.û
    private GameObject _homoObject;

    public void Initialize()
    {
        _homoObject = transform.GetChild(0).gameObject;

        if (_homoObject == null) Debug.LogError($"Failed to initialize HomoObject Container | {gameObject.name}");
    }

    public void TurnOff() => _homoObject.SetActive(false);

    public void TurnOn() => _homoObject.SetActive(true);
}
