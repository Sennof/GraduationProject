using System.Collections.Generic;
using UnityEngine;

public class FObj : MonoBehaviour
{
    public Transform myTransform;
    [HideInInspector] public bool autoUpdate;
    [HideInInspector] public float partLength = 2.0f;
    [HideInInspector] public float thickness = 0.02f;
    [HideInInspector] public float partDepth = 0.5f;

    //Vertical part
    [HideInInspector] public enum OPTIONS { Regular = 0, Sin = 1 }
    [HideInInspector] public OPTIONS op;
    [HideInInspector] public float sinusMultiplier = 1;
    [HideInInspector] public bool sinusHorizontal = true;
    [HideInInspector] public bool sinusForward = false;
    [HideInInspector] public float verticalPartThickness = 0.02f;
    [HideInInspector] public float verticalPartDepth = 0.4f;
    [HideInInspector] public bool addSideWalls = false;
    [HideInInspector] public float verticalPartZOffset = 0.0f;
    [HideInInspector] public int countOfVert = 1;

    //Other
    [HideInInspector] public bool merged;
    [HideInInspector] public GameObject back;
    [HideInInspector] public List<GameObject> allFrontPartsGO = new List<GameObject>();
    [HideInInspector] public List<GameObject> allHorizontalPartsGO = new List<GameObject>();
    [HideInInspector] public List<GameObject> allVerticalPartsGO = new List<GameObject>();
    [HideInInspector] public List<GameObject> allObjects = new List<GameObject>();
    [HideInInspector] public bool randomUV;
    [HideInInspector] public bool addObjects;
    [HideInInspector] public bool objsRandomPlacment = true;
    [HideInInspector] public bool objsRandomRotation = true;
    [HideInInspector] public float objsMaxRotation = 360f;
    [HideInInspector] public bool frontSide;
    [HideInInspector] public bool FSrandomPlacement;
    [HideInInspector] public float FSthickness = 0.01f;
    [HideInInspector] public bool FSrandomRotation;
    [HideInInspector] public float FSmaxRotation = 45f;
    [HideInInspector] public bool backSide;
    [HideInInspector] public float GapY = 0.7f;
    [HideInInspector] public int Count = 1;
    [HideInInspector] public Material horizontalMaterial;
    [HideInInspector] public Material verticalMaterial;
    [HideInInspector] public Material backSideMaterial;
    [HideInInspector] public Material frontSideMaterial;

    private void OnValidate()
    {
        myTransform = transform;
    }

}
