using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrainMesh : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int width = 50;
    public int height = 50;
    public float scale = 3f;          // Height of bumps
    public float bumpFrequency = 0.1f; // How frequent the bumps are

    private void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];

        // Create vertices
        for (int i = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * bumpFrequency, z * bumpFrequency) * scale;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // Create triangles
        int[] triangles = new int[width * height * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
