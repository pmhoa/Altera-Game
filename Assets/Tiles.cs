using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform plane;
    public GameObject tile;
    public float zsize;
    public float xsize;
    void Start()
    {

    }
    public void SpawnTiles()
    {
        plane = transform;
        ClearTiles();
        xsize = Mathf.Floor(plane.localScale.x);
        zsize = Mathf.Floor(plane.localScale.z);
        for (float x = 0; x < xsize; x++)
        {
            for (float z = 0; z < zsize; z++)
            {
                GameObject t = Instantiate(tile);
                t.name = $"Tile X{x},Z{z}";
                t.transform.localPosition = new Vector3(x, 0, z) + plane.position;
                t.transform.SetParent(plane);
            }
        }
    }
    public void ClearTiles()
    {
        for (int i = plane.childCount - 1; i > -1; --i)
            DestroyImmediate(plane.GetChild(i).gameObject);
    }



}
