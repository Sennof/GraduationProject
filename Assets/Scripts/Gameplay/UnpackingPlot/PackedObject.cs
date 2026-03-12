using UnityEngine;

[RequireComponent (typeof(Interactable), typeof(ItemObject))]
public class PackedObject : MonoBehaviour
{
    [SerializeField] private GameObject _unpackedObjectPrefab;
    private GameObject _unpackedObject;

    public void Initialize()
    {

    }

    public void UnpackObject()
    {
        // getting essential datas
        Transform targetFolder = transform.GetComponent<ItemObject>().GetDefaultParent();

        //making an object
        _unpackedObject = Instantiate(_unpackedObjectPrefab, transform.position, Quaternion.identity, targetFolder);
        _unpackedObject.GetComponent<ItemObject>().Initialize();
        InitializeInteractingObject();
        InitializeScripts();

        Destroy(gameObject);
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
}