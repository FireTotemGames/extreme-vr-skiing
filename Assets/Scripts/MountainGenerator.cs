using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MountainGenerator : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    public static MountainGenerator Instance;
    
    [Header("Slope Generation")]
    [SerializeField] int width = 100;
    [SerializeField] int height = 100;
    [SerializeField] float scale = 20.0f;
    [SerializeField] float heightMultiplier = 5.0f;
    [SerializeField] int tilesZ = 2;
    [SerializeField] private Material material;
    [SerializeField] private PhysicMaterial physicMaterial;
    [SerializeField] private GameObject tileTriggerPrefab;
    [SerializeField] private GameObject invisibleWallPrefab;
    [SerializeField] private GameObject rampPrefab;

    private List<MeshFilter> meshFilters;
    private List<Mesh> meshes;
    
    private float slopeAngle;

    [Header("Gaussian Tree Distribution")]
    [SerializeField] private int numberOfTrees = 100;
    [SerializeField] private float gaussWidth = 50f;
    [SerializeField] private GameObject tree;
    [SerializeField] private AnimationCurve probabilityCurve;
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        slopeAngle = transform.rotation.eulerAngles.x;
        GenerateTerrain();
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
                float heightValue = Mathf.PerlinNoise((float)x / (width) * scale, (float)(tilesZ * height + z) / (height) * scale) * heightMultiplier;

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
        
        meshes.Last().vertices = vertices;
        meshes.Last().triangles = triangles;
    }

    private void UpdateMesh()
    {
        meshes.Last().RecalculateNormals();
        meshFilters.Last().mesh = meshes.Last();
    }
    
    private void GenerateTerrain()
    {
        meshFilters = new List<MeshFilter>();
        meshes = new List<Mesh>();
        
        AddTile(false);
        AddTile();
    }
    
    private void SpawnTrees()
    {
        Transform treeContainer = new GameObject("TreeContainer").transform;
        treeContainer.transform.parent = meshFilters.Last().transform;
        
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 randomPosition = new Vector3();
            float x;
            float z = randomPosition.z = tilesZ * height + Random.Range(0, height);
    
            do
            {
                x = Random.Range(-1f, 1f);
            } while (Random.Range(0f, 1f) > probabilityCurve.Evaluate(Mathf.Abs(x)));
            
            randomPosition.x = x * gaussWidth;
            randomPosition.z = z;
            randomPosition.y = Mathf.PerlinNoise(randomPosition.x / width * scale, randomPosition.z / height * scale) * heightMultiplier;
    
            Quaternion treeRotation = Quaternion.Euler(-slopeAngle, 0f ,0f);
            randomPosition.z -= tilesZ * height;
            Instantiate(tree, randomPosition, treeRotation, treeContainer);
        }
    }
    
    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void AddTile(bool generateTileTrigger = true)
    {
        GameObject tile = new GameObject("Tile_" + 0f + "_" + tilesZ);
        //tile.transform.rotation = Quaternion.Euler(0, 0, 180);
        tile.transform.parent = transform;

        MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();

        meshFilters.Add(meshFilter);
        meshes.Add(new Mesh());

        meshRenderer.material = material;

        CreateShape();
        UpdateMesh();

        MeshCollider meshCollider = tile.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshes.Last();
        meshCollider.material = physicMaterial;

        SpawnTrees();
        //GenerateGaussianGrid();

        if (generateTileTrigger == true)
        {
            Instantiate(tileTriggerPrefab, Vector3.forward * height / 10f, Quaternion.identity, meshFilter.transform);
        }

        Instantiate(invisibleWallPrefab, new Vector3(width / 2f, 0f, height / 2f), quaternion.identity, meshFilter.transform);
        Instantiate(invisibleWallPrefab, new Vector3(-width / 2f, 0f, height / 2f), Quaternion.identity, meshFilter.transform);

        Vector3 rampPosition = new Vector3();
        rampPosition.x = Random.Range(-20f, 20f);
        rampPosition.z = Random.Range(height / 6f, height * 5f / 6f);
        rampPosition.y = Mathf.PerlinNoise(rampPosition.x / width * scale, (tilesZ * height + rampPosition.z) / height * scale) * heightMultiplier - 0.5f;
        Instantiate(rampPrefab, rampPosition, Quaternion.Euler(5f, 0f, 0f), meshFilter.transform);
        
        tile.transform.localPosition = new Vector3(0, 0, tilesZ * height);
        tile.transform.localRotation = Quaternion.identity;
        
        tilesZ++;
    }

    public void RemoveTile()
    {
        // meshFilters[0].gameObject.SetActive(false);
        GameObject mesh = meshFilters[0].gameObject;
        meshFilters.RemoveAt(0);
        meshes.RemoveAt(0);
        Destroy(mesh);
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}