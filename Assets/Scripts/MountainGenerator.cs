using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MountainGenerator : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    public static MountainGenerator Instance;

    [Header("Avalanche")]
    [SerializeField] private Avalanche avalanche;
    [SerializeField] private float avalancheStartSpeed;
    [SerializeField] private float avalancheAcceleration;

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
    private bool avalancheActive;
    private float avalancheSpeed;

    [Header("Gaussian Tree Distribution")]
    [SerializeField] private int numberOfTrees = 100;
    [SerializeField] private float treeWidth = 50f;
    [SerializeField] private Obstacle[] trees;
    [SerializeField] private AnimationCurve probabilityCurveTree;

    [Header("Stones")]
    [SerializeField] private GameObject[] stonePrefabs;
    [SerializeField] private int numberOfStones = 100;
    [SerializeField] private float stoneWidth = 50f;
    [SerializeField] private Obstacle[] stones;
    [SerializeField] private AnimationCurve probabilityCurveStone;

    public int Width => width;
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
        avalancheSpeed = avalancheStartSpeed;
    }

    private void Update()
    {
        if (avalancheActive == true)
        {
            Vector3 position = avalanche.transform.localPosition;
            position.z += avalancheSpeed * Time.deltaTime;
            avalanche.transform.localPosition = position;
            avalancheSpeed += avalancheAcceleration * Time.deltaTime * Time.deltaTime;
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
        AddTile(false);
        AddTile();
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
            } while (Random.Range(0f, 1f) > probabilityCurveTree.Evaluate(Mathf.Abs(x)));
            
            randomPosition.x = x * treeWidth / 2f;
            randomPosition.z = z;
            randomPosition.y = Mathf.PerlinNoise(randomPosition.x / width * scale, randomPosition.z / height * scale) * heightMultiplier;
    
            Quaternion treeRotation = Quaternion.Euler(-slopeAngle, 0f ,0f);
            randomPosition.z -= tilesZ * height;
            Obstacle treePrefab = trees[Random.Range(0, trees.Length)];
            GameObject tree = Instantiate(treePrefab, randomPosition, treeRotation, treeContainer).gameObject;
            tree.transform.localScale = Vector3.one * Random.Range(treePrefab.minScale, treePrefab.maxScale);
        }
    }
    
    private void SpawnStones()
    {
        Transform stoneContainer = new GameObject("StoneContainer").transform;
        stoneContainer.transform.parent = meshFilters.Last().transform;
        
        for (int i = 0; i < numberOfStones; i++)
        {
            Vector3 randomPosition = new Vector3();
            float x;
            float z = randomPosition.z = tilesZ * height + Random.Range(0, height);
    
            do
            {
                x = Random.Range(-1f, 1f);
            } while (Random.Range(0f, 1f) > probabilityCurveStone.Evaluate(Mathf.Abs(x)));
            
            randomPosition.x = x * stoneWidth / 2f;
            randomPosition.z = z;
            randomPosition.y = Mathf.PerlinNoise(randomPosition.x / width * scale, randomPosition.z / height * scale) * heightMultiplier;
    
            Quaternion stoneRotation = Quaternion.Euler(0f, Random.Range(0f, 360f) ,0f);
            randomPosition.z -= tilesZ * height;
            Obstacle stonePrefab = stones[Random.Range(0, stones.Length)];
            GameObject stone = Instantiate(stonePrefab, randomPosition, stoneRotation, stoneContainer).gameObject;
            stone.transform.localScale = Vector3.one * Random.Range(stonePrefab.minScale, stonePrefab.maxScale);
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
        tile.layer = LayerMask.NameToLayer("Ground");

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
        SpawnStones();

        if (generateTileTrigger == true)
        {
            Instantiate(tileTriggerPrefab, Vector3.forward * height / 10f, Quaternion.identity, meshFilter.transform);
        }

        Instantiate(invisibleWallPrefab, new Vector3(width / 2f * 0.9f, 0f, height / 2f), quaternion.identity, meshFilter.transform);
        Instantiate(invisibleWallPrefab, new Vector3(-width / 2f * 0.9f, 0f, height / 2f), Quaternion.identity, meshFilter.transform);

        Vector3 rampPosition = new Vector3();
        rampPosition.x = Random.Range(-20f, 20f);
        rampPosition.z = Random.Range(height / 6f, height * 5f / 6f);
        rampPosition.y = Mathf.PerlinNoise(rampPosition.x / width * scale, (tilesZ * height + rampPosition.z) / height * scale) * heightMultiplier - 0.5f;
        Instantiate(rampPrefab, rampPosition, Quaternion.Euler(0f, 0f, 0f), meshFilter.transform);
        
        tile.transform.localPosition = new Vector3(0, 0, tilesZ * height);
        tile.transform.localRotation = Quaternion.identity;
        
        tilesZ++;
    }

    public void RemoveTile()
    {
        GameObject mesh = meshFilters[0].gameObject;
        meshFilters.RemoveAt(0);
        meshes.RemoveAt(0);
        Destroy(mesh);
    }

    public void ActivateAvalanche()
    {
        avalancheActive = true;
        avalanche.ActivateDeathTrigger();
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}