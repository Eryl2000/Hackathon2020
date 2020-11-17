using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileGeneration : MonoBehaviour
{
    [SerializeField]
    private float heightMultiplier;

    public HeightMapGenerator heightMapGenerator;

    [SerializeField]
    private TerrainType[] terrainTypes;

    private MeshRenderer tileRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;


    void Start()
    {
        tileRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        GenerateTile();
    }


    void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // calculate the offsets based on the tile position
        float[,] heightMap = new float[tileWidth, tileDepth];
        for (int i = 0; i < tileWidth; ++i)
        {
            for (int j = 0; j < tileDepth; ++j)
            {
                float zOffset = (0.5f - (float)i / tileWidth) * 11.0f;
                float xOffset = (0.5f - (float)j / tileDepth) * 11.0f;
                heightMap[i, j] = heightMapGenerator.getHeight(transform.position + new Vector3(xOffset, 0, zOffset));
            }
        }

        // generate a heightMap using noise
        Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRenderer.material.mainTexture = tileTexture;

        UpdateMeshVertices(heightMap);
    }


    private Texture2D BuildTexture(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];

                TerrainType terrainType = ChooseTerrainType(height);
                colorMap[colorIndex] = terrainType.color;
            }
        }

        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }


    private TerrainType ChooseTerrainType(float height)
    {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach (TerrainType terrainType in terrainTypes)
        {
            // return the first terrain type whose height is higher than the generated one
            if (height < terrainType.height)
            {
                return terrainType;
            }
        }
        return terrainTypes[terrainTypes.Length - 1];
    }


    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];

                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * this.heightMultiplier, vertex.z);

                vertexIndex++;
            }
        }

        // update the vertices in the mesh and update its properties
        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
    }

}