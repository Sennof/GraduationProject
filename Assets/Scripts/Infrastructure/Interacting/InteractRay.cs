using TMPro;
using UnityEngine;

public class InteractRay : MonoBehaviour
{
    #region Fields

    [Header("UI")]
    [Tooltip("Text field to display interaction hints.")]
    [SerializeField] private TMP_Text _hintText;

    [Header("Settings")]
    [Tooltip("Enable or disable interaction raycasting.")]
    [SerializeField] private bool _enabled = true;
    [Tooltip("Layers the raycast can hit.")]
    [SerializeField] private LayerMask _layerMask;

    private GameObject _hitObject;
    private RaycastHit _rayHit;
    private Interactable _targetInteractable;

    #endregion


    #region Public Methods

    public void TurnOff() => _enabled = false;

    public void TurnOn() => _enabled = true;

    #endregion


    #region Private Methods

    private void Raycasting()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _rayHit, 10, _layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _rayHit.distance, Color.yellow);

            if (_hitObject != _rayHit.collider.gameObject)
            {
                _hitObject = _rayHit.collider.gameObject;
                _targetInteractable = _hitObject.GetComponent<Interactable>();
            }

            if (_targetInteractable == null || _hitObject == null)
            {
                _hintText.text = "";
                return;
            }

            Debug.Log($"Hitted: {_hitObject.name} | InteractRay");

            if (_targetInteractable.GetActiveState() && _targetInteractable.GetActingDistance() >= _rayHit.distance)
            {
                bool usingSideEvents = _targetInteractable.GetStateUsingSideEvents();

                _hintText.text = $"Press: {_targetInteractable.GetMainTriggerKey()}";
                if (usingSideEvents)
                {
                    _hintText.text += $" ({_targetInteractable.GetSideTriggerKey()})";
                }

                if (Input.GetKeyDown(_targetInteractable.GetMainTriggerKey()))
                {
                    _targetInteractable.InvokeMainActions();
                }

                if (usingSideEvents && Input.GetKeyDown(_targetInteractable.GetSideTriggerKey()))
                {
                    _targetInteractable.InvokeSideActions();
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

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (_enabled)
        {
            Raycasting();
        }
    }

    #endregion
}