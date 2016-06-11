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

    Color32 setColor(int x, int y) {
        if (map[x, y] == 0)
            return new Color32(0, 255, 255, 1);
        else if (map[x, y] < 3)
            return new Color32(0, 255, 0, 1);
        else return new Color32(255, 255, 255, 1);
    }

    void GenerateMesh() {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mesh.Clear();

        
        int pos_relat = 1;
        for (int y = 0; y < height - 1; y++) {
            for (int x = 0; x < width - 1; x++) {
                Area area = new Area(x, y, map, pos_relat - 1);

                foreach (Vector3 vector in area.processVertex())
                    vertex.Add(vector);
                foreach (int vector in area.procesarTriangles())
                    triangles.Add(vector);
                foreach (Vector2 vector in area.processUv())
                    uv.Add(vector);

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

public class Area {
    private int x_sup_left;
    private int y_sup_left;
    private int[,] map;
    private int pos_relat;

    public Area(int x, int y, int[,] map, int pos_relat) {
        x_sup_left = x;
        y_sup_left = y;
        this.map = map;
        this.pos_relat = pos_relat;
    }

    public Vector3[] processVertex() {
        Vector3[] vertexArray = new Vector3[4];

        vertexArray[0] = new Vector3(x_sup_left, y_sup_left, map[x_sup_left, y_sup_left]);
        vertexArray[1] = new Vector3(x_sup_left + 1, y_sup_left, map[x_sup_left + 1, y_sup_left]);
        vertexArray[2] = new Vector3(x_sup_left, y_sup_left + 1, map[x_sup_left, y_sup_left + 1]);
        vertexArray[3] = new Vector3(x_sup_left + 1, y_sup_left + 1, map[x_sup_left + 1, y_sup_left + 1]);

        return vertexArray;
    }

    public int[] procesarTriangles() {
        int[] triangleArray = new int[6];

        triangleArray[0] = pos_relat;
        triangleArray[1] = pos_relat + 3;
        triangleArray[2] = pos_relat + 2;

        triangleArray[3] = pos_relat;
        triangleArray[4] = pos_relat + 1;
        triangleArray[5] = pos_relat + 3;

        return triangleArray;
    }

    public Vector2[] processUv() {
        Vector2[] uvArray = new Vector2[4];

        uvArray[0] = new Vector2(0, 1);
        uvArray[1] = new Vector2(1, 1);
        uvArray[2] = new Vector2(0, 0);
        uvArray[3] = new Vector2(1, 0);

        return uvArray;
    }

}