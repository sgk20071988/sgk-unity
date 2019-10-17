using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TMeshData
{
    public Vector3[] vertices;
    public int[] triangles;
}

[System.Serializable]
public struct THorizontData
{
    public string name;
    public int color;
    public TMeshData surfaceup;
    public TMeshData surfacedn;
    public TMeshData surfacesd;
}

[System.Serializable]
public struct TModelMeshData
{
    public THorizontData[] horizonts;
}


[System.Serializable]
public struct TModelBores
{
    public string name;
    public int id;
    public double x_up, y_up, z_up;
    public double x_dn, y_dn, z_dn;
}


[System.Serializable]
public struct TModelData3D
{
    public TModelBores[] bores;
    public TModelMeshData mesh;
}

[System.Serializable]
public struct TGMFdata
{
    public TModelData3D data3d;
}

[System.Serializable]
public struct TJSONdata
{
    public TGMFdata gmfdata;
}

public class meshGenData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
