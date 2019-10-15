using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class meshgen : MonoBehaviour
{
//    private TMeshData meshdata, xdata;
//    private Mesh mesh;

    TJSONdata jsondata;

//    private string meshDataFileName = "plot.json";
    private string gmfFileName = "plotfull.json";

    private void Generate()
    {
/*
 * gameObject.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
//        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        mesh.name = "Testing Mesh";
        mesh.vertices = meshdata.vertices;
        mesh.triangles = meshdata.triangles;
        mesh.RecalculateNormals();
 */
    }

    private void OnDrawGizmos()
    {
//        if (meshdata.vertices == null) return;
        
        //Gizmos.color = Color.black;
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    Gizmos.DrawSphere(vertices[i], 0.1f);
        //}
    }

    private void Awake()
    {
        LoadMeshData();
        Generate();
    }

    private void LoadMeshData()
    {
/*
        string filePath = Path.Combine(Application.streamingAssetsPath, meshDataFileName);
        print(filePath);

        if (File.Exists(filePath))
        {
            string meshAsJson = File.ReadAllText(filePath);
            modelmesh = JsonUtility.FromJson<TModelMeshData>(meshAsJson);

            print("horizonts: " +  modelmesh.horizonts.Length + " " + modelmesh.horizonts[0].name);

        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
*/
        string gmfFilePath = Path.Combine(Application.streamingAssetsPath, gmfFileName);
        print(gmfFilePath);

        if (File.Exists(gmfFilePath))
        {
            string gmfAsJson = File.ReadAllText(gmfFilePath);
            jsondata = JsonUtility.FromJson<TJSONdata>(gmfAsJson);

//            string json;
//            json = JsonUtility.ToJson(jsondata);
//            print(json);

            print("bores (" + jsondata.gmfdata.data3d.bores.Length + "): " + jsondata.gmfdata.data3d.bores[0].name + ", ...");
            print("horizonts (" + jsondata.gmfdata.data3d.mesh.horizonts.Length + "): " + jsondata.gmfdata.data3d.mesh.horizonts[0].name + ", ...");

        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }

            float minx, maxx, miny, maxy, minz, maxz;
            float x, y, z;
 
            minx = miny = minz = 1000000000.0F;
            maxx = maxy = maxz = -minx;

            for (int h = 0; h < jsondata.gmfdata.data3d.mesh.horizonts.Length; h++)
            {
                THorizontData horz = jsondata.gmfdata.data3d.mesh.horizonts[h];
                Vector3[] vertices;

                for (int j = 0; j < 3; j++)
                {
                    vertices = horz.surfaceup.vertices;
                    if (j == 1) vertices = horz.surfacedn.vertices;
                    if (j == 2) vertices = horz.surfacesd.vertices;

                    for (int i = 0; i < vertices.Length; i++)
                    {
                        x = vertices[i].x *= 0.02F;
                        y = vertices[i].y *= 0.05F;
                        z = vertices[i].z *= 0.02F;
                        if (x > maxx) maxx = x;
                        if (x < minx) minx = x;
                        if (y != 0.0F && y > maxy) maxy = y;
                        if (y != 0.0F && y < miny) miny = y;
                        if (z > maxz) maxz = z;
                        if (z < minz) minz = z;
                    }
                }
            }

            print("box: " + minx + ", " + maxx + ", " + miny + ", " + maxy + ", " + minz + ", " + maxz);

            x = -(minx + maxx) * 0.5F;
            y = -miny;
            z = -(minz + maxz) * 0.5F;

            for (int h = 0; h < jsondata.gmfdata.data3d.mesh.horizonts.Length; h++)
            {
                THorizontData horz = jsondata.gmfdata.data3d.mesh.horizonts[h];
                Vector3[] vertices;

                for (int j = 0; j < 3; j++)
                {
                    vertices = horz.surfaceup.vertices;

                    if (j == 1) vertices = horz.surfacedn.vertices;
                    if (j == 2) vertices = horz.surfacesd.vertices;

                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i].x += x;
                        if (vertices[i].y != 0.0F) vertices[i].y += y;
                        vertices[i].z += z;
                    }
                }
            }

            minx = miny = minz = 1000000000.0F;
            maxx = maxy = maxz = -minx;

            for (int h = 0; h < jsondata.gmfdata.data3d.mesh.horizonts.Length; h++)
            {
                THorizontData horz = jsondata.gmfdata.data3d.mesh.horizonts[h];
                Vector3[] vertices;

                for (int j = 0; j < 3; j++)
                {
                    vertices = horz.surfaceup.vertices;

                    if (j == 1) vertices = horz.surfacedn.vertices;
                    if (j == 2) vertices = horz.surfacesd.vertices;
                
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        x = vertices[i].x;
                        y = vertices[i].y;
                        z = vertices[i].z;
                        if (x > maxx) maxx = x;
                        if (x < minx) minx = x;
                        if (y != 0.0F && y > maxy) maxy = y;
                        if (y != 0.0F && y < miny) miny = y;
                        if (z > maxz) maxz = z;
                        if (z < minz) minz = z;
                    }
                }
            }

            print("box: " + minx + ", " + maxx + ", " + miny + ", " + maxy + ", " + minz + ", " + maxz);
    }
    
    // Start is called before the first frame update
    void Start()
    {

        for (int h = 0; h < 1; h++)// jsondata.gmfdata.data3d.mesh.horizonts.Length; h++)
        {
            THorizontData horzmesh = jsondata.gmfdata.data3d.mesh.horizonts[h];
            string name;

            name = "mesh" + h;

            GameObject horz;
            horz = new GameObject(name);
            horz.AddComponent<MeshFilter>();

            Mesh mesh1;

            Mesh meshup, meshdn, meshsd;
  
            meshup = new Mesh();
            meshup.vertices = horzmesh.surfaceup.vertices;
            meshup.triangles = horzmesh.surfaceup.triangles;
            meshdn = new Mesh();
            meshdn.vertices = horzmesh.surfacedn.vertices;
            meshdn.triangles = horzmesh.surfacedn.triangles;
            meshsd = new Mesh();
            meshsd.vertices = horzmesh.surfacesd.vertices;
            meshsd.triangles = horzmesh.surfacesd.triangles;
            
            CombineInstance[] combine = new CombineInstance[3];

            combine[0].mesh = meshup;
            combine[0].transform = Matrix4x4.identity;
            combine[1].mesh = meshdn;
            combine[1].transform = Matrix4x4.identity;
            combine[2].mesh = meshsd;
            combine[2].transform = Matrix4x4.identity;

            horz.GetComponent<MeshFilter>().mesh = mesh1 = new Mesh();
            mesh1.name = "Mesh"+h;

            mesh1.CombineMeshes(combine);

//*
//                        mesh1.vertices = horzmesh.surfacesd.vertices;
//                        mesh1.triangles = horzmesh.surfacesd.triangles;

/*
                        mesh1.vertices = new Vector3[horzmesh.surfacesd.vertices.Length];
                        mesh1.triangles = new int[horzmesh.surfacesd.triangles.Length];

                        for (int i = 0; i < horzmesh.surfacesd.vertices.Length; i++)
                        {
                            mesh1.vertices[i] = horzmesh.surfacesd.vertices[i];
                        }

                        for (int i = 0; i < horzmesh.surfacesd.triangles.Length; i++)
                            mesh1.triangles[i] = horzmesh.surfacesd.triangles[i];

                        string ss;
                        ss = JsonUtility.ToJson(mesh1.vertices);
                        print("v (" + mesh1.vertices.Length + "): " + ss + ": " + horzmesh.surfacesd.vertices[0].x + "=" + (float)mesh1.vertices[0].x);
                        ss = JsonUtility.ToJson(mesh1.vertices);
                        print("v (" + mesh1.triangles.Length + "): " + ss + ": " + horzmesh.surfacesd.triangles[0] + "=" + (float)mesh1.triangles[0]);
            */
            //            horzmesh.surfacesd.vertices.CopyTo(mesh.vertices, 0);
            //            horzmesh.surfacesd.triangles.CopyTo(mesh.triangles, 0);
            mesh1.RecalculateNormals();

            MeshRenderer meshRenderer = horz.AddComponent<MeshRenderer>();
            meshRenderer.material = gameObject.GetComponent<MeshRenderer>().material;
            meshRenderer.material.color = new Color32((byte)(horzmesh.color % (256 * 256)), (byte)((horzmesh.color / 256) % 256), (byte)(horzmesh.color / (256 * 256)), 255);
            
            Rigidbody rigibody = horz.AddComponent<Rigidbody>();
            rigibody.isKinematic = true;
            
            MeshCollider meshCollider = horz.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
