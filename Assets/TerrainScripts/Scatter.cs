using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

public class Scatter : MonoBehaviour
{
    public List<GameObject> trees;
    public List<GameObject> lRocks;
    public List<GameObject> sRocks;

    [Range(0, 1)]
    public float treeDist = 1f;
    [Range(0, 1)]
    public float lRockDist = 1f;
    [Range(0, 1)]
    public float sRockDist = 1f;

    [Range(0, 1)]
    public float scatterChance = 0.05f;
    public float globalScale = 1f;
    public Vector2 heightRangeT = new Vector2(-10f, 0f);
    public Vector2 heightRangeR = new Vector2(1f, 2f);
    public Vector2 scaleRangeT = new Vector2(0.5f, 1.5f);
    public Vector2 scaleRangeR = new Vector2(0.8f, 1.2f);
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
        if (trees.Count == 0 && lRocks.Count == 0 && sRocks.Count == 0) return;

        float rand = Random.value * (treeDist + lRockDist + sRockDist);

        if (rand < treeDist && trees.Count > 0)
        {
            ScatterTree(vertex, parentTransform);
        } else if (rand < treeDist + lRockDist && lRocks.Count > 0)
        {
            ScatterRock(vertex, parentTransform, lRocks[Random.Range(0, lRocks.Count)]);
        }
        else if (rand < treeDist + lRockDist + sRockDist && sRocks.Count > 0)
        {
            ScatterRock(vertex, parentTransform, sRocks[Random.Range(0, sRocks.Count)]);
        }
    }

    private void ScatterTree(Vector3 vertex, Transform parentTransform)
    {
        GameObject tree = trees[Random.Range(0, trees.Count)];
        GameObject instance = Instantiate(tree, vertex, Quaternion.identity);

        instance.transform.parent = parentTransform;

        // Ensure the instance inherits the parent's transformations
        instance.transform.localPosition = vertex + Vector3.up * Random.Range(heightRangeT.x, heightRangeT.y);
        instance.transform.localRotation = Quaternion.identity;

        float randomScale = Random.Range(scaleRangeT.x, scaleRangeT.y) * globalScale;
        instance.transform.localScale = Vector3.one * randomScale;
    }

    private void ScatterRock(Vector3 vertex, Transform parentTransform, GameObject rock)
    {
        GameObject instance = Instantiate(rock, vertex, Quaternion.identity);

        instance.transform.parent = parentTransform;

        // Ensure the instance inherits the parent's transformations
        instance.transform.localPosition = vertex + Vector3.up * Random.Range(heightRangeR.x, heightRangeR.y);
        instance.transform.localRotation = Quaternion.Euler(
            Random.Range(0f, 360f), // Random rotation around x-axis
            Random.Range(0f, 360f), // Random rotation around y-axis
            Random.Range(0f, 360f)  // Random rotation around z-axis
        );

        float randomScale = Random.Range(scaleRangeR.x, scaleRangeR.y) * globalScale;
        instance.transform.localScale = Vector3.one * 20 * randomScale;
    }
}
