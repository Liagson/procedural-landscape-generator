using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class CreatePlaneMesh : MonoBehaviour {

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;

    List<Vector3> vertex = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    List<Color32> colours = new List<Color32>();

    void Start() {
        GenerateMap();
        GenerateMesh();
    }
    
    void GenerateMap() {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++) {
            SmoothMap();
        }
        RandomFillMap2();
        for (int i = 0; i < 2; i++) {
            SmoothMapHeight();
        }

    }


    void RandomFillMap() {
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 0;
                } else {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void RandomFillMap2() {
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (map[x, y] != 0) {
                    if (x < 10 || x > width - 10 || y < 10 || y > height - 10)
                        map[x, y] = 0;
                    else map[x, y] = pseudoRandom.Next(1, 10);
                }
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    void SmoothMapHeight() {
        for (int x = 1; x < width - 1; x++) {
            for (int y = 1; y < height - 1; y++) {
                int neighbourWallTiles;
                if (map[x, y] != 0) {
                    neighbourWallTiles = map[x - 1, y - 1] + map[x - 1, y] + map[x - 1, y + 1];
                    neighbourWallTiles += map[x, y - 1] + map[x, y] + map[x, y + 1];
                    neighbourWallTiles += map[x + 1, y - 1] + map[x + 1, y] + map[x + 1, y + 1];
                    map[x, y] = neighbourWallTiles / 9;

                }
                
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    if (neighbourX != gridX || neighbourY != gridY) {
                        wallCount += map[neighbourX, neighbourY];
                    }
                } else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    public void processVertex(int x_sup_left, int y_sup_left) {
        vertex.Add(new Vector3(x_sup_left, y_sup_left, map[x_sup_left, y_sup_left]));
        vertex.Add(new Vector3(x_sup_left + 1, y_sup_left, map[x_sup_left + 1, y_sup_left]));
        vertex.Add(new Vector3(x_sup_left, y_sup_left + 1, map[x_sup_left, y_sup_left + 1]));
        vertex.Add(new Vector3(x_sup_left + 1, y_sup_left + 1, map[x_sup_left + 1, y_sup_left + 1]));
    }

    public void processTriangles(int pos_relat) {

        triangles.Add(pos_relat);
        triangles.Add(pos_relat + 3);
        triangles.Add(pos_relat + 2);

        triangles.Add(pos_relat);
        triangles.Add(pos_relat + 1);
        triangles.Add(pos_relat + 3);
    }

    public void processUv() {

        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
    }

    Color32 setColor(int x, int y) {
        if (map[x, y] == 0)
            return new Color32(0, 255, 255, 1);
        else if (map[x, y] < 3)
            return new Color32(0, 255, 0, 1);
        else return new Color32(255, 255, 255, 1);
    }

    void GenerateMesh() {
        MeshFilter mf = (MeshFilter)this.gameObject.AddComponent(typeof(MeshFilter));
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mesh.Clear();

        
        int pos_relat = 1;
        for (int y = 0; y < height - 1; y++) {
            for (int x = 0; x < width - 1; x++) {

                processVertex(x, y);
                processTriangles(pos_relat - 1);
                processUv();

                colours.Add(setColor(x, y));
                colours.Add(setColor(x + 1, y));
                colours.Add(setColor(x, y + 1));
                colours.Add(setColor(x + 1, y + 1));
                
                pos_relat += 4;
            }
        }

        mesh.vertices = vertex.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors32 = colours.ToArray();
        mesh.RecalculateNormals();
        mesh.Optimize();
        
    }
}