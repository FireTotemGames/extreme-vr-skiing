using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMountain : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */
    public int terrainWidth = 256;
    public int terrainLength = 256;
    public int terrainHeight = 100;
    public float terrainScale = 20f;
    public float terrainDetailScale = 25f;
    public float playerMovementSpeed = 5f;
    public Transform playerTransform;
    public float slopeSteepness = 0.5f;

    private Terrain terrain;
    private Vector3 terrainPosition;
    private float[,] heightMap;
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    void Start()
    {
        // Get the terrain component and position
        terrain = GetComponent<Terrain>();
        terrainPosition = terrain.transform.position;

        // Create a new height map
        heightMap = new float[terrainWidth, terrainLength];

        // Generate the initial terrain
        GenerateTerrain(terrainPosition.x, terrainPosition.z);
    }

    void Update()
    {
        // Move the terrain forward based on player position
        //Debug.Log("Player position: " + playerTransform.position);
        //Debug.Log("Terrain position: " + terrainPosition);
        if (playerTransform.position.z > terrainPosition.z + terrainLength)
        {
            terrainPosition.z += terrainLength;
            GenerateTerrain(terrainPosition.x, terrainPosition.z);
        }
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */
    void GenerateTerrain(float xOff, float zOff)
    {
        // Loop through each point on the terrain
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainLength; z++)
            {
                Debug.Log("Terrain should change here");
                // Calculate the height of the point based on Perlin noise
                float height = Mathf.PerlinNoise((xOff + x) / terrainDetailScale, (zOff + z) / terrainDetailScale) * terrainHeight * slopeSteepness;

                // Add the height to the height map
                heightMap[x, z] = height;
            }
        }

        // Apply the height map to the terrain
        terrain.terrainData.SetHeights(0, 0, heightMap);
    }
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