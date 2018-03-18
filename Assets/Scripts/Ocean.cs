using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Ocean : MonoBehaviour
{
    public int xSize, zSize;
    public float waveAplitude = .5f;
    public int waveSpeed = 10;
    public float time = 0;

    private Vector3[] vertices;
    private Mesh mesh;
    private const float NormalTriangleSize = 0.2f;
    

    private void Update()
    {
        Generate();
    }

    private void Generate()
    {
        time += Time.deltaTime;
        if (time >= float.MaxValue - 1)
        {
            time = 0;
        }
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, GetHeightAtLocation(x,z) ,z);
                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, y = 0; y < zSize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public float GetHeightAtLocation(float x , float z)
    {
        float waveHeight = Mathf.Sin(x + (time * waveSpeed)) * waveAplitude;
        waveHeight += Mathf.Sin(z + (time * waveSpeed + 3)) * waveAplitude * .5f; 
        //waveHeight = Mathf.Clamp(waveHeight, -.1f, 1);
        return waveHeight;
    }
    public Vector3 GetNormalAtLocation(float x, float z)
    {
        Vector3 a = new Vector3(x, GetHeightAtLocation(x, z + NormalTriangleSize), z + NormalTriangleSize);
        Vector3 b = new Vector3(x + NormalTriangleSize, GetHeightAtLocation(x + NormalTriangleSize, z - NormalTriangleSize), z - NormalTriangleSize);
        Vector3 c = new Vector3(x - NormalTriangleSize, GetHeightAtLocation(x - NormalTriangleSize, z - NormalTriangleSize), z - NormalTriangleSize);
        Vector3 dir = Vector3.Cross(b - a, c - a);
        return dir / dir.magnitude;
    }
    //private void OnDrawGizmos()
    //{
    //    if(vertices == null)
    //    {
    //        return;    
    //    }
    //    Gizmos.color = Color.black;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], 0.1f);
    //    }
    //}
}
