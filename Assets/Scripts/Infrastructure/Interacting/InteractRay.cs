using TMPro;
using UnityEngine;

public class InteractRay : MonoBehaviour
{
    [SerializeField] private TMP_Text _hintText;

    [SerializeField] private bool _enabled = true;
    [SerializeField] private LayerMask _layerMask;
    
    private GameObject _hit;
    private RaycastHit _rayHit;
    private Interactable _target;

    public void TurnOff() => _enabled = false;

    public void TurnOn() => _enabled = true;

    private void Update()
    {
        if (_enabled) Raycasting();
    }

    private void Raycasting()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _rayHit, 10, _layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _rayHit.distance, Color.yellow);
            if (_hit != _rayHit.collider.gameObject)
            {
                _hit = _rayHit.collider.gameObject;
                _target = _hit.GetComponent<Interactable>();
            }

            if (_target == null || _hit == null)
            {
                _hintText.text = "";
                return;
            }

            Debug.Log($"hitted: {_hit.name} | InteractRay");

            if (_target.GetActiveState() & _target.GetActDistance() >= _rayHit.distance)
            {
                _hintText.text = $"ֽאזלטעו: {_target.GetTriggerKey()}";

                if (Input.GetKeyDown(_target.GetTriggerKey()))
                {
                    _target.InvokeActions();
                }
            }
            else
            {
                _hintText.text = "";
            }
        }
        else
        {
            _hintText.text = "";
        }
    }

}
