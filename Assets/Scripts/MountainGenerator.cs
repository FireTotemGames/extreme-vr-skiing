using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */
    public int width = 100;
    public int height = 100;
    public float scale = 20.0f;
    public float heightMultiplier = 5.0f;
    public int tilesX = 2;
    public int tilesZ = 2;
    public Material material;

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
    void CreateShape(int tx, int tz)
    {
        Vector3[] vertices = new Vector3[width * (height + 1)];
        int[] triangles = new int[(width - 1) * height * 6];

        int triangleIndex = 0;

        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightValue = Mathf.PerlinNoise((float)(tx * width + x) / (width) * scale, (float)(tz * height + z) / (height) * scale) * heightMultiplier;

                vertices[z * width + x] = new Vector3(x, heightValue, z);

                if (x < width - 1 && z < height)
                {
                    int vertexIndex = z * width + x;
                    triangles[triangleIndex + 0] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width + 1;
                    triangles[triangleIndex + 2] = vertexIndex + width;

                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + 1;
                    triangles[triangleIndex + 5] = vertexIndex + width + 1;

                    triangleIndex += 6;
                }
            }
        }

        Debug.Log(tz);
        meshes[tz].vertices = vertices;
        meshes[tz].triangles = triangles;
    }

    void UpdateMesh(int tx, int tz)
    {
        meshes[tz * tilesX + tx].RecalculateNormals();
        meshFilters[tz * tilesX + tx].mesh = meshes[tz * tilesX + tx];
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
        tile.transform.position = new Vector3(width, 0, tilesZ * height);
        tile.transform.rotation = Quaternion.Euler(0, 0, 180);
        tile.transform.parent = transform;

        MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();

        meshFilters.Add(meshFilter);
        meshes.Add(new Mesh());

        meshRenderer.material = material;

        CreateShape(0, tilesZ);
        UpdateMesh(0, tilesZ);

        MeshCollider meshCollider = tile.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshes[tilesZ];
        
        tilesZ++;
        // GenerateTerrain();
        
    }

    public void RemoveTile()
    {
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