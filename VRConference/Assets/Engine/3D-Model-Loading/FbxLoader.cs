using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Network;
using Network.FileShare;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utility;

namespace Engine._3D_Model_Loading
{
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
        private static extern int GetVertciesOfID(int id, [In, Out] Vector3[] vecArray, int vecSize);

        [DllImport(dll)]
        private static extern int GetIndceiseOfID(int id, [In, Out] int[] indeciArray, int vecSize);
        [DllImport(dll)]
        private static extern IntPtr GetMaterialData(int id);

        [DllImport(dll)]
        private static extern IntPtr GetObjectMaterialName(int id);

        public static FbxLoader instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private List<FileEntry> files;

        public string FilePath = "";
        public Button UnloadModelButton;

        public GameObject ButtonListContent;
        public Button ButtonPrefab;

        public GameObject gameObjectPreFab;

        [Serializable]
        public struct MaterialData
        {
            public Material material;
            public List<string> nodeNames;
        }

        public List<MaterialData> materialDatas;

        public void SetPath(string path)
        {
            FilePath = path;
        }

        // Start is called before the first frame update
        void Start()
        {
            files = new List<FileEntry>();
            StartFBX();

            UnloadModelButton.onClick.AddListener(() =>
            {
                NetworkController.instance.networkSend.FBXUnloadFile();
                UnloadFile();
            });

            if (Valve.VR.InteractionSystem.Player.instance != null)
            {
                GetComponent<Canvas>().worldCamera = Valve.VR.InteractionSystem.Player.instance.rightHand.transform.GetChild(4).GetComponent<Camera>();
            }
            ScaleUp.active = false;
            ScaleDown.active = false;

        }

        public GameObject ModelanchorTable;
        public GameObject ModelanchorWorld;

        public float scale = 1.0f;
        public void ScaleModelUP()
        {
            rootNode.transform.parent = ModelanchorWorld.transform;
            rootNode.transform.localPosition = Vector3.zero;
            rootNode.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            SetMeshCollider(rootNode, true);
        }

        public void ScaleModelDown()
        {
            rootNode.transform.parent = ModelanchorTable.transform;
            rootNode.transform.localPosition = Vector3.zero;
            rootNode.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);



