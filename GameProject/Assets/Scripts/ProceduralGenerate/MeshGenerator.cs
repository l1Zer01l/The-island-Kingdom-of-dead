using UnityEngine;

namespace TheIslandKOD
{
    public static class MeshGenerator
    {

        public static MeshData GenerateTerrainMesh(float[,] heightMap)
        {
            int mapWidth = heightMap.GetLength(0);
            int mapHeight = heightMap.GetLength(1);

            float topLeftX = (mapWidth - 1) / -2f;
            float topLeftZ = (mapHeight - 1) / 2f;

            MeshData meshData = new MeshData(mapWidth, mapHeight);

            int vertexIndex = 0;
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y], topLeftZ - y);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)mapWidth, y / (float)mapHeight);

                    if (x < mapWidth - 1 && y < mapHeight - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + mapWidth + 1, vertexIndex + mapWidth);
                        meshData.AddTriangle(vertexIndex + mapWidth + 1, vertexIndex, vertexIndex + 1); 
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }

    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        private int triangleIndex;
        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth-1) * (meshHeight-1)*6];
        }

        public void AddTriangle(int x, int y, int z)
        {

            triangles[triangleIndex] = x;
            triangles[triangleIndex + 1] = y;
            triangles[triangleIndex + 2] = z;

            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}