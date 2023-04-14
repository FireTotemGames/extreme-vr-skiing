using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */
    [SerializeField] int width = 100;
    [SerializeField] int height = 100;
    [SerializeField] int treeAmount = 1000;
    [SerializeField] float scale = 20.0f;
    [SerializeField] float heightMultiplier = 5.0f;
    [SerializeField] int tilesX = 2;
    [SerializeField] int tilesZ = 2;
    [SerializeField] Material material;
    
    [SerializeField] GameObject treePrefab;

    private List<MeshFilter> meshFilters;
    private List<Mesh> meshes;
    
    private float timer;
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    void Start()
    {
        timer = 2f;
        GenerateTerrain();
    }

    private void Update()
    {
        
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            AddTile();
            //RemoveTile();
            timer = 2f;
        }
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */
    void CreateShape()
    {
        Vector3[] vertices = new Vector3[width * (height + 1)];
        int[] triangles = new int[(width - 1) * height * 6];

        int triangleIndex = 0;

        for (int z = 0; z <= height; z++)
        {
            for (int x = -width/2; x < width/2; x++)
            {
                float heightValue = Mathf.PerlinNoise((float)(width + x) / (width) * scale, (float)(tilesZ * height + z) / (height) * scale) * heightMultiplier;

                vertices[z * width + (x + width/2)] = new Vector3(x, heightValue, z);

                if ((x + width/2) < width - 1 && z < height)
                {
                    int vertexIndex = z * width + (x + width/2);
                    triangles[triangleIndex + 0] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width;
                    triangles[triangleIndex + 2] = vertexIndex + width + 1;

                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + width + 1;
                    triangles[triangleIndex + 5] = vertexIndex + 1;

                    triangleIndex += 6;
                }
            }
        }
        
        meshes[tilesZ].vertices = vertices;
        meshes[tilesZ].triangles = triangles;
    }

    void UpdateMesh(int tz)
    {
        meshes[tz * tilesX].RecalculateNormals();
        meshFilters[tz * tilesX].mesh = meshes[tz * tilesX];
    }
    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */
    public void GenerateTerrain()
    {
        meshFilters = new List<MeshFilter>();
        meshes = new List<Mesh>();
        AddTile();
    }

    public void AddTile()
    {
        GameObject tile = new GameObject("Tile_" + 0f + "_" + tilesZ);
        tile.transform.position = new Vector3(0, 0, tilesZ * height);
        //tile.transform.rotation = Quaternion.Euler(0, 0, 180);
        tile.transform.parent = transform;

        MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();

        meshFilters.Add(meshFilter);
        meshes.Add(new Mesh());

        meshRenderer.material = material;

        CreateShape();
        UpdateMesh(tilesZ);

        MeshCollider meshCollider = tile.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshes[tilesZ];

        SpawnTrees();
        
        tilesZ++;
    }

    public void RemoveTile()
    {
        GameObject mesh = meshFilters[0].gameObject;
        meshFilters.RemoveAt(0);
        meshes.RemoveAt(0);
        Destroy(mesh);
        
    }

    private void SpawnTrees()
    {
        for (int i = 0; i < treeAmount; i++)
        {
            Vector3 randomPosition = new Vector3();
            randomPosition.x = Random.Range(-width/2, width/2);
            randomPosition.z = tilesZ * height + Random.Range(0, height);
            randomPosition.y = Mathf.PerlinNoise(randomPosition.x / scale, randomPosition.z / scale) * heightMultiplier;
            Instantiate(treePrefab, randomPosition, Quaternion.identity, meshFilters.Last().transform);
        }
        
    }
    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}