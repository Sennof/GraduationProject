using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interactable), typeof(ItemObject))]
public class PackedObject : MonoBehaviour
{
    #region Fields

    [Header("Settings")]
    [Tooltip("Prefab to instantiate when unpacked.")]
    [SerializeField] private GameObject _unpackedObjectPrefab;

    private GameObject _unpackedObject;
    private Coroutine _unpackingCoroutine = null;

    #endregion


    #region Public Methods

    public void Initialize()
    {
    }

    public void UnpackObject()
    {
        if (_unpackingCoroutine != null)
        {
            StopCoroutine(_unpackingCoroutine);
            _unpackingCoroutine = null;
        }

        _unpackingCoroutine = StartCoroutine(UnpackingRoutine());
    }

    #endregion


    #region Private Methods

    private void InitializeInteractingObject()
    {
        if (_unpackedObject.GetComponent<InteractingObject>())
        {
            EntryPoint.Instance.InitializeInteractingObjects();
        }
    }

    private void InitializeScripts()
    {
        IInitializeable[] initScripts = _unpackedObject.GetComponents<IInitializeable>();
        foreach (var script in initScripts)
        {
            if ((Object)script != this)
            {
                script.Initialize();
            }
        }
    }

    #endregion


    #region Coroutines

    private IEnumerator UnpackingRoutine()
    {
        Transform targetFolder = transform.GetComponent<ItemObject>().GetDefaultParent();
        transform.DOScale(0, 0.5f);
        transform.DORotate(new Vector3(0, 540, 0), 0.55f, RotateMode.FastBeyond360);

        yield return new WaitForSeconds(0.5f);

        _unpackedObject = Instantiate(_unpackedObjectPrefab, transform.position, Quaternion.identity, targetFolder);
        _unpackedObject.GetComponent<ItemObject>().Initialize();
        InitializeInteractingObject();
        InitializeScripts();

        Destroy(gameObject);
    }

    #endregion
}