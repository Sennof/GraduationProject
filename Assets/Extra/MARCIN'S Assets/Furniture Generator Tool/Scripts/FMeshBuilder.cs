using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace FMeshBuilder
{
    public class FMeshGenerator : MonoBehaviour
    {
        public static GameObject createGameObject(string name, Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "ID:" + name;
            gameObject.transform.SetParent(parent);
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>();
            return gameObject;
        }
        public static Material getMaterial(string name)
        {
            Material material = Resources.Load<Material>("Materials/" + name) as Material;
            return material;
        }

        public static SerializedProperty clearNulls(SerializedProperty obj)
        {
            for (int a = 0; a < obj.arraySize; a++)
            {
                if (obj.GetArrayElementAtIndex(a).objectReferenceValue == null)
                {
                    obj.DeleteArrayElementAtIndex(a);
                    a = 0;
                }
            }
            return obj;
        }
        public static void mergeObjects(SerializedProperty obj, string name, Transform parent)
        {
            obj = clearNulls(obj);

            GameObject[] gameObjects = new GameObject[obj.arraySize];
            for (int y = 0; y < obj.arraySize; y++)
            {
                gameObjects[y] = (GameObject)obj.GetArrayElementAtIndex(y).objectReferenceValue;
            }

            Material material = gameObjects[0].GetComponent<MeshRenderer>().sharedMaterial;
            MeshFilter[] meshFilters = new MeshFilter[gameObjects.Length];
            for (int z = 0; z < gameObjects.Length; z++)
            {
                meshFilters[z] = gameObjects[z].GetComponent<MeshFilter>();
            }
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            GameObject g = createGameObject("test", parent);
            g.GetComponent<MeshFilter>().mesh = new Mesh();
            g.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
            g.GetComponent<MeshRenderer>().sharedMaterial = material;
            g.name = name;

        }

        public static Mesh rebuildMesh(float m_PartDepth, float m_PartLenght, float m_Thickness, float HeightOffset, float ZOffset = 0, bool randomUV = false)
        {
            float uvOffset = 0;
            Mesh mesh = new Mesh();
            /*Vector3[] vertices = new Vector3[] {
                                                //Left
                                                new Vector3(0, HeightOffset, 0.0f+ZOffset),
                                                new Vector3(m_PartDepth, HeightOffset,0.0f+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset, 0+ZOffset),
                                                new Vector3(0, m_Thickness+HeightOffset, 0+ZOffset),
                                                //Right
                                                new Vector3(0, HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(m_PartDepth, HeightOffset,m_PartLenght+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(0, m_Thickness+HeightOffset, m_PartLenght+ZOffset)
                                                

            };

            //Left,Right,Top
            int[][] faces = { 
                    new int[] {0,1,2,3},
                    new int[] {2,6,7,3},
                    new int[] {7,6,5,4},
                    new int[] {1,5,6,2}
                    
            };

            List<int> triangles = new List<int>();
            for (int i = 0; i < faces.Length; i++)
            {
                triangles.Add(faces[i][0]);
                triangles.Add(faces[i][3]);
                triangles.Add(faces[i][2]);
                triangles.Add(faces[i][0]);
                triangles.Add(faces[i][2]);
                triangles.Add(faces[i][1]);
            }*/
            Vector3[] vertices = new Vector3[] {    
                                                //Top
                                                new Vector3(0, m_Thickness+HeightOffset,0.0f+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset,0.0f+ZOffset),
                                                new Vector3(0, m_Thickness+HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset, m_PartLenght+ZOffset),

                                                //Left
                                                new Vector3(0, HeightOffset, 0.0f+ZOffset),
                                                new Vector3(m_PartDepth, HeightOffset,0.0f+ZOffset),
                                                new Vector3(0, m_Thickness+HeightOffset, 0+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset, 0+ZOffset),

                                                //Right
                                                new Vector3(0, HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(m_PartDepth, HeightOffset,m_PartLenght+ZOffset),
                                                new Vector3(0, m_Thickness+HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset, m_PartLenght+ZOffset),

                                                //Bottom
                                                new Vector3(0.0f, HeightOffset,0.0f+ZOffset),
                                                new Vector3(m_PartDepth, HeightOffset, 0.0f+ZOffset),
                                                new Vector3(0.0f, HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(m_PartDepth, HeightOffset, m_PartLenght+ZOffset),

                                                //Front
                                                new Vector3(m_PartDepth, HeightOffset,0.0f+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset, 0.0f+ZOffset),
                                                new Vector3(m_PartDepth, m_Thickness+HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(m_PartDepth, HeightOffset, m_PartLenght+ZOffset),

                                                //Back
                                                new Vector3(0, HeightOffset,0.0f+ZOffset),
                                                new Vector3(0, m_Thickness+HeightOffset, 0.0f+ZOffset),
                                                new Vector3(0, m_Thickness+HeightOffset, m_PartLenght+ZOffset),
                                                new Vector3(0, HeightOffset, m_PartLenght+ZOffset)
            };


            //Set UVs coords
            Vector2[] newUVs = new Vector2[vertices.Length];
            //Top
            newUVs[0] = new Vector2(0, 0);
            newUVs[1] = new Vector2(1 * m_PartDepth, 0);
            newUVs[2] = new Vector2(0, 1 * m_PartLenght);
            newUVs[3] = new Vector2(1 * m_PartDepth, 1 * m_PartLenght);
            //Left
            newUVs[4] = new Vector2(0, 0);
            newUVs[5] = new Vector2(1 * m_PartDepth, 0);
            newUVs[6] = new Vector2(0, 1 * m_Thickness);
            newUVs[7] = new Vector2(1 * m_PartDepth, 1 * m_Thickness);
            //Right
            newUVs[8] = new Vector2(0, 0);
            newUVs[9] = new Vector2(1 * m_PartDepth, 0);
            newUVs[10] = new Vector2(0, 1 * m_Thickness);
            newUVs[11] = new Vector2(1 * m_PartDepth, 1 * m_Thickness);
            //Bottom
            newUVs[12] = new Vector2(0, 0);
            newUVs[13] = new Vector2(1 * m_PartDepth, 0);
            newUVs[14] = new Vector2(0, 1 * m_PartLenght);
            newUVs[15] = new Vector2(1 * m_PartDepth, 1 * m_PartLenght);
            //Front
            newUVs[16] = new Vector2(0, 0);
            newUVs[17] = new Vector2(1 * m_Thickness, 0);
            newUVs[18] = new Vector2(1 * m_Thickness, 1 * m_PartLenght);
            newUVs[19] = new Vector2(0, 1 * m_PartLenght);
            //Back
            newUVs[20] = new Vector2(0, 0);
            newUVs[21] = new Vector2(1 * m_Thickness, 0);
            newUVs[22] = new Vector2(1 * m_Thickness, 1 * m_PartLenght);
            newUVs[23] = new Vector2(0, 1 * m_PartLenght);

            if (randomUV)
            {
                uvOffset = Random.Range(0.0f, 2.0f);

                for (int i = 0; i < newUVs.Length; i++)
                {
                    newUVs[i] = newUVs[i] + Vector2.one * uvOffset;
                }
            }

            //mesh.SetVertices(vertices);

            //Top, Left, Right, Bottom, Front,
            int[] indices = new int[] { 2, 3, 1, 0, 5, 4, 6, 7, 8, 9, 11, 10, 12, 13, 15, 14, 16, 17, 18, 19, 23, 22, 21, 20 };
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Quads, 0);
            mesh.uv = newUVs;
            mesh.RecalculateNormals();

            return mesh;
        }
        public static void SaveToOBJ(MeshFilter[] meshFilters)
        {
            string name = "test 0";

            string path = string.Format("{0}{1}{2}{3}", Application.dataPath, "/", name, ".obj");
            do
            {
                int i = (int)name[name.Length - 1] - '0' + 1;
                name = name.Replace(name[name.Length - 1], i.ToString()[0]);
                Debug.Log(name);
                path = string.Format("{0}{1}{2}{3}", Application.dataPath, "/", name, ".obj");
                Debug.Log(path);
            }
            while (File.Exists(path));

            List<Vector3> allVerts = new List<Vector3>();
            List<Vector2> allUVs = new List<Vector2>();
            List<Vector3> allNormals = new List<Vector3>();
            List<int> allIndices = new List<int>();

            int length = 0;
            for (int i = 0; i < meshFilters.Length; i++)
            {
                for (int v = 0; v < meshFilters[i].sharedMesh.vertices.Length; v++)
                {
                    allVerts.Add(meshFilters[i].sharedMesh.vertices[v]);
                }

                for (int uv = 0; uv < meshFilters[i].sharedMesh.uv.Length; uv++)
                {
                    allUVs.Add(meshFilters[i].sharedMesh.uv[uv]);
                }

                for (int n = 0; n < meshFilters[i].sharedMesh.normals.Length; n++)
                {
                    allNormals.Add(meshFilters[i].sharedMesh.normals[n]);
                }

                for (int ind = 0; ind < meshFilters[i].sharedMesh.GetIndices(0).Length; ind++)
                {
                    allIndices.Add(meshFilters[i].sharedMesh.GetIndices(0)[ind]+length);
                }
                length += meshFilters[i].sharedMesh.GetIndices(0).Length;

            }

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    for (int i = 0; i < allVerts.ToArray().Length; i++)
                    {
                        string s = string.Format("v {0} {1} {2}", (float)allVerts.ToArray()[i].x, (float)allVerts.ToArray()[i].y, (float)allVerts.ToArray()[i].z);
                        s = s.Replace(',', '.');
                        sw.WriteLine(s);
                    }

                    for (int i = 0; i < allUVs.ToArray().Length; i++)
                    {
                        string s = string.Format("vt {0} {1}", allUVs.ToArray()[i].x, allUVs.ToArray()[i].y);
                        s = s.Replace(',', '.');
                        sw.WriteLine(s);

                    }

                    for (int i = 0; i < allNormals.ToArray().Length; i++)
                    {
                        string s = string.Format("vn {0} {1} {2}", allNormals.ToArray()[i].x, allNormals.ToArray()[i].y, allNormals.ToArray()[i].z);
                        s = s.Replace(',', '.');
                        sw.WriteLine(s);
                    }


                    for (int y = 0; y < allIndices.ToArray().Length; y++)
                    {
                        string p = string.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8} {9}/{10}/{11}",
                            allIndices.ToArray()[y] + 1, allIndices.ToArray()[y] + 1, allIndices.ToArray()[y] + 1,
                           allIndices.ToArray()[y + 1] + 1, allIndices.ToArray()[y + 1] + 1, allIndices.ToArray()[y + 1] + 1,
                            allIndices.ToArray()[y + 2] + 1, allIndices.ToArray()[y + 2] + 1, allIndices.ToArray()[y + 2] + 1,
                            allIndices.ToArray()[y + 3] + 1, allIndices.ToArray()[y + 3] + 1, allIndices.ToArray()[y + 3] + 1);
                        sw.WriteLine(p);
                        y += 3;
                    }
                }
            }
        }
    }
}
