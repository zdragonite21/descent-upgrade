using UnityEngine;
using System.Collections.Generic;

public class Scatter : MonoBehaviour
{
    public List<GameObject> prefabs;
    [Range(0, 1)]
    public float scatterChance = 0.001f;
    public float globalScale = 1f;
    public float sinkRand = 100f;

    public void ScatterObjects(MeshData meshData, Transform parentTransform)
    {
        foreach (Vector3 vertex in meshData.getVertices())
        {
            if (Random.value < scatterChance)
            {
                ScatterObjectAtVertex(vertex, parentTransform);
            }
        }
    }

    private void ScatterObjectAtVertex(Vector3 vertex, Transform parentTransform)
    {
        if (prefabs.Count == 0) return;

        //GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
        GameObject prefab = prefabs[0];

        GameObject instance = Instantiate(prefab, vertex, Quaternion.identity);

        instance.transform.parent = parentTransform;

        // Ensure the instance inherits the parent's transformations
        instance.transform.localPosition = vertex - Vector3.up * Random.value * sinkRand;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one * globalScale;
        //instance.transform.localScale = Vector3.one;
    }
}
