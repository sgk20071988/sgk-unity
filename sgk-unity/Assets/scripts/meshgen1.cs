﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class meshgen1 : MonoBehaviour
{
    public float StartLoc = 50f;

    private TJSONdata jsondata;
    public string gmfFileName = "plotfull.json";

    private void Generate()
    {
    }

    private void Awake()
    {
        LoadMeshData();
        Generate();
    }

    private void LoadMeshData()
    {
        string gmfFilePath = Path.Combine(Application.streamingAssetsPath, gmfFileName);
        print(gmfFilePath);

        if (File.Exists(gmfFilePath))
        {
            string gmfAsJson = File.ReadAllText(gmfFilePath);
            jsondata = JsonUtility.FromJson<TJSONdata>(gmfAsJson);
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

        for (int h = 0; h < jsondata.gmfdata.data3d.mesh.horizonts.Length; h++)
        {
            THorizontData horzmesh = jsondata.gmfdata.data3d.mesh.horizonts[h];
            string name;

            name = "mesh" + h;

            GameObject horz;
            horz = new GameObject(name);
            horz.AddComponent<MeshFilter>();

            Mesh mesh;

            CombineInstance[] combine = new CombineInstance[3];

            combine[0].mesh = mesh = new Mesh();
            combine[0].transform = Matrix4x4.identity;
            mesh.vertices = horzmesh.surfaceup.vertices;
            mesh.triangles = horzmesh.surfaceup.triangles;

            combine[1].mesh = mesh = new Mesh();
            combine[1].transform = Matrix4x4.identity;
            mesh.vertices = horzmesh.surfacedn.vertices;
            mesh.triangles = horzmesh.surfacedn.triangles;

            combine[2].mesh = mesh = new Mesh();
            combine[2].transform = Matrix4x4.identity;
            mesh.vertices = horzmesh.surfacesd.vertices;
            mesh.triangles = horzmesh.surfacesd.triangles;

            horz.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Mesh"+h;
            mesh.CombineMeshes(combine);
            mesh.RecalculateNormals();

            horz.transform.parent = transform;

            MeshRenderer meshRenderer = horz.AddComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load("OnePlaneCrossSection", typeof(Material)) as Material;
            Material material = meshRenderer.material;
            material.color = new Color32((byte)(horzmesh.color % (256 * 256)), (byte)((horzmesh.color / 256) % 256), (byte)(horzmesh.color / (256 * 256)), 255);
            material.SetColor("_CrossColor", new Color32((byte)(horzmesh.color % (256 * 256)), (byte)((horzmesh.color / 256) % 256), (byte)(horzmesh.color / (256 * 256)), 255));
           
            var foundObjects = FindObjectsOfType<GameObject>();
            GameObject quad = foundObjects[0];

            foreach (object o in foundObjects)
            {
                GameObject g = (GameObject)o;
                if (g.name=="Quad"){
                    quad = g;
                } 
                
            }

            OnePlaneCuttingController ctrl = horz.AddComponent<OnePlaneCuttingController>();
            ctrl.plane = quad;

            Rigidbody rigibody = horz.AddComponent<Rigidbody>();
            rigibody.isKinematic = true;
            
            MeshCollider meshCollider = horz.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            horz.transform.position = new Vector3( transform.position.x + StartLoc, transform.position.y, transform.position.z + StartLoc );
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
