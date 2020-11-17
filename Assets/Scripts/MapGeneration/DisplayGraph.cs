using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGraph : MonoBehaviour
{
    [SerializeField]
    private HeightMapGenerator heightMapGenerator;


    private Graph graph;

    // Start is called before the first frame update
    void Start()
    {
        graph = heightMapGenerator.GetGraph();
        for (int i = 0; i < graph.Vertices.Count; ++i)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = graph.Vertices[i];
            sphere.transform.parent = transform;
        }

        for (int i = 0; i < graph.Vertices.Count; ++i)
        {
            for (int j = 0; j < i; ++j)
            {
                if (graph.AdjacencyMatrix[i][j] != 0)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = (graph.Vertices[i] + graph.Vertices[j]) / 2.0f;
                    cube.transform.parent = transform;
                    cube.transform.localScale = new Vector3(1, 1, (graph.Vertices[i] - graph.Vertices[j]).magnitude);
                    cube.transform.rotation = Quaternion.LookRotation(graph.Vertices[i] - graph.Vertices[j]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
