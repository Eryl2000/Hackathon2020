using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{


    public Graph(int numberOfPoints)
    {
        Vertices = new List<Vector3>();
        for (int i = 0; i < numberOfPoints; ++i)
        {
            Vertices.Add(Vector3.zero);
        }

        AdjacencyMatrix = new List<List<int>>();
        for (int i = 0; i < numberOfPoints; ++i)
        {
            List<int> subList = new List<int>();
            for (int j = 0; j < numberOfPoints; ++j)
            {
                subList.Add(0);
            }
            AdjacencyMatrix.Add(subList);
        }
    }

    public List<Vector3> Vertices;
    public List<List<int>> AdjacencyMatrix;


    public int GetClosestPoint(int pointIndex, List<int> pointsToExclude)
    {
        int ret = -1;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < Vertices.Count; ++i)
        {
            if (i == pointIndex || pointsToExclude.Contains(i))
            {
                continue;
            }

            float dist = Vector3.Distance(Vertices[i], Vertices[pointIndex]);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                ret = i;
            }
        }
        return ret;
    }


    public void AddEdge(int pointIndex1, int pointIndex2)
    {
        AdjacencyMatrix[pointIndex1][pointIndex2] = AdjacencyMatrix[pointIndex2][pointIndex1] = 1;
    }

    public void RemoveEdge(int pointIndex1, int pointIndex2)
    {
        AdjacencyMatrix[pointIndex1][pointIndex2] = AdjacencyMatrix[pointIndex2][pointIndex1] = 0;
    }


    public List<int> Neighbors(int pointIndex)
    {
        List<int> ret = new List<int>();
        for (int i = 0; i < Vertices.Count; ++i)
        {
            if (AdjacencyMatrix[pointIndex][i] != 0)
            {
                ret.Add(i);
            }
        }
        return ret;
    }


    public int NumberOfNeighbors(int pointIndex)
    {
        int count = 0;
        for (int i = 0; i < Vertices.Count; ++i)
        {
            if (AdjacencyMatrix[pointIndex][i] != 0)
            {
                count += 1;
            }
        }
        return count;
    }


    public int FarthestLeft()
    {
        int ret = -1;
        float farthestLeft = Mathf.Infinity;
        for (int i = 0; i < Vertices.Count; ++i)
        {
            if (Vertices[i].x < farthestLeft)
            {
                farthestLeft = Vertices[i].x;
                ret = i;
            }
        }
        return ret;
    }


    public int FarthestRight()
    {
        int ret = -1;
        float farthestRight = -Mathf.Infinity;
        for (int i = 0; i < Vertices.Count; ++i)
        {
            if (Vertices[i].x > farthestRight)
            {
                farthestRight = Vertices[i].x;
                ret = i;
            }
        }
        return ret;
    }


    public int FarthestForward()
    {
        int ret = -1;
        float farthestForward = -Mathf.Infinity;
        for (int i = 0; i < Vertices.Count; ++i)
        {
            if (Vertices[i].z > farthestForward)
            {
                farthestForward = Vertices[i].z;
                ret = i;
            }
        }
        return ret;
    }
}
