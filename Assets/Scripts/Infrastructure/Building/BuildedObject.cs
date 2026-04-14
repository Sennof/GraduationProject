using System.Collections.Generic;
using UnityEngine;

public class BuildedObject : MonoBehaviour, IInitializeable
{
    #region Fields

    [Header("Components")]
    [Tooltip("Barrier object that visualizes placement collision.")]
    [SerializeField] private GameObject _buildedObjectBarrier;
    [Tooltip("Renderer of the barrier object.")]
    [SerializeField] private Renderer _buildedObjectBarrierRenderer;
    [Tooltip("Collider of the barrier object.")]
    [SerializeField] private BoxCollider _buildedObjectBarrierCollider;

    [Header("Placement Settings")]
    [Tooltip("Layer mask for obstacles that block placement.")]
    [SerializeField] private LayerMask _obstacleMask;

    private bool _isBuilding = false;
    private GameObject _self;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        _self = gameObject;

        SetBuildedState();
    }

    public bool CheckPlace()
    {
        if (_isBuilding == false)
        {
            return false;
        }

        Transform barrierTransform = _buildedObjectBarrierCollider.transform;
        Vector3 center = barrierTransform.TransformPoint(_buildedObjectBarrierCollider.center);
        Vector3 worldSize = Vector3.Scale(_buildedObjectBarrierCollider.size, barrierTransform.lossyScale);

        Vector3 halfExtents = new Vector3(worldSize.x * 0.5f, 0.5f, worldSize.z * 0.5f);

        Collider[] collisions = Physics.OverlapBox(center, halfExtents, barrierTransform.rotation, _obstacleMask);

        foreach (var col in collisions)
        {
            if (col.transform.IsChildOf(this.transform))
            {
                continue;
            }

            if (col.CompareTag("floor"))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    public void SetBuildedState()
    {
        _buildedObjectBarrierRenderer.material.color = Color.blue;

        DisableBuilding();
        InitializeScripts();
    }

    public void EnableBuilding()
    {
        _buildedObjectBarrier.SetActive(true);
        _isBuilding = true;
    }

    public void DisableBuilding()
    {
        _isBuilding = false;
        _buildedObjectBarrier.SetActive(false);
    }

    public GameObject GetBarrierObject() => _buildedObjectBarrier;

    public void SetActive() => _buildedObjectBarrier.SetActive(true);

    public void SetInactive() => _buildedObjectBarrier.SetActive(false);

    public GameObject GetMe() => _self;

    #endregion


    #region Private Methods

    private void InitializeScripts()
    {
        IInitializeable[] initScripts = GetComponents<IInitializeable>();
        foreach (var script in initScripts)
        {
            if ((Object)script != this)
            {
                script.Initialize();
            }
        }
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        if (_isBuilding == false)
        {
            return;
        }

        if (CheckPlace())
        {
            _buildedObjectBarrierRenderer.material.color = Color.green;
        }
        else
        {
            _buildedObjectBarrierRenderer.material.color = Color.red;
        }
    }

    private void OnDrawGizmos()
    {
        if (_buildedObjectBarrierCollider == null)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Transform t = _buildedObjectBarrierCollider.transform;

        Gizmos.matrix = Matrix4x4.TRS(t.TransformPoint(_buildedObjectBarrierCollider.center), t.rotation, t.lossyScale);

        Gizmos.DrawWireCube(Vector3.zero, _buildedObjectBarrierCollider.size);
    }

    #endregion
}