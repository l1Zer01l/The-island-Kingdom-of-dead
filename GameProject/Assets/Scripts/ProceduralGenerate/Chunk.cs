using System;
using UnityEngine;

namespace TheIslandKOD
{
    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;

    }
    public class Chunk 
    {

        private static int number;
        private Vector2 m_position;
        private GameObject m_meshObject;
        
        private Bounds bounds;

        private LODInfo[] m_detailLevels;
        private LODMesh[] m_lodMeshes;

        private MapData m_mapData;
        private GenerateMap m_mapGenerator;

        private MeshFilter m_meshFilter;
        private MeshRenderer m_meshRenderer;

        private bool m_mapDataReceived;
        private int m_previousLODIndex = -1;
        public Chunk(Vector2 coordinate, int size, LODInfo[] detailsLevels, Transform parent, 
                     MapGenerator mapGenerator, GenerateMap generateMap)
        {
            m_detailLevels = detailsLevels;
            m_mapGenerator = generateMap;

            m_position = coordinate * size;
            Vector3 positionV3 = new Vector3(m_position.x, 0, m_position.y);
            bounds = new Bounds(m_position, Vector2.one * size);

            m_meshObject = new GameObject("TerrainChunk" + number++);
            m_meshFilter = m_meshObject.AddComponent<MeshFilter>();
            m_meshRenderer = m_meshObject.AddComponent<MeshRenderer>();

            m_meshObject.transform.parent = parent;
            m_meshObject.transform.position = positionV3;

            SetVisible(false);

            m_lodMeshes = new LODMesh[m_detailLevels.Length];
            for (int i = 0; i < m_detailLevels.Length; i++)
            {
                m_lodMeshes[i] = new LODMesh(m_detailLevels[i].lod, mapGenerator);
            }

            mapGenerator.RequestMapData(m_position, OnMapDataReceived);
        }

        public Bounds GetBounds()
        {
            return bounds;
        }
        public void SetVisible(bool visible)
        {
            m_meshObject.SetActive(visible);
        }

        public void UpdateChunk(bool visible, float distance)
        {
            if (m_mapDataReceived)
            {
                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < m_detailLevels.Length - 1; i++)
                    {
                        if (distance > m_detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != m_previousLODIndex)
                    {
                        LODMesh lodMesh = m_lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            m_previousLODIndex = lodIndex;
                            m_meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(m_mapData);

                            UpdateChunk(visible, distance);
                        }
                    }

                    GenerateMap.terrainChunksVisibleLastUpdate.Add(this);

                }

                SetVisible(visible);
            }
        }

        public bool IsVisible()
        {
            return m_meshObject.activeSelf;
        }

        private void OnMapDataReceived(MapData mapData)
        {
            m_mapData = mapData;
            m_mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.MAX_CHUNK_SIZE,
                                                                     MapGenerator.MAX_CHUNK_SIZE);
            m_meshRenderer.material.mainTexture = texture;

            float viewerDstFromNearestEddge = Mathf.Sqrt(bounds.SqrDistance(m_mapGenerator.GetViewPosition()));
            bool isVisible = viewerDstFromNearestEddge <= m_mapGenerator.GetMaxViewDst();
            UpdateChunk(isVisible, viewerDstFromNearestEddge);
        }

    }
}
