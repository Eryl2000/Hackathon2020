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
        float distanceThrehold = 5.0f;

        float minHeight = 1;
        for (int i = 0; i < graph.Vertices.Count; ++i)
        {
            for (int j = 0; j < i; ++j)
            {
                if (graph.AdjacencyMatrix[i][j] == 0)
                {
                    continue;
                }

                float dist = DistanceToEdge(position, graph.Vertices[i], graph.Vertices[j]);
                if (dist < distanceThrehold)
                {
                    float height = (dist / distanceThrehold) * (dist / distanceThrehold);
                    if (height < minHeight)
                    {
                        minHeight = height;
                    }
                }
            }
        }
        return minHeight;
        //return DistanceToEdge(position, graph.Vertices[0], graph.Vertices[1]);
        //return Mathf.Sin(0.05f * (position.x * position.x + position.z * position.z));
    }


    public Graph GetGraph()
    {
        return graph;
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
