using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EndlessTerrain : MonoBehaviour
{
    public static float scale = .2f;

    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public const float maxViewDst = 850;
    public const float deleteViewDst = maxViewDst * 1.5f;
    public float deleteWait = 5f;
    public Transform viewer;
    public Material mapMaterial;

    static Scatter scatter;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    List<TerrainChunk> terrainChunksDelete = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        scatter = FindObjectOfType<Scatter>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        StartCoroutine(CheckChunksForDeletion());

        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        for (int i = 0; i < terrainChunksDelete.Count; i++)
        {
            terrainChunkDictionary.Remove(terrainChunksDelete[i].GetCoord());
            terrainChunksDelete[i].Delete();
        }
        terrainChunksDelete.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
                }

            }
        }
    }

    IEnumerator CheckChunksForDeletion()
    {
        while (true)
        {
            yield return new WaitForSeconds(deleteWait);

            foreach (var chunk in terrainChunkDictionary.Values)
            {
                if (chunk.InDeleteZone() && !terrainChunksDelete.Contains(chunk))
                {
                    terrainChunksDelete.Add(chunk);
                }
            }
        }
    }

    public class TerrainChunk
    {
        public Action onChunkDeleted;

        GameObject meshObject;
        MeshData meshData;
        Vector2 position;
        Vector2 coord;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;


        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            this.coord = coord;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            meshObject.layer = LayerMask.NameToLayer("Ground");

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.localScale = Vector3.one * scale;
            meshObject.transform.parent = parent;
            SetVisible(false);

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);

            //Texture2D texture = TextureGenerator.TextureFromHeightMap(mapData.heightMap);
            ////meshRenderer.material.mainTexture = texture;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            UpdateTerrainChunk();
            this.meshData = meshData;
            meshFilter.mesh = meshData.CreateMesh();
            meshCollider.sharedMesh = meshFilter.mesh;

            scatter.ScatterObjects(meshData, meshObject.transform, ref onChunkDeleted);
        }


        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

        public void Delete()
        {
            onChunkDeleted?.Invoke();
            onChunkDeleted = null;
            Destroy(meshObject);
        }

        public bool InDeleteZone()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool deletedZone = viewerDstFromNearestEdge > deleteViewDst;
            return deletedZone;
        }

        public Vector2 GetCoord()
        {
            return coord;
        }
    }
}
