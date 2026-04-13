using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Interactable), typeof(ItemObject))]
public class PackedObject : MonoBehaviour
{
    [SerializeField] private GameObject _unpackedObjectPrefab;
    private GameObject _unpackedObject;

    private Coroutine _unpackingCor = null;
    public void Initialize()
    {

    }

    public void UnpackObject()
    {
        if(_unpackingCor != null)
        {
            StopCoroutine(_unpackingCor);
            _unpackingCor = null;
        } 

        _unpackingCor = StartCoroutine(UnpackingRoutine());
    }

    private void InitializeInteractingObject()
    {
       if(_unpackedObject.GetComponent<InteractingObject>()) 
            EntryPoint.Instance.InitializeInteractingObjects();
    }

    private void InitializeScripts()
    {
        IInitializeable[] initScripts = _unpackedObject.GetComponents<IInitializeable>();
        foreach (var script in initScripts)
        {
            if ((Object)script != this)
                script.Initialize();
        }
    }

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
}