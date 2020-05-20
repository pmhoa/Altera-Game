using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tiles))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        Tiles t = (Tiles)target;
        if (GUILayout.Button("Build Grid"))
        {
            t.SpawnTiles();
        }
        if (GUILayout.Button("Clear Grid"))
        {
            t.ClearTiles();
        }
        if (GUILayout.Button("Update All Tiles"))
        {
            foreach (Tiles ts in FindObjectsOfType<Tiles>())
            {
                ts.SpawnTiles();
            }
        }
    }
}
