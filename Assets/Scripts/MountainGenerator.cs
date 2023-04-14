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
    [Header("Slope Generation")]
    [SerializeField] int width = 100;
    [SerializeField] int height = 100;
    [SerializeField] float scale = 20.0f;
    [SerializeField] float heightMultiplier = 5.0f;
    [SerializeField] int tilesX = 2;
    [SerializeField] int tilesZ = 2;
    [SerializeField] private Material material;
    [SerializeField] private PhysicMaterial physicMaterial;
    
    private List<MeshFilter> meshFilters;
    private List<Mesh> meshes;
    
    private float timer;
    private float slopeAngle;

    [Header("Gaussian Tree Distribution")]
    [SerializeField] private int numberOfTrees = 100;
    [SerializeField] private float gaussWidth = 50f;
    [SerializeField] private GameObject tree;
    [SerializeField] private AnimationCurve probabilityCurve;
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    void Start()
    {
        slopeAngle = transform.rotation.eulerAngles.x;
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
        meshCollider.material = physicMaterial;

        SpawnTrees();
        //GenerateGaussianGrid();
        
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

    private void SpawnTrees()
    {
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
            Instantiate(tree, randomPosition, treeRotation, meshFilters.Last().transform);
        }
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}