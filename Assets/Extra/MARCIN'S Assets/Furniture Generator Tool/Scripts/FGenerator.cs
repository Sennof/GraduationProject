using UnityEditor;
using UnityEngine;
using FMeshBuilder;
using System.Collections.Generic;

[CustomEditor(typeof(FObj))]
public class FGenerator : Editor
{
    private SerializedObject m_Object;

    //Horizontal parts data
    private SerializedProperty m_PartLenght;
    private SerializedProperty m_Thickness;
    private SerializedProperty m_PartDepth;

    //Vertical part data
    private SerializedProperty m_Op;
    private SerializedProperty m_SinusMultiplier;
    private SerializedProperty m_SinusHorizontal;
    private SerializedProperty m_SinusForward;
    private SerializedProperty m_VerticalPartThickness;
    private SerializedProperty m_VerticalPartDepth;
    private SerializedProperty m_VerticalPartZOffset;
    private SerializedProperty m_AddSideWalls;
    private SerializedProperty m_CountOfVerticalPart;

    //Other
    private SerializedProperty m_addObjects;
    private SerializedProperty m_objsRandomPlacement;
    private SerializedProperty m_objsRandomRotation;
    private SerializedProperty m_objsMaxRotation;
    private SerializedProperty m_RandomUV;
    private SerializedProperty m_horizontalMaterial;
    private SerializedProperty m_verticalMaterial;
    private SerializedProperty m_backSideMaterial;
    private SerializedProperty m_frontSideMaterial;
    private SerializedProperty m_FrontSide;
    private SerializedProperty m_FSrandomPlacement;
    private SerializedProperty m_FSthickness;
    private SerializedProperty m_FSrandomRotation;
    private SerializedProperty m_FSmaxRotation;
    private SerializedProperty m_BackSide;
    private SerializedProperty m_GapY;
    private SerializedProperty m_Count;
    private SerializedProperty m_Transform;
    private static MeshFilter[] horizontalPartMeshFilters;
    private static MeshFilter[] verticalPartMeshFilters;

    private Transform t;
    bool materialFoldoutHeaderGroup;
    private SerializedProperty m_allFrontPartsGO;
    private SerializedProperty m_allHorizontalPartsGO;
    private SerializedProperty m_allVerticalPartsGO;
    private SerializedProperty m_allObjects;
    private SerializedProperty m_back;
    private SerializedProperty m_merged;

