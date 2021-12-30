using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class FbxLoader : MonoBehaviour
{

#if UNITY_IOS
    const string dll = "__Internal":
#else
    const string dll = "FbxReader";
#endif


    [DllImport(dll)]
    private static extern void StartFBX();

    [DllImport(dll)]
    private static extern bool ImportFbxFile(string path);

    [DllImport(dll)]
    private static extern int GetRootNodeId();

    [DllImport(dll)]
    private static extern IntPtr GetChieldsOf(int id, out int size);

    [DllImport(dll)]
    private static extern IntPtr GetObjectName(int id);

    [DllImport(dll)]
    private static extern bool HasNodeMesh(int id);

    [DllImport(dll)]
    private static extern IntPtr GetTransform(int id);

    [DllImport(dll)]
    private static extern int GetVertciesArraySize(int id);

    [DllImport(dll)]
    private static extern int GetIndecieArraySize(int id);

    [DllImport(dll)]
    private static extern int GetVertciesOfID(int id, [In, Out] Vector3[] vecArray,int vecSize);

    [DllImport(dll)]
    private static extern int GetIndceiseOfID(int id,[In,Out] int[] indeciArray, int vecSize);
    [DllImport(dll)]
    private static extern IntPtr GetMaterialData(int id);


    public string FilePath = "";
    public Button LoadModelButton;
    public Button UnloadModelButton;

    public void SetPath(string path)
    {
        FilePath = path;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartFBX();

        LoadModelButton.onClick.AddListener(LoadFbxFile);
        UnloadModelButton.onClick.AddListener(()=>
        {
            if (rootNode != null)
            {
                Destroy(rootNode);
            }
        });

        if(Player.instance != null)
        {
            GetComponent<Canvas>().worldCamera = Player.instance.rightHand.transform.GetChild(4).GetComponent<Camera>(); ;
        }

    }

    GameObject rootNode = null;


    void LoadFbxFile()
    {
        if (ImportFbxFile(FilePath))
        {
            ProcessNode(GetRootNodeId());
        }
    }
    public Material material;
    void ProcessMesh(int id,GameObject obj)
    {
        var unityMesh = new Mesh();

        int vertciesSize = GetVertciesArraySize(id);
        int indicesSize = GetIndecieArraySize(id);

        Vector3[] vertcies = new Vector3[vertciesSize];
        int[] indices = new int[indicesSize];


        GetVertciesOfID(id, vertcies, vertciesSize);
        GetIndceiseOfID(id, indices, indicesSize);

        unityMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        unityMesh.vertices = vertcies;
        unityMesh.triangles = indices;
        unityMesh.RecalculateNormals();

        var unityMeshFilter = obj.AddComponent<MeshFilter>();
        unityMeshFilter.mesh = unityMesh;
        
        var unityRenderer = obj.AddComponent<MeshRenderer>();
        unityRenderer.material = material;
        unityRenderer.staticShadowCaster = false;
        unityRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //ProcessMaterial(id, unityRenderer.material);
        {
            // Assign the default material (hack!)
           // var unityPrimitive = GameObject.CreatePrimitive(PrimitiveType.Quad);
           // var unityMat = unityPrimitive.GetComponent<MeshRenderer>().sharedMaterial;
           // unityRenderer.sharedMaterial = unityMat;
            //UnityEngine.Object.DestroyImmediate(unityPrimitive);
        }
    }



    void ProcessMaterial(int id,Material mat)
    {
        Shader shader = Shader.Find("Specular");
        Material material = new Material(shader);
        int size = 3;
        IntPtr returnedPtr = GetMaterialData(id);
        float[] colorData = new float[size];
        Marshal.Copy(returnedPtr, colorData, 0, size);

        mat.color = new Color(colorData[0], colorData[1], colorData[2]);
    }

    void ProcessTransform(int id, GameObject obj)
    {
        int size = 10;
        IntPtr returnedPtr = GetTransform(id);
        float[] tempData = new float[size];
        Marshal.Copy(returnedPtr, tempData, 0, size);

        obj.transform.localPosition = new Vector3(tempData[0], tempData[1], tempData[2]);
        obj.transform.localRotation = new Quaternion(tempData[3], tempData[4], tempData[5], tempData[6]);
        obj.transform.localScale= new Vector3(tempData[7], tempData[8], tempData[9]);
    }

    void ProcessNode(int id, GameObject ParentObj = null)
    {

        GameObject currentObj = new GameObject(Marshal.PtrToStringAnsi(GetObjectName(id)));

        if (ParentObj != null)
        {
            currentObj.transform.parent = ParentObj.transform;
            
        }
        else
        {
            rootNode = currentObj;
            rootNode.transform.parent = gameObject.transform.parent.parent.transform.GetChild(2).transform;
        }

        ProcessTransform(id, currentObj);

        if (HasNodeMesh(id))
        {
            ProcessMesh(id, currentObj);
        }

        var childs = GetChieldArray(id);
        for (int i = 0; i < childs.Length; i++)
        {
            ProcessNode(childs[i], currentObj);
        }
    }
    int[] GetChieldArray(int parrent)
    {
        int size = 0;
        IntPtr returnedPtr = GetChieldsOf(parrent, out size);
        int[] Child = new int[size];
        Marshal.Copy(returnedPtr, Child, 0, size);
        return Child;
    }
}
