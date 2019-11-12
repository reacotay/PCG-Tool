using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlatShadify : EditorWindow
{
    private string error = "";

    [MenuItem("Window/No Shared Vertices")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FlatShadify));
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a clone of the game object does not share vertices");
        GUILayout.Space(20);

        if(GUILayout.Button("Process"))
        {
            error = "";
            NoShared();
        }
    }

    private void NoShared()
    {
        Transform curr = Selection.activeTransform;

        if (curr == null)
        {
            error = "No appropriate object selected.";
            Debug.Log(error);
            return;
        }

        MeshFilter mf;
        mf = curr.GetComponent<MeshFilter>();

        if (mf == null || mf.sharedMesh == null)
        {
            error = "No mesh selected.";
            Debug.Log(error);
            return;
        }

        GameObject go = Instantiate(curr.gameObject) as GameObject;
        mf = go.GetComponent<MeshFilter>();
        Mesh mesh = Instantiate(mf.sharedMesh) as Mesh;
        mf.sharedMesh = mesh;
        Selection.activeObject = go.transform;


        Vector3[] oldVerts = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = new Vector3[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            vertices[i] = oldVerts[triangles[i]];
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

}
