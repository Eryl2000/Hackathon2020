using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    public float getHeight(Vector3 position)
    {
        return Mathf.Sin(0.05f * (position.x * position.x + position.z * position.z));
    }
}
