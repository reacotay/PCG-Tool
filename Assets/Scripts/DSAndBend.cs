using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DSAndBend : MonoBehaviour
{
    public enum BendAxis { X, Y, Z };

    public int mDivisions;
    public float mSizeX;
    public float mSizeZ;
    public float mHeight;

    public BendAxis axis;
    public float rotate;
    public float fromPosition;
    Mesh mesh;
    Vector3[] vertices;
    int mVertCount;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        GenerateTerrain();
        Bend();

    }

    void GenerateTerrain()
    {
        mVertCount = (mDivisions + 1) * (mDivisions + 1);
        vertices = new Vector3[mVertCount];
        Vector2[] uvs = new Vector2[mVertCount];
        int[] tris = new int[mDivisions * mDivisions * 6];

        float halfSizeX = mSizeX / 2;
        float halfSizeZ = mSizeZ / 2;

        float divisionSize = ((mSizeX + mSizeZ) / 2) / mDivisions;


        int triOffset = 0;

        for (int i = 0; i <= mDivisions; i++)
        {
            for (int j = 0; j <= mDivisions; j++)
            {
                vertices[i * (mDivisions + 1) + j] = new Vector3(-halfSizeX + j * divisionSize, 0f, halfSizeZ - i * divisionSize);
                uvs[i * (mDivisions + 1) + j] = new Vector2((float)i / mDivisions, (float)j / mDivisions);

                if (i < mDivisions && j < mDivisions)
                {
                    int topLeft = i * (mDivisions + 1) + j;
                    int botLeft = (i + 1) * (mDivisions + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = botLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = botLeft + 1;
                    tris[triOffset + 5] = botLeft;

                    triOffset += 6;
                }
            }
        }
        vertices[0].y = Random.Range(-mHeight, mHeight);
        vertices[mDivisions].y = Random.Range(-mHeight, mHeight);
        vertices[vertices.Length - 1].y = Random.Range(-mHeight, mHeight);
        vertices[vertices.Length - 1 - mDivisions].y = Random.Range(-mHeight, mHeight);

        int iterations = (int)Mathf.Log(mDivisions, 2);
        int numSquares = 1;
        int squareSize = mDivisions;

        for (int i = 0; i < iterations; i++)
        {
            int row = 0;
            for (int j = 0; j < numSquares; j++)
            {
                int col = 0;
                for (int k = 0; k < numSquares; k++)
                {
                    DiamondSquare(row, col, squareSize, mHeight);
                    col += squareSize;
                }
                row += squareSize;
            }
            numSquares *= 2;
            squareSize /= 2;
            mHeight *= .5f;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void DiamondSquare(int row, int col, int size, float offset)
    {
        int halfsize = (int)(size * .5f);
        int topLeft = row * (mDivisions + 1) + col;
        int botLeft = (row + size) * (mDivisions + 1) + col;

        int mid = (int)(row + halfsize) * (mDivisions + 1) + (int)(col + halfsize);
        vertices[mid].y = (vertices[topLeft].y + vertices[topLeft + size].y + vertices[botLeft].y + vertices[botLeft + size].y) * .25f + Random.Range(-offset, offset);

        vertices[topLeft + halfsize].y = (vertices[topLeft].y + vertices[topLeft + size].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
        vertices[mid - halfsize].y = (vertices[topLeft].y + vertices[botLeft].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
        vertices[mid + halfsize].y = (vertices[topLeft + size].y + vertices[botLeft + size].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
        vertices[botLeft + halfsize].y = (vertices[botLeft].y + vertices[botLeft + size].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
    }

    void Bend()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        if (axis == BendAxis.X)
        {
            float meshWidth = mesh.bounds.size.z;
            for (int i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].z + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].x * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad) - formPos;

                vertices[i].x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;


            }
        }

        else if (axis == BendAxis.Y)
        {
            float meshWidth = mesh.bounds.size.z;

            for (int i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].z + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].y * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                vertices[i].y += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            }
        }
        else if (axis == BendAxis.Z)
        {
            float meshWidth = mesh.bounds.size.x;
            for (int i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].x + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].y * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                vertices[i].y += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].x = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    void SaveMesh(Mesh mesh, string name, bool makeNewInstance)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/Meshes/", name, "asset");

        if (string.IsNullOrEmpty(path))
            return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
}
