using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    public Vector3 firstNode;
    public int numberOfPoints;

    private Graph graph;
    public void Awake()
    {
        graph = CreateGraph.GenerateGraph(firstNode, numberOfPoints);
    }


    public float getHeight(Vector3 position)
    {
        //float distanceThrehold = 15.0f;

        float height = 0;

        float noiseScale1 = 0.01f;
        float octave1 = 1.0f;
        height += noiseScale1 * (Mathf.PerlinNoise(octave1 * position.x, octave1 * position.z) - 0.5f);

        float noiseScale2 = 0.1f;
        float octave2 = 0.1f;
        height += noiseScale2 * (Mathf.PerlinNoise(octave2 * position.x, octave2 * position.z) - 0.5f);

        float noiseScale3 = 0.3f;
        float octave3 = 0.05f;
        height += noiseScale3 * (Mathf.PerlinNoise(octave3 * position.x, octave3 * position.z) - 0.5f);

        float noiseScale4 = 0.8f;
        float octave4 = 0.01f;
        height += noiseScale4 * (Mathf.PerlinNoise(octave4 * position.x, octave4 * position.z) - 0.5f);

        float distanceThrehold = Mathf.Lerp(15, 25, height / (noiseScale1 + noiseScale2 + noiseScale3 + noiseScale4) + 0.3f);

        float dist = ClosestDistanceToEdge(position);
        if (dist <= distanceThrehold)
        {
            height += (dist / distanceThrehold) * (dist / distanceThrehold);
        }
        else
        {
            height += 1.0f;
        }

        height = Mathf.Clamp(height, 0, 1);
        return height;
    }


    public Graph GetGraph()
    {
        return graph;
    }


    public float ClosestDistanceToEdge(Vector3 point)
    {
        float minDist = Mathf.Infinity;
        for (int i = 0; i < graph.Vertices.Count; ++i)
        {
            for (int j = 0; j < i; ++j)
            {
                if (graph.AdjacencyMatrix[i][j] == 0)
                {
                    continue;
                }

                float dist = DistanceToEdge(point, graph.Vertices[i], graph.Vertices[j]);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }
        }
        return minDist;
    }


    private float DistanceToEdge(Vector3 point, Vector3 vertex1, Vector3 vertex2)
    {
        float A = point.x - vertex1.x;
        float B = point.z - vertex1.z;
        float C = vertex2.x - vertex1.x;
        float D = vertex2.z - vertex1.z;

        float dot = A * C + B * D;
        float len_sq = C * C + D * D;
        float param = -1;
        if (len_sq != 0)
        {
            //in case of 0 length line
            param = dot / len_sq;
        }

        float xx, zz;

        if (param < 0)
        {
            xx = vertex1.x;
            zz = vertex1.z;
        }
        else if (param > 1)
        {
            xx = vertex2.x;
            zz = vertex2.z;
        }
        else
        {
            xx = vertex1.x + param * C;
            zz = vertex1.z + param * D;
        }

        float dx = point.x - xx;
        float dz = point.z - zz;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }
}
