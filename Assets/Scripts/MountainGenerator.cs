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

    public int gaussianOffset = 3;
    public int gaussianWidth = 10;
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
        //GenerateGaussianGrid();
        tilesZ++;
    }

    public void RemoveTile()
    {
        GameObject mesh = meshFilters[0].gameObject;
        meshFilters.RemoveAt(0);
        meshes.RemoveAt(0);
        Destroy(mesh);
        
    }

    // private void SpawnTrees()
    // {
    //     for (int i = 0; i < treeAmount; i++)
    //     {
    //         Vector3 randomPosition = new Vector3();
    //         randomPosition.x = Random.Range(-width/2, width/2);
    //         randomPosition.z = tilesZ * height + Random.Range(0, height);
    //         randomPosition.y = Mathf.PerlinNoise(randomPosition.x / scale, randomPosition.z / scale) * heightMultiplier;
    //         Instantiate(treePrefab, randomPosition, Quaternion.identity, meshFilters.Last().transform);
    //     }
    //     
    // }
    
    private void SpawnTrees()
    {
        
        for (int i = 0; i < treeAmount; i++)
        {
            Vector3 randomPosition = new Vector3();
            randomPosition.x = Random.Range(-width/2, width/2);
            randomPosition.z = tilesZ * height + Random.Range(0, height);

            // Calculate distance from center normalized to [-1, 1]
            float normalizedDistanceFromCenter = Mathf.Sqrt(randomPosition.x * randomPosition.x) / (width / 2f) * 2f - 1f;

            // Calculate Gaussian distribution weight based on normalized distance from center
            float gaussianWeight = Mathf.Exp(-0.5f * normalizedDistanceFromCenter * normalizedDistanceFromCenter / (gaussianWidth * gaussianWidth));

            randomPosition.x = NextGaussian() * width;
            // Calculate Perlin noise value
            float perlinNoise = Mathf.PerlinNoise(randomPosition.x / scale, randomPosition.z / scale) * heightMultiplier;

            // Calculate final y position as a weighted average of Perlin noise and Gaussian weight
            //randomPosition.x = gaussianWeight;
            randomPosition.y = perlinNoise;

            

            Instantiate(treePrefab, randomPosition, Quaternion.identity, meshFilters.Last().transform);
        }
    }
    
    public static float NextGaussian() {
        float v1, v2, s;
        do {
            v1 = 2.0f * Random.Range(0f,1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f,1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);
        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
 
        return v1 * s;
    }
    
    // private void SpawnTrees()
    // {
    //     List<GameObject> trees = new List<GameObject>();
    //     float treeSpacing = 1f;
    //     float[,] distribution = new float[width, width];
    //     float sum = 0f;
    //
    //     // Calculate Gaussian distribution
    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int z = 0; z < width; z++)
    //         {
    //             float exponent = -((x - mean) * (x - mean) + (z - mean) * (z - mean)) / (2f * standardDeviation * standardDeviation);
    //             distribution[x, z] = Mathf.Exp(exponent);
    //             sum += distribution[x, z];
    //         }
    //     }
    //
    //     // Normalize distribution
    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int z = 0; z < width; z++)
    //         {
    //             distribution[x, z] /= sum;
    //         }
    //     }
    //
    //     // Spawn trees with Gaussian distribution
    //     for (int i = 0; i < treeAmount; i++)
    //     {
    //         // Choose a random position based on the Gaussian distribution
    //         Vector2 randomPosition = RandomGaussianPosition(distribution);
    //         Vector3 spawnPosition = new Vector3(randomPosition.x - width / 2f, 0f, tilesZ * height + randomPosition.y);
    //         spawnPosition.y = Mathf.PerlinNoise(spawnPosition.x, spawnPosition.z) * heightMultiplier;
    //
    //         // Check if position is already occupied by a tree
    //         bool positionIsOccupied = false;
    //         foreach (GameObject tree in trees)
    //         {
    //             if (Vector3.Distance(tree.transform.position, spawnPosition) < treeSpacing)
    //             {
    //                 positionIsOccupied = true;
    //                 break;
    //             }
    //         }
    //
    //         // If position is occupied, skip this iteration of the loop
    //         if (positionIsOccupied)
    //         {
    //             continue;
    //         }
    //
    //         // Spawn tree at position
    //         GameObject treeObject = Instantiate(treePrefab, spawnPosition, Quaternion.identity, meshFilters.Last().transform);
    //         trees.Add(treeObject);
    //     }
    // }
    //
    // private Vector2 RandomGaussianPosition(float[,] distribution)
    // {
    //     float randomX = Random.Range(0f, 1f);
    //     float randomY = Random.Range(0f, 1f);
    //     float sum = 0f;
    //
    //     // Find position in distribution that corresponds to the random values
    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int z = 0; z < width; z++)
    //         {
    //             sum += distribution[x, z];
    //             if (randomX <= sum && randomY <= distribution[x, z])
    //             {
    //                 return new Vector2(x, z);
    //             }
    //         }
    //     }
    //
    //     // If no position was found, return a random position
    //     return new Vector2(Random.Range(0, width), Random.Range(0, width));
    // }
    
    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}