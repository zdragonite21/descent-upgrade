using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Scatter : MonoBehaviour
{
    public List<GameObject> prefabs;
    [Range(0, 1)]
    public float scatterChance = 0.05f;
    public float globalScale = 1f;
    public Vector2 heightRange = new Vector2(-10f, 0f);
    public Vector2 scaleRange = new Vector2(0.5f, 1.5f);
    public float probMapStrength = 2f;

    public void ScatterObjects(MeshData meshData, Transform parentTransform)
    {
        Vector3[] normals = meshData.getNormals();
        Vector3[] vertices = meshData.getVertices();
        float[] probability = meshData.getProbability();

        for (int i = 0; i < normals.Length; i++)
        {
            Vector3 vertex = vertices[i];

            // Normal Map Probability Scatter
            //float chance = scatterChance * Mathf.Pow(Mathf.Abs(Vector3.Dot(Vector3.up, normals[i])), 10);

            // Noise Map Probability Scatter
            float chance = scatterChance * Mathf.Pow(probability[i], probMapStrength);
            if (vertex.z != 0 && Random.value < chance)
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
        instance.transform.localPosition = vertex + Vector3.up * Random.Range(heightRange.x, heightRange.y);
        instance.transform.localRotation = Quaternion.identity;

        float randomScale = Random.Range(scaleRange.x, scaleRange.y) * globalScale;
        instance.transform.localScale = Vector3.one * randomScale;
    }
}