            SetMeshCollider(rootNode,false);


        }

        void SetMeshCollider(GameObject obj,bool aktiv)
        {

            obj.GetComponent<MeshCollider>().enabled = aktiv;

            for(int i = 0; i < obj.transform.childCount; i++)
            {
                SetMeshCollider(obj.transform.GetChild(i).gameObject, aktiv);
            }
        }






        public string fileName;
        public string currentFileName;

        public void SetFile(string name)
        {
            fileName = name;
            NetworkController.instance.networkSend.FBXLoadFile(fileName);
        }

        public void UnloadFile()
        {
            if (rootNode != null)
            {
                Destroy(rootNode);
                modelIsLoaded = false;
            }
        }

        public void Update()
        {

            foreach (FileEntry fileEntry in FileShare.instance.fileEntries)
            {

                if (fileEntry.local && fileEntry.localPath.Contains(".fbx"))
                {
                    string name = fileEntry.fileName;
                    if (!files.Contains(fileEntry))
                    {
                        Debug.Log(fileEntry.localPath);
                        var temp = Instantiate(ButtonPrefab, ButtonListContent.transform);
                        temp.name = name;
                        temp.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            SetFile(temp.name);
                        });
                        temp.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
                        files.Add(fileEntry);
                    }
                }
            }

            if (fileName == "") { return; }

            FileEntry entry = null;

            foreach (FileEntry fileEntry in FileShare.instance.fileEntries)
            {
                if (fileEntry.fileName == fileName)
                {
                    entry = fileEntry;
                    break;
                }
            }

            if (entry == null && !FileShare.instance.syncingFile)
            {
                Debug.Log("FBX Loader: Fbx unknown");
                FileShare.instance.SyncFiles();
            }
            else if (entry != null && !entry.local && !FileShare.instance.syncingFile)
            {
                Debug.Log("FBX Loader: Downloading FBX");
                FileShare.instance.SyncFile(entry);
            }
            else if (entry != null && entry.local && currentFileName != fileName)
            {
                currentFileName = fileName;

                Debug.Log("FBX Loader: Loading File");
                LoadFbxFile(entry.localPath);
            }

            if (importReady)
            {
                ProcessNode(GetRootNodeId());
                importReady = false;
            }
            if (modelIsLoaded)
            {
                ScaleUp.active = true;
                ScaleDown.active = true;
            }
            else
            {
                ScaleUp.active = true;
                ScaleDown.active = true;
            }
        }

        GameObject rootNode = null;
        bool importReady = false;

        [SerializeField]
        GameObject ScaleUp;
        [SerializeField]
        GameObject ScaleDown;
        bool modelIsLoaded;
        public void LoadFbxFile(string path)
        {
            if (path.Contains(".fbx"))
            {

                try
                {
                    Threader.RunAsync(() =>
                    {
                        ImportFbxFile(path);
                        meshData.Clear();
                        ProcessNodeData(GetRootNodeId());
                        importReady = true;
                        modelIsLoaded = true;
                    });
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
        public Material material;


        struct MeshData
        {
            public Vector3[] vertcies;
            public int[] indices;

            public MeshData(Vector3[] vertcies, int[] indices)
            {
                this.vertcies = vertcies;
                this.indices = indices;
            }
        }

        private Dictionary<int, MeshData> meshData = new Dictionary<int,MeshData>();

        void ReadDataOfNodes(int id)
        {
            int vertciesSize = GetVertciesArraySize(id);
            int indicesSize = GetIndecieArraySize(id);

            Vector3[] vertcies = new Vector3[vertciesSize];
            int[] indices = new int[indicesSize];

            GetVertciesOfID(id, vertcies, vertciesSize);
            GetIndceiseOfID(id, indices, indicesSize);

            meshData.Add(id, new MeshData(vertcies,indices));
        }

        void ProcessNodeData(int id)
        {
            if (HasNodeMesh(id))
            {
                ReadDataOfNodes(id);
            }
            
            var childs = GetChieldArray(id);
            for (int i = 0; i < childs.Length; i++)
            {
                ProcessNodeData(childs[i]);
            }
        }

        void ProcessMesh(int id, GameObject obj)
        {
            var unityMesh = new Mesh();
            unityMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            unityMesh.vertices = meshData[id].vertcies;
            unityMesh.triangles = meshData[id].indices;
            unityMesh.RecalculateNormals();
            var unityMeshFilter = obj.GetComponent<MeshFilter>();
            unityMeshFilter.mesh = unityMesh;
            var unityRenderer = obj.GetComponent<MeshRenderer>();
            var meshCollider = obj.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = unityMesh;
            meshCollider.enabled = false;
            unityRenderer.enabled = true;
            string materialName = Marshal.PtrToStringAnsi(GetObjectMaterialName(id));
            Material tempMaterial = material;

            bool stop = false;
            foreach(var materialData in materialDatas)
            {
                foreach (var nodeName in materialData.nodeNames)
                {
                    string matName = nodeName.Replace(" ", "");
                    materialName = materialName.Replace(" ", "");
                    if (matName == materialName)
                    {
                        tempMaterial = materialData.material;
                        stop = true;
                    }
                }

                if (stop)
                {
                    break;
                }
            }
            
            unityRenderer.material = tempMaterial;
            unityRenderer.staticShadowCaster = false;
            unityRenderer.shadowCastingMode = ShadowCastingMode.Off;
  
        }

        void ProcessTransform(int id, GameObject obj)
        {
            int size = 10;
            IntPtr returnedPtr = GetTransform(id);
            float[] tempData = new float[size];
            Marshal.Copy(returnedPtr, tempData, 0, size);

            obj.transform.localPosition = new Vector3(tempData[0], tempData[1], tempData[2]);
            obj.transform.localRotation = new Quaternion(tempData[3], tempData[4], tempData[5], tempData[6]);
            obj.transform.localScale = new Vector3(tempData[7], tempData[8], tempData[9]);
        }

        void ProcessNode(int id, GameObject ParentObj = null)
        {

            GameObject currentObj = Instantiate(gameObjectPreFab);
            currentObj.name = Marshal.PtrToStringAnsi(GetObjectName(id));

            if (ParentObj != null)
            {
                currentObj.transform.parent = ParentObj.transform;

            }
            else
            {
                rootNode = currentObj;
                rootNode.transform.parent = ModelanchorTable.transform;
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
}
