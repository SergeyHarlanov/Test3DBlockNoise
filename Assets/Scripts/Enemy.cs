using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]

    public Path mypath;

    public float myspeed = 2;

    public Vector2Int curpos;

    public Player player;

    public Grid grid;

    private Vector2Int posplayer;//позиция игрока


    void Start()
    {
        grid.enemyactive += Grid_enemyactive;
        StartCoroutine(move());
    }
 
    public void Grid_enemyactive()//подписка на событие активности и бег за игроком
    {
        if (!this) return;

        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        Vector3 dir = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * myspeed);
        GetComponent<FieldOfView>().ViewMeshFilter.GetComponent<MeshRenderer>().material.color = Color.red;//указываем игроку что мы в активности за ним бегать
    }

    private IEnumerator move()
    {
        bool back = false;

        for(int i=0;i< 9999;i++)
        {
            if (i == mypath.myp.Count - 1)
            {
                back = true;
            }
            if (i == 0)
            {
                back = false;
            }

            if (back)
            {
               
               Vector3 dir = (mypath.myp[i - 1] - transform.position).normalized;
                while (transform.position != mypath.myp[i - 1])//меняем Vector
                {
                    if (GetComponent<NavMeshAgent>().enabled) yield break;
                    if (dir != Vector3.zero) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * myspeed * 3);

                    transform.position = Vector3.MoveTowards(transform.position, mypath.myp[i - 1], Time.deltaTime * myspeed / 1.5f);
                    yield return null;
                }
                i -= 2;
            }
            else
            {
               
              Vector3 dir = (mypath.myp[i] - transform.position).normalized;
         
                while (transform.position != mypath.myp[i])//меняем vector
                {
                    if (GetComponent<NavMeshAgent>().enabled) yield break;
                    if (dir != Vector3.zero) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * myspeed * 3);

                    transform.position = Vector3.MoveTowards(transform.position, mypath.myp[i], Time.deltaTime * myspeed);
                    yield return null;
                }
            }

        }
    }

    public bool CheckPath(int x, int y)
    {
        int xx = (mypath.ts.Count == 0 ? curpos.x : mypath.ts[mypath.ts.Count - 1].x) + x;
        int yy = (mypath.ts.Count == 0 ? curpos.y : mypath.ts[mypath.ts.Count - 1].y) + y;

        if (grid.fields[xx, yy] == 1)
        {
            return true;
        }
        return false;
    }
}
