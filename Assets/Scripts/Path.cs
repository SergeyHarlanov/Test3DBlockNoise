using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path //поиск рандомного пути
{
    public Vector3 target_my;

    public List<Vector3> dirwas = new List<Vector3>();

    public List<Vector2Int> ts = new List<Vector2Int>();

    public Vector2Int str;

    Vector3[] dir = new Vector3[]//dir
    {
                new Vector3(-1, 0, 0),//left
                new Vector3(1, 0, 0),//right
                new Vector3(0, 0, 1)//up
    };

    public List<Vector3> myp = new List<Vector3>();


    public List<Vector3> ChakingforVisibility(Vector3 ps, Vector3 lastdir, Grid grid)
    {
        dirwas = new List<Vector3>();

        List<Vector3> dr = new List<Vector3>();

        for (int i = 0; i < dir.Length; i++)
        {
            if (!Bounds(ps + dir[i], grid))
            {
                if (lastdir == dir[0] && dir[i] == dir[1])
                {
                    continue;
                }
                if (lastdir == dir[1] && dir[i] == dir[0])
                {
                    continue;
                }
                dr.Add(ps + dir[i]);
                dirwas.Add(dir[i]);
            }
        }
        if (dr.Count == 0) return null;
        return dr;
    }
    public Path(Vector3 target, Grid grid, int countcell, Vector2Int curpos)//Находим рандомную позицию в рандомную сторону относительно наших ограничений
    {
        str = curpos;

        Vector3 lastdir = Vector3.zero;//последнее направление да бы не была влево, вправо, влево, вправо]

        target_my = target;

        for (int i = 0; i < countcell; i++)
        {
            if (i == 0)
            {
                int done = Random.Range(0, ChakingforVisibility(target, lastdir, grid).Count);

                myp.Add(ChakingforVisibility(target, lastdir, grid)[done]);

                if (dirwas[done] == dir[0])
                {
                    lastdir = dirwas[done];
                    curpos.x--;
                }
                if (dirwas[done] == dir[1])
                {
                    lastdir = dirwas[done];
                    curpos.x++;
                }
                if (dirwas[done] == dir[2])
                {
                    lastdir = dirwas[done];
                    curpos.y++;
                }

                ts.Add(new Vector2Int(curpos.x, curpos.y));

               grid.fields[curpos.x, curpos.y] = 1;
            }
            else
            {
                if (ChakingforVisibility(myp[i - 1], lastdir, grid) == null)
                {
                    return;
                }
                int done = Random.Range(0, ChakingforVisibility(myp[i - 1], lastdir, grid).Count);

                myp.Add(ChakingforVisibility(myp[i - 1], lastdir, grid)[done]);

                if (dirwas[done] == dir[0])
                {
                    lastdir = dirwas[done];
                    curpos.x--;
                }
                if (dirwas[done] == dir[1])
                {
                    lastdir = dirwas[done];
                    curpos.x++;
                }
                if (dirwas[done] == dir[2])
                {
                    lastdir = dirwas[done];
                    curpos.y++;
                }

                curpos.x = Mathf.Clamp(curpos.x, 0, grid.Size.x * 2);//limit
                curpos.y = Mathf.Clamp(curpos.y, 0, grid.Size.y * 2);//limit

               grid.fields[curpos.x, curpos.y] = 1;
                ts.Add(new Vector2Int(curpos.x, curpos.y));
            }

        }
    }
    public bool Bounds(Vector3 target, Grid grid)//Определяем limit
    {

        float minX = grid.transform.position.x - (grid.Size.x - 0.5f);
        float maxX = grid.transform.position.x - (-grid.Size.x + 0.5f);
        float minZ = grid.transform.position.z - grid.Size.y + 0.5f;
        float maxZ = grid.transform.position.z - (-grid.Size.y + 0.5f);
        float x = Mathf.Clamp(target.x, minX, maxX);
        float z = Mathf.Clamp(target.z, minZ, maxZ);



        if (target != new Vector3(x, target.y, z)) return true;
        return false;
    }

}
