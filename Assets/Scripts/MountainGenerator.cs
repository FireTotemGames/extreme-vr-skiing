using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    public int width = 100;
    public int height = 100;
    public float scale = 20.0f;
    public float heightMultiplier = 5.0f;
    public int tilesX = 2;
    public int tilesZ = 2;

    private MeshFilter[] meshFilters;
    private Mesh[] meshes;

    void Start()
    {
        meshFilters = new MeshFilter[tilesX * tilesZ];
        meshes = new Mesh[tilesX * tilesZ];

        for (int tz = 0; tz < tilesZ; tz++)
        {
            for (int tx = 0; tx < tilesX; tx++)
            {
                GameObject tile = new GameObject("Tile_" + tx + "_" + tz);
                tile.transform.position = new Vector3(tx * width, 0, tz * height);
                tile.transform.parent = transform;

                MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();

                meshFilters[tz * tilesX + tx] = meshFilter;
                meshes[tz * tilesX + tx] = new Mesh();

                meshRenderer.material = GetComponent<MeshRenderer>().material;

                CreateShape(tx, tz);
                UpdateMesh(tx, tz);
            }
        }
    }

    void CreateShape(int tx, int tz)
    {
        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        int triangleIndex = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightValue = Mathf.PerlinNoise((float)(tx * width + x) / (width * tilesX) * scale, (float)(tz * height + z) / (height * tilesZ) * scale) * heightMultiplier;

                vertices[z * width + x] = new Vector3(x, heightValue, z);

                if (x < width - 1 && z < height - 1)
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

        meshes[tz * tilesX + tx].vertices = vertices;
        meshes[tz * tilesX + tx].triangles = triangles;
    }

    void UpdateMesh(int tx, int tz)
    {
        meshes[tz * tilesX + tx].RecalculateNormals();
        meshFilters[tz * tilesX + tx].mesh = meshes[tz * tilesX + tx];
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}