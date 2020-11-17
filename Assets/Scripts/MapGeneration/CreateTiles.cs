using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTiles : MonoBehaviour
{
    [SerializeField]
    private HeightMapGenerator heightMapGenerator;

    public GameObject TileBlueprint;


    // Start is called before the first frame update
    void Start()
    {
        Graph graph = heightMapGenerator.GetGraph();
        Vector3 farthestLeft = graph.Vertices[graph.FarthestLeft()];
        Vector3 farthestRight = graph.Vertices[graph.FarthestRight()];
        Vector3 root = graph.Vertices[0];
        Vector3 farthestForward = graph.Vertices[graph.FarthestForward()];

        float extraMargin = 1.5f;
        int width = (int)(extraMargin * (farthestRight.x - farthestLeft.x) / 10.0f);
        int height = (int)(extraMargin * (farthestForward.z - root.z) / 10.0f);

        float centerHorizontal = 0.5f * (farthestLeft.x + farthestRight.x);
        float centerVertical = 0.5f * (farthestForward.z + root.z);

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                float xCoord = centerHorizontal + (i - 0.5f * width) * 10.0f;
                float zCoord = centerVertical + (j - 0.5f * height) * 10.0f;
                GameObject tile = Instantiate(TileBlueprint, new Vector3(xCoord, 0, zCoord), Quaternion.identity);
                tile.transform.parent = transform;
                tile.GetComponent<TileGeneration>().heightMapGenerator = heightMapGenerator;
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

}
