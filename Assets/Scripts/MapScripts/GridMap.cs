using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GridMap : MonoBehaviour {

    //Array of Levelobjects
    GridTile[,] gridArray;

    public List<GridTile> tilePrefabOptions;

    [Header("Testing Area")]
    public int width;
    public int depth;

	// Use this for initialization
	void Start () {
        CreateGridArray(width, depth);        
	}

    void CreateGridArray(int width, int depth)
    {
        gridArray = new GridTile[width, depth];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                CreateTile(x, z, 0);
            }
        }
        CreateTile(5, 7, 3);
        CreateTile(6, 7, 3);
        CreateTile(7, 7, 3);
        CreateTile(5, 8, 3);
        //CombineMesh(gameObject);
    }

    void CreateTile(int x, int z, int prefabChoice)
    {
        if(gridArray.GetLength(0) > x && gridArray.GetLength(1) > z)
        {
            if(gridArray[x, z] != null)
            {
                Debug.Log("Destroyed");
                Destroy(gridArray[x, z].gameObject);
            }
            gridArray[x, z] = Instantiate(tilePrefabOptions[prefabChoice]);
            gridArray[x, z].transform.localPosition = new Vector3(x, 0, z);
            gridArray[x, z].transform.SetParent(transform);
        }
        
    }

    void CombineMesh(GameObject parentOfObjectsToCombine)
    {
        if (parentOfObjectsToCombine == null) return;

        Vector3 originalPosition = parentOfObjectsToCombine.transform.position;
        parentOfObjectsToCombine.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = parentOfObjectsToCombine.GetComponentsInChildren<MeshFilter>();
        Dictionary<Material, List<MeshFilter>> materialToMeshFilterList = new Dictionary<Material, List<MeshFilter>>();
        List<GameObject> combinedObjects = new List<GameObject>();

        for (int i = 0; i < meshFilters.Length; i++)
        {
            var materials = meshFilters[i].GetComponent<MeshRenderer>().sharedMaterials;
            if (materials == null) continue;
            if (materials.Length > 1)
            {
                parentOfObjectsToCombine.transform.position = originalPosition;
                Debug.LogError("Objects with multiple materials on the same mesh are not supported. Create multiple meshes from this object's sub-meshes in an external 3D tool and assign separate materials to each.");
                return;
            }
            var material = materials[0];
            if (materialToMeshFilterList.ContainsKey(material)) materialToMeshFilterList[material].Add(meshFilters[i]);
            else materialToMeshFilterList.Add(material, new List<MeshFilter>() { meshFilters[i] });
        }

        foreach (var entry in materialToMeshFilterList)
        {
            List<MeshFilter> meshesWithSameMaterial = entry.Value;
            string materialName = entry.Key.ToString().Split(' ')[0];

            CombineInstance[] combine = new CombineInstance[meshesWithSameMaterial.Count];
            for (int i = 0; i < meshesWithSameMaterial.Count; i++)
            {
                combine[i].mesh = meshesWithSameMaterial[i].sharedMesh;
                combine[i].transform = meshesWithSameMaterial[i].transform.localToWorldMatrix;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            materialName += "_" + combinedMesh.GetInstanceID();

            string goName = (materialToMeshFilterList.Count > 1) ? "CombinedMeshes_" + materialName : "CombinedMeshes_" + parentOfObjectsToCombine.name;
            GameObject combinedObject = new GameObject(goName);
            var filter = combinedObject.AddComponent<MeshFilter>();
            filter.sharedMesh = combinedMesh;
            var renderer = combinedObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = entry.Key;
            combinedObjects.Add(combinedObject);
        }

        GameObject resultGO = null;
        if (combinedObjects.Count > 1)
        {
            resultGO = new GameObject("CombinedMeshes_" + parentOfObjectsToCombine.name);
            foreach (var combinedObject in combinedObjects) {
                combinedObject.transform.parent = resultGO.transform;
                combinedObject.AddComponent<MeshCollider>();
            }
        }
        else
        {
            resultGO = combinedObjects[0];
            resultGO.AddComponent<MeshCollider>();
        }        

        parentOfObjectsToCombine.SetActive(false);
        parentOfObjectsToCombine.transform.position = originalPosition;
        resultGO.transform.position = originalPosition;
        
    }

    //Function to place object into array of LevelObjects

    //Function to read potential spot for object and change according to tiles next to it
}
