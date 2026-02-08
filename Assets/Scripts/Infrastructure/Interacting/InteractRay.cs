using UnityEngine;

public class InteractRay : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField] private LayerMask _layerMask;
    
    private GameObject _hit;
    private RaycastHit _rayHit;
    private Interactable _target;

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

            if (_target == null || _hit == null) return;

            Debug.Log($"hitted: {_hit.name} | InteractRay");

            if (_target.GetActiveState() & _target.GetActDistance() >= _rayHit.distance)
            {
                if (Input.GetKeyDown(_target.GetTriggerKey()))
                {
                    _target.InvokeActions();
                }
            }
        }
    }

}