    public void OnEnable()
    {
        m_Object = new SerializedObject(target);

        //Part data
        m_PartLenght = m_Object.FindProperty("partLength");
        m_Thickness = m_Object.FindProperty("thickness");
        m_PartDepth = m_Object.FindProperty("partDepth");

        //Vertical part data
        m_Op = m_Object.FindProperty("op");
        m_SinusMultiplier = m_Object.FindProperty("sinusMultiplier");
        m_SinusHorizontal = m_Object.FindProperty("sinusHorizontal");
        m_SinusForward = m_Object.FindProperty("sinusForward");
        m_VerticalPartThickness = m_Object.FindProperty("verticalPartThickness");
        m_VerticalPartDepth = m_Object.FindProperty("verticalPartDepth");
        m_VerticalPartZOffset = m_Object.FindProperty("verticalPartZOffset");
        m_AddSideWalls = m_Object.FindProperty("addSideWalls");
        m_CountOfVerticalPart = m_Object.FindProperty("countOfVert");

        //Other
        m_merged = m_Object.FindProperty("merged");
        m_back = m_Object.FindProperty("back");
        m_allFrontPartsGO = m_Object.FindProperty("allFrontPartsGO");
        m_allHorizontalPartsGO = m_Object.FindProperty("allHorizontalPartsGO");
        m_allVerticalPartsGO = m_Object.FindProperty("allVerticalPartsGO");
        m_allObjects = m_Object.FindProperty("allObjects");

        m_objsRandomRotation = m_Object.FindProperty("objsRandomRotation");
        m_objsMaxRotation = m_Object.FindProperty("objsMaxRotation");
        m_objsRandomPlacement = m_Object.FindProperty("objsRandomPlacment");
        m_addObjects = m_Object.FindProperty("addObjects");
        m_RandomUV = m_Object.FindProperty("randomUV");
        m_FrontSide = m_Object.FindProperty("frontSide");
        m_FSrandomPlacement = m_Object.FindProperty("FSrandomPlacement");
        m_FSthickness = m_Object.FindProperty("FSthickness");
        m_FSrandomRotation = m_Object.FindProperty("FSrandomRotation");
        m_FSmaxRotation = m_Object.FindProperty("FSmaxRotation");
        m_BackSide = m_Object.FindProperty("backSide");
        m_GapY = m_Object.FindProperty("GapY");
        m_Count = m_Object.FindProperty("Count");
        m_horizontalMaterial = m_Object.FindProperty("horizontalMaterial");
        m_verticalMaterial = m_Object.FindProperty("verticalMaterial");
        m_backSideMaterial = m_Object.FindProperty("backSideMaterial");
        m_frontSideMaterial = m_Object.FindProperty("frontSideMaterial");

        m_Transform = m_Object.FindProperty("myTransform");
        t = (Transform)m_Transform.objectReferenceValue;

        if (t.childCount == 0)
        {
           // RecreateTheObj();
        }
        else {
            if (horizontalPartMeshFilters == null){
                Transform tChild = t.GetChild(0);
                horizontalPartMeshFilters = new MeshFilter[tChild.childCount];
                for (int i = 0; i < tChild.childCount; i++){
                    horizontalPartMeshFilters[i] = tChild.GetChild(i).GetComponent<MeshFilter>();
                }
            }
        }
    }
    public void RecreateTheObj()
    {
        //Debug.Log(Application.dataPath);
        //Delete all
        if (t.childCount > 0) DestroyImmediate(t.GetChild(0).gameObject);

        m_allFrontPartsGO.ClearArray();
        m_allHorizontalPartsGO.ClearArray();
        m_allObjects.ClearArray();
        m_allVerticalPartsGO.ClearArray();
        m_back.objectReferenceValue = null;
        
        m_merged.boolValue = false;
        //Create new
        GameObject horizontalPartsGO = new GameObject();
        horizontalPartsGO.name = "Shelf";
        horizontalPartsGO.transform.SetParent(t);

        horizontalPartMeshFilters = new MeshFilter[m_Count.intValue];

        if(m_CountOfVerticalPart.intValue > 0) 
            verticalPartMeshFilters = new MeshFilter[m_Count.intValue * m_CountOfVerticalPart.intValue - (m_CountOfVerticalPart.intValue)];

        int frontPartCount = 0;
        int objectsCount = 0;
        for (int i = 0; i < m_Count.intValue; i++)
        {
            //Create horizontal part
            GameObject horizontalGO = FMeshGenerator.createGameObject(i.ToString(), horizontalPartsGO.transform);
            horizontalPartMeshFilters[i] = horizontalGO.GetComponent<MeshFilter>();
            horizontalPartMeshFilters[i].mesh = FMeshGenerator.rebuildMesh(
                m_PartDepth.floatValue, 
                m_PartLenght.floatValue,
                m_Thickness.floatValue, 
                m_GapY.floatValue * i,
                0,
                m_RandomUV.boolValue);
            horizontalGO.GetComponent<MeshRenderer>().material = (Material)m_horizontalMaterial.objectReferenceValue;
            m_allHorizontalPartsGO.InsertArrayElementAtIndex(i);
            m_allHorizontalPartsGO.GetArrayElementAtIndex(i).objectReferenceValue = horizontalGO;
            //Front part
            if (i < m_Count.intValue - 1)
            {
                for (int fp = 0; fp < m_CountOfVerticalPart.intValue+1; fp++)
                {
                    //Size of box;
                    Vector3 size = new Vector3(0, 0, 0);
                    size.z = (m_PartLenght.floatValue - m_VerticalPartThickness.floatValue) / (m_CountOfVerticalPart.intValue + 1);
                    size.y = m_GapY.floatValue - m_Thickness.floatValue;
                    size.x = m_VerticalPartDepth.floatValue;

                    Vector3 boxPosition = new Vector3(m_VerticalPartDepth.floatValue, (m_GapY.floatValue * i) + m_Thickness.floatValue, m_VerticalPartThickness.floatValue + (size.z*fp));
                    
                    int random = Random.Range(0, 2);
                    if (m_FrontSide.boolValue)
                    {
                        if (m_FSrandomPlacement.boolValue && random == 0 || !m_FSrandomPlacement.boolValue)
                        {
                            GameObject frontSide = FMeshGenerator.createGameObject(i + "|" + "frontPart", horizontalGO.transform);
                            frontSide.GetComponent<MeshFilter>().mesh = FMeshBuilder.FMeshGenerator.rebuildMesh(m_FSthickness.floatValue, size.z - m_VerticalPartThickness.floatValue, size.y, 0, 0, m_RandomUV.boolValue);
                            frontSide.GetComponent<MeshRenderer>().material = (Material)m_frontSideMaterial.objectReferenceValue;
                            frontSide.transform.localPosition = boxPosition;

                            m_allFrontPartsGO.InsertArrayElementAtIndex(frontPartCount);
                            m_allFrontPartsGO.GetArrayElementAtIndex(frontPartCount).objectReferenceValue = frontSide;
                            frontPartCount++;

                            if (m_FSrandomRotation.boolValue)
                            {
                                float r = Random.Range(0f, m_FSmaxRotation.floatValue);
                                r = Mathf.Clamp(r, 0, m_FSmaxRotation.floatValue);
                                frontSide.transform.Rotate(Vector3.up, r);
                            }
                        }
                    }
                    //Add objs
                    if (m_addObjects.boolValue)
                    {
                        int objsRandom = Random.Range(0, 2);
                        if (m_objsRandomPlacement.boolValue && objsRandom == 0 || !m_objsRandomPlacement.boolValue)
                        {
                            GameObject[] allObjectsPrefab = Resources.LoadAll<GameObject>("Prefabs/");
                            
                            GameObject g = Instantiate(allObjectsPrefab[Random.Range(0,allObjectsPrefab.Length)], null) as GameObject;
                            m_allObjects.InsertArrayElementAtIndex(objectsCount);
                            m_allObjects.GetArrayElementAtIndex(objectsCount).objectReferenceValue = g;
                            objectsCount++;
   
                            g.transform.SetParent(horizontalGO.transform.transform, false);
                            g.transform.localPosition = new Vector3(m_PartDepth.floatValue - m_PartDepth.floatValue / 2, boxPosition.y, boxPosition.z + (size.z - m_VerticalPartThickness.floatValue) / 2);
                            
                            if (m_objsRandomRotation.boolValue)
                            {
                                if (g.transform.childCount == 0) g.transform.Rotate(Vector3.up, Random.Range(0, m_objsMaxRotation.floatValue));
                                foreach (Transform t in g.transform)
                                {
                                    t.Rotate(Vector3.up, Random.Range(0, m_objsMaxRotation.floatValue));
                                }
                            }
                        }
                    }
                }

            }
            //-----------
            //Create vertical part
            float moveOffset = 0;
            if (m_CountOfVerticalPart.intValue > 0)
            {
                for (int z = 0; z < m_CountOfVerticalPart.intValue+2; z++)
                {
                    float zOffset = (m_PartLenght.floatValue - m_VerticalPartThickness.floatValue) / (m_CountOfVerticalPart.intValue + 1);
                    float vertZOffset = moveOffset;
                    moveOffset += zOffset;

                    if (i < m_Count.intValue - 1)
                    {
                        GameObject verticalGO = FMeshGenerator.createGameObject(i+"|"+z.ToString(), horizontalGO.transform);
                        m_allVerticalPartsGO.InsertArrayElementAtIndex(i);
                        m_allVerticalPartsGO.GetArrayElementAtIndex(i).objectReferenceValue = verticalGO;
                        verticalPartMeshFilters[i] = verticalGO.GetComponent<MeshFilter>();

                        generateVerticalMeshes(i, vertZOffset);
                        verticalGO.GetComponent<MeshRenderer>().material = (Material)m_verticalMaterial.objectReferenceValue;
                    }
                }
            }
            //Sin function
            if (m_Op.intValue == 1)
            {
                float sinFunc = Mathf.Sin(((float)i * m_PartLenght.floatValue));

                for (int vertGO = 0; vertGO < horizontalPartsGO.transform.childCount; vertGO++)
                {
                   if(m_SinusHorizontal.boolValue) horizontalPartsGO.transform.GetChild(vertGO).localPosition += Vector3.forward * sinFunc * m_SinusMultiplier.floatValue;
                   if(m_SinusForward.boolValue) horizontalPartsGO.transform.GetChild(vertGO).localPosition += Vector3.right * sinFunc * m_SinusMultiplier.floatValue/2;
                }
            }
        }
        //Create back;
        if (m_BackSide.boolValue && m_Op.intValue == 0)
        {
            GameObject back = FMeshGenerator.createGameObject("Back", horizontalPartsGO.transform);
            float h = (m_GapY.floatValue * (m_Count.intValue-1) + m_Thickness.floatValue);
            float depth = 0.01f;
            back.GetComponent<MeshFilter>().mesh = FMeshGenerator.rebuildMesh(depth, m_PartLenght.floatValue, h, 0,0,m_RandomUV.boolValue);
            back.GetComponent<MeshRenderer>().material = (Material)m_backSideMaterial.objectReferenceValue;
            back.transform.localPosition -= Vector3.right * depth;
            back.name = "Back";
            m_back.objectReferenceValue = back;
        }
    }
    private void generateVerticalMeshes(int loopID, float vertZOffset)
    {
        float vertDepth = m_VerticalPartDepth.floatValue;
        float vertHeight = m_GapY.floatValue - m_Thickness.floatValue;
        float vertHeightOffset = m_Thickness.floatValue + (m_GapY.floatValue * loopID);

        verticalPartMeshFilters[loopID].mesh = FMeshGenerator.rebuildMesh(vertDepth, m_VerticalPartThickness.floatValue, vertHeight, vertHeightOffset, vertZOffset,m_RandomUV.boolValue);
    }
    private void checkForMissingMaterials()
    {
        if (m_backSideMaterial.objectReferenceValue == null) m_backSideMaterial.objectReferenceValue = FMeshGenerator.getMaterial("wood");
        if (m_frontSideMaterial.objectReferenceValue == null) m_frontSideMaterial.objectReferenceValue = FMeshGenerator.getMaterial("wood");
        if (m_horizontalMaterial.objectReferenceValue == null) m_horizontalMaterial.objectReferenceValue = FMeshGenerator.getMaterial("wood");
        if (m_verticalMaterial.objectReferenceValue == null) m_verticalMaterial.objectReferenceValue = FMeshGenerator.getMaterial("wood");
    }
    public override void OnInspectorGUI()
    {
        m_Object.Update();
        GUILayout.Space(10);
        GUILayout.Label("Horizontal Part Data:", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(m_PartLenght, new GUIContent("Length:"));
        EditorGUILayout.PropertyField(m_Thickness, new GUIContent("Thickness:"));
        EditorGUILayout.PropertyField(m_PartDepth, new GUIContent("Depth:"));

        GUILayout.Space(10);
        GUILayout.Label("Vertical Part Data:", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_Op, new GUIContent("Placment:"));
        if (m_Op.intValue == 1)
        {
            EditorGUILayout.PropertyField(m_SinusHorizontal, new GUIContent("   Horizontal:"));
            EditorGUILayout.PropertyField(m_SinusForward, new GUIContent("   Forward:"));
            EditorGUILayout.PropertyField(m_SinusMultiplier, new GUIContent("   Multiplier:"));
        }
        EditorGUILayout.PropertyField(m_VerticalPartThickness, new GUIContent("Thickness:"));
        EditorGUILayout.PropertyField(m_VerticalPartDepth, new GUIContent("Depth:"));
        EditorGUILayout.PropertyField(m_CountOfVerticalPart, new GUIContent("Count:"));

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(m_FrontSide, new GUIContent("Add front side:"));
        if (m_FrontSide.boolValue) {
            EditorGUILayout.PropertyField(m_FSthickness, new GUIContent("    Thickness:"));
            EditorGUILayout.PropertyField(m_FSrandomPlacement, new GUIContent("    Random placement:"));
            EditorGUILayout.PropertyField(m_FSrandomRotation, new GUIContent("    Random rotation:"));
            if (m_FSrandomRotation.boolValue)
                EditorGUILayout.PropertyField(m_FSmaxRotation, new GUIContent("        Max angle:"));
        }
        if(m_Op.intValue == 0) EditorGUILayout.PropertyField(m_BackSide, new GUIContent("Add back side:"));

        EditorGUILayout.PropertyField(m_addObjects, new GUIContent("Add objects:"));
        if (m_addObjects.boolValue)
        {
            EditorGUILayout.PropertyField(m_objsRandomPlacement, new GUIContent("   Random placment:"));
            EditorGUILayout.PropertyField(m_objsRandomRotation, new GUIContent("   Random rotation:"));
            if (m_objsRandomRotation.boolValue)
            {
                EditorGUILayout.PropertyField(m_objsMaxRotation, new GUIContent("   Max angle:"));
            }
        }

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(m_Count, new GUIContent("Steps:"));
        EditorGUILayout.PropertyField(m_GapY, new GUIContent("Height:"));
        EditorGUILayout.PropertyField(m_RandomUV, new GUIContent("Random UV:"));

        GUILayout.Space(10);
        materialFoldoutHeaderGroup = EditorGUILayout.BeginFoldoutHeaderGroup(materialFoldoutHeaderGroup, new GUIContent("Materials:"));

        checkForMissingMaterials();

        if (materialFoldoutHeaderGroup)
        {
            EditorGUILayout.PropertyField(m_horizontalMaterial);
            EditorGUILayout.PropertyField(m_verticalMaterial);
            EditorGUILayout.PropertyField(m_frontSideMaterial);
            EditorGUILayout.PropertyField(m_backSideMaterial);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(10);

        //Clamp values
        if (m_PartLenght.floatValue < 0.5f) m_PartLenght.floatValue = 0.5f;
        if (m_Thickness.floatValue < 0.01f) m_Thickness.floatValue = 0.01f;
        if (m_PartDepth.floatValue < 0.1f) m_PartDepth.floatValue = 0.1f;
        if (m_GapY.floatValue < m_Thickness.floatValue) m_GapY.floatValue = m_Thickness.floatValue;
        if (m_Count.intValue < 1) m_Count.intValue = 1;
        if (m_VerticalPartThickness.floatValue < 0.01f) m_VerticalPartThickness.floatValue = 0.01f;
        m_FSmaxRotation.floatValue = Mathf.Clamp(m_FSmaxRotation.floatValue, 0, 90f);
        m_FSthickness.floatValue = Mathf.Clamp(m_FSthickness.floatValue, 0.01f, Mathf.Infinity);
        m_VerticalPartDepth.floatValue = Mathf.Clamp(m_VerticalPartDepth.floatValue, 0.01f, Mathf.Infinity);
        m_CountOfVerticalPart.intValue = Mathf.Clamp(m_CountOfVerticalPart.intValue, 0, 1000000);

        float maxLenght = m_PartLenght.floatValue - m_VerticalPartThickness.floatValue;

        m_VerticalPartZOffset.floatValue = Mathf.Clamp(m_VerticalPartZOffset.floatValue, 0f, maxLenght);

        if (m_VerticalPartThickness.floatValue > m_PartLenght.floatValue) m_PartLenght.floatValue = m_VerticalPartThickness.floatValue;

        //Action Buttons
        if (GUILayout.Button("Update"))
        {
            RecreateTheObj();
        }
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(m_allFrontPartsGO, new GUIContent("Front Parts:"));
        EditorGUILayout.PropertyField(m_allHorizontalPartsGO, new GUIContent("Horizontal Parts:"));
        EditorGUILayout.PropertyField(m_allVerticalPartsGO, new GUIContent("Vertical Parts:"));
        EditorGUILayout.PropertyField(m_allObjects, new GUIContent("Objects:"));
        GUILayout.Space(10);
        if (GUILayout.Button("Merge") && !m_merged.boolValue)
        {
            //FMeshGenerator.SaveToOBJ(t.GetComponentsInChildren<MeshFilter>());
            GameObject newObj = new GameObject("Furniture [Merged]");
            GameObject allSmallParts = new GameObject("Objects");
            allSmallParts.transform.SetParent(newObj.transform);

            if(m_allFrontPartsGO.arraySize>0) FMeshGenerator.mergeObjects(m_allFrontPartsGO, "Front Side", newObj.transform);
            FMeshGenerator.mergeObjects(m_allHorizontalPartsGO, "Horizontal Parts", newObj.transform);
            if(m_allVerticalPartsGO.arraySize>0)FMeshGenerator.mergeObjects(m_allVerticalPartsGO, "Vertical Parts", newObj.transform);

            m_allObjects = FMeshGenerator.clearNulls(m_allObjects);
            for (int i = 0; i < m_allObjects.arraySize; i++)
            {
               GameObject g = (GameObject)m_allObjects.GetArrayElementAtIndex(i).objectReferenceValue;
                g.transform.SetParent(newObj.transform.GetChild(0));
            }
            if (m_BackSide.boolValue && m_back.objectReferenceValue != null)
            {
                GameObject back = (GameObject)m_back.objectReferenceValue;
                back.transform.SetParent(newObj.transform);
            }

            

            m_merged.boolValue = true;
        }

        m_Object.ApplyModifiedProperties();
        DrawDefaultInspector();
    }
}
