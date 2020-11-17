using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreateGraph
{

    public static Graph GenerateGraph(Vector3 firstNode, int numberOfPoints)
    {
        float maxDistance = 10.0f;
        float minDistance = 2.0f;
        float angleRange = 45.0f;
        int maxTriesToCreateVertex = 20;

        Graph graph = new Graph(numberOfPoints);
        List<int> leaves = new List<int>();
        int pointsInGraph = 0;

        leaves.Add(0);
        graph.Vertices[0] = firstNode;
        pointsInGraph += 1;

        // Create graph
        int vertexToConnect, parent;
        while (pointsInGraph < numberOfPoints)
        {
            if (leaves.Count == 0)
            {
                parent = Random.Range(1, pointsInGraph);  // If there are no more leaves, then start at a random vertex
            }
            else
            {
                parent = leaves[0];
                leaves.RemoveAt(0);
            }

            bool childCreated = false;

            // Try to add first edge
            vertexToConnect = GetValidConnection(graph, maxDistance, pointsInGraph, parent);
            if (vertexToConnect != -1)
            {
                graph.AddEdge(parent, vertexToConnect);
            }
            else if (CreateVertex(graph, leaves, minDistance, maxDistance, angleRange, maxTriesToCreateVertex, pointsInGraph, parent))
            {
                childCreated = true;
                pointsInGraph += 1;
            }

            // End condition
            if (pointsInGraph >= numberOfPoints) break;

            // Try to add second edge or child
            vertexToConnect = GetValidConnection(graph, maxDistance, pointsInGraph, parent);
            if (vertexToConnect != -1)
            {
                graph.AddEdge(parent, vertexToConnect);
            }
            else if (CreateVertex(graph, leaves, minDistance, maxDistance, angleRange, maxTriesToCreateVertex, pointsInGraph, parent))
            {
                childCreated = true;
                pointsInGraph += 1;
            }

            // Prevent dead-end paths
            if (!childCreated && graph.NumberOfNeighbors(parent) < 2)
            {
                graph.AddEdge(parent, parent + 1);
            }
        }

        // Fix any unconnected leaves
        float maxDistanceMultiplier = 1;
        while (leaves.Count > 1)
        {
            for (int i = leaves.Count - 1; i >= 0; i--)
            {
                vertexToConnect = GetValidConnection(graph, maxDistanceMultiplier * maxDistance, pointsInGraph, leaves[i]);
                if (vertexToConnect != -1)
                {
                    graph.AddEdge(leaves[i], vertexToConnect);
                    leaves.RemoveAt(i);
                }
            }
            maxDistanceMultiplier += 1;
        }

        return graph;
    }


    private static bool CreateVertex(Graph graph, List<int> leaves, float minDistance, float maxDistance, float angleRange, int maxTriesToCreateVertex, int pointsInGraph, int parent)
    {

        Vector3 newVertex;
        for (int tries = 0; tries < maxTriesToCreateVertex; ++tries)
        {
            float randAngle = Random.Range(-angleRange, angleRange);
            float randDistance = Random.Range(minDistance, maxDistance);
            newVertex = graph.Vertices[parent] + Quaternion.Euler(0, randAngle, 0) * Vector3.forward * randDistance;
            graph.Vertices[pointsInGraph] = newVertex;
            if (!IntersectsAnotherEdge(graph, pointsInGraph, parent, pointsInGraph))
            {
                graph.AddEdge(parent, pointsInGraph);
                leaves.Add(pointsInGraph);
                return true;
            }
        }
        return false;
    }


    private static int GetFarthestVertexForward(Graph graph)
    {
        int ret = -1;
        float farthestForward = 0.0f;
        for (int vertex = 0; vertex < graph.Vertices.Count; ++vertex)
        {
            if (graph.Vertices[vertex].z > farthestForward)
            {
                farthestForward = graph.Vertices[vertex].z;
                ret = vertex;
            }
        }
        return ret;
    }


    private static int GetValidConnection(Graph graph, float maxDistance, int pointsInGraph, int parent)
    {
        int ret = -1;
        int minDegree = pointsInGraph;
        for (int i = 0; i < pointsInGraph; ++i)
        {
            if (graph.Vertices[i].z <= graph.Vertices[parent].z)
            {
                continue;
            }
            if (graph.AdjacencyMatrix[i][parent] != 0)
            {
                continue;
            }
            if (Vector3.Distance(graph.Vertices[parent], graph.Vertices[i]) > maxDistance)
            {
                continue;
            }
            if (IntersectsAnotherEdge(graph, pointsInGraph, i, parent))
            {
                continue;
            }

            int numberOfNeighbors = graph.NumberOfNeighbors(i);
            if (numberOfNeighbors < minDegree)
            {
                minDegree = numberOfNeighbors;
                ret = i;
            }
        }
        return ret;
    }


    private static bool IntersectsAnotherEdge(Graph graph, int pointsInGraph, int vertex1, int vertex2)
    {
        bool intersects = false;
        for (int otherVertex = 0; otherVertex < pointsInGraph; ++otherVertex)
        {
            if (otherVertex == vertex1 || otherVertex == vertex2)
            {
                continue;
            }
            List<int> neighbors = graph.Neighbors(otherVertex);
            foreach (int neighbor in neighbors)
            {
                if (neighbor == vertex1 || neighbor == vertex2)
                {
                    continue;
                }
                if (neighbor < otherVertex)
                {
                    continue;
                }
                if (EdgesIntersect(graph.Vertices[vertex1], graph.Vertices[otherVertex], graph.Vertices[vertex2], graph.Vertices[neighbor]))
                {
                    intersects = true;
                    break;
                }
            }
        }
        return intersects;
    }


    private static bool EdgesIntersect(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        if (o1 != o2 && o3 != o4)
        {
            return true;
        }
        if (o1 == 0 && IsOnSegment(p1, p2, q1)) return true;
        if (o2 == 0 && IsOnSegment(p1, q2, q1)) return true;
        if (o3 == 0 && IsOnSegment(p2, p1, q2)) return true;
        if (o4 == 0 && IsOnSegment(p2, q1, q2)) return true;

        return false;
    }


    private static int orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        float val = (q.z - p.z) * (r.x - q.x) - (q.x - p.x) * (r.z - q.z);
        if (val == 0)
        {
            return 0;
        }
        return (val > 0) ? 1 : 2;
    }


    private static bool IsOnSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) && q.z <= Mathf.Max(p.z, r.z) && q.z >= Mathf.Min(p.z, r.z))
            return true;
        return false;
    }


}
