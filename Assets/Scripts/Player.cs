using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private Grid grid;

    public bool move_now;//проверяем в движение ли мы сейчас

    public Vector2Int curpos = new Vector2Int(9, 9);

    //для рандомного путя до точки
    public List<Vector3> myp = new List<Vector3>();
    public List<Vector2Int> ts = new List<Vector2Int>();

    public GameObject tableend;

    [Header("Scale")]

    public Scale scale;
    [Header("NewData")]

    public Vector3 startposplayer;
    public Vector3 startposcamera;
    private void Start()
    {
        startposplayer = transform.position;//Стартовая позиция игрока
        startposcamera = Camera.main.transform.position;//стартовая позиция камеры
    }
    private void Update()
    {
        Camera.main.transform.position = startposcamera - (startposplayer - transform.position);//позиция камеры новая

        Movements();//движение игрока

        if (!move_now) GetComponent<Animator>().Play("Idle01");//проверяем проигрывать ли нам анимацию
    }
    public void StateIdleEnd()
    {
        StopAllCoroutines();
        move_now = false;
        GetComponent<Animator>().Play("Idle01");
    }
    public void End(bool win)//конец игры
    {
        tableend.gameObject.SetActive(true);

        if (win )//в случае если мы выйграли в противном оставляем всё как есть
        {
            StateIdleEnd();
            tableend.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 128, 0, 0.5f);//background
            tableend.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "You win";//header
            this.enabled = false;//отключаем управление у игрока да бы дать подумать пользователю
        }
        else
        {
            StateIdleEnd();
        }
    }

    public bool ActiveMove(Vector3 forward, Vector2Int index)//проверяем пустая ли ячейку куда мы движемся и не уходим ли мы за ограничения
    {
        if (curpos.x + index.x >= 0 && curpos.y + index.y >= 0 && (curpos.x + index.x) < (grid.Size.x * 2 ) && (curpos.y + index.y) < (grid.Size.y * 2 ))
        {
            if(grid.fields[curpos.x + index.x, curpos.y + index.y] == 1 && Waslimit(transform.position + forward))
            return true;
        }
        return false;
    }
    private void Movements()//движение игрока
    {
        if (Input.GetKeyDown(KeyCode.A) && !move_now && ActiveMove(new Vector3(-1, 0, 0), new Vector2Int(-1, 0)))
        {
            scale.ScaleIncrease();
            curpos.x--;//проверяем limit ли у нас что бы мы не смогли изменит вектор просто стоя на месте и пытаясь пройти через ограничение
            StopAllCoroutines();
            StartCoroutine(Move(transform.position + new Vector3(-1, 0, 0)));
        }
        if (Input.GetKeyDown(KeyCode.D) && !move_now && ActiveMove(new Vector3(1, 0, 0), new Vector2Int(1, 0)))
        {
            scale.ScaleIncrease();
            curpos.x++;//проверяем limit ли у нас что бы мы не смогли изменит вектор просто стоя на месте и пытаясь пройти через ограничение
            StopAllCoroutines();
            StartCoroutine(Move(transform.position + new Vector3(1, 0, 0)));
        }
        if (Input.GetKeyDown(KeyCode.W) && !move_now && ActiveMove(new Vector3(0, 0, 1), new Vector2Int(0, 1)))
        {
            scale.ScaleIncrease();
            curpos.y++;//проверяем limit ли у нас что бы мы не смогли изменит вектор просто стоя на месте и пытаясь пройти через ограничение
            StopAllCoroutines();
            StartCoroutine(Move(transform.position + new Vector3(0, 0, 1)));
        }
        if (Input.GetKeyDown(KeyCode.S) && !move_now && ActiveMove(new Vector3(0, 0, -1), new Vector2Int(0, -1)))
        {
            scale.ScaleIncrease();
            curpos.y--;//проверяем limit ли у нас что бы мы не смогли изменит вектор просто стоя на месте и пытаясь пройти через ограничение
            StopAllCoroutines();
            StartCoroutine(Move(transform.position + new Vector3(0, 0, -1)));
        }
    }
 
    public IEnumerator Move(Vector3 targetpos)//происходит движение
    {
        Vector3 dir = (targetpos - transform.position).normalized;

        while (transform.position != targetpos )
        {

            GetComponent<Animator>().Play("Move01");
            //limit
            float minX = grid.transform.position.x - (grid.Size.x - 0.5f);
            float maxX = grid.transform.position.x - (-grid.Size.x + 0.5f);
            float minZ = grid.transform.position.z - (grid.Size.y - 0.5f);
            float maxZ = grid.transform.position.z - (-grid.Size.y + 0.5f);
            targetpos.x = Mathf.Clamp(targetpos.x, minX, maxX);
            targetpos.z = Mathf.Clamp(targetpos.z, minZ, maxZ);

            //change
            transform.position = Vector3.MoveTowards(transform.position, targetpos, Time.deltaTime * Speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * Speed* 3);

          

            if (transform.position == targetpos)//если мы завершили движение
            {
                if(curpos.x == grid.Size.x*2 - 1 && curpos.y == grid.Size.y * 2 - 1)//проверяем достиглы ли мы точки EXIT
                {
                    End(true);
                }
                move_now = false;//если стоим
            }
            else move_now = true;//если все же движемся

            yield return null;

        }
    }
    public bool Waslimit(Vector3 targetpos)//проверяем на лимит что бы за определнную границу не могли выйти
    {
        float minX = grid.transform.position.x - (grid.Size.x - 0.5f);
        float maxX = grid.transform.position.x - (-grid.Size.x + 0.5f);
        float minZ = grid.transform.position.z - grid.Size.y + 0.5f;
        float maxZ = grid.transform.position.z - (-grid.Size.y + 0.5f);
        targetpos.x = Mathf.Clamp(targetpos.x, minX, maxX);
        targetpos.z = Mathf.Clamp(targetpos.z, minZ, maxZ);
        if (transform.position == targetpos) return false;
        else return true;
    }

    public bool CheckPath(int x, int y)
    {
        int xx = (ts.Count == 0 ? curpos.x : ts[ts.Count - 1].x) + x;
        int yy = (ts.Count == 0 ? curpos.y : ts[ts.Count - 1].y) + y;

        if (grid.fields[xx, yy] == 1)
        {
            return true;
        }
        return false;
    }


    public void Starter()//Первые несколько линий мы делаем рандомно а после уже достраиваем путь до точки выхода (EXIT)
    {
        Path path = new Path(transform.position, grid, 20, new Vector2Int(curpos.x, curpos.y));
        for (int i = 0; i < path.myp.Count; i++)
        {
            myp.Add(path.myp[i]);
            ts.Add(path.ts[i]);
        }
        BuildingNavigation();
    }


    public void BuildingNavigation()//дорога к выходу она будет пустая в таком случае
    {
        float x = (myp.Count == 0 ? curpos.x : ts[ts.Count - 1].x) - 9;
        float y = (myp.Count == 0 ? curpos.y : ts[ts.Count - 1].y) - 9;

        while (x != 0 || y != 0)
        {
            x = (myp.Count == 0 ? curpos.x : ts[ts.Count - 1].x) - 9;
            y = (myp.Count == 0 ? curpos.y : ts[ts.Count - 1].y) - 9;
            if ((myp.Count == 0 ? curpos.x : ts[ts.Count - 1].x) - 9 < 0)
            {

                Vector3 nextp;
                if (myp.Count == 0)
                {
                    nextp = transform.position + new Vector3(1, 0, 0);//cash
                }
                else
                {
                    nextp = myp[myp.Count - 1] + new Vector3(1, 0, 0);//cash
                }
                myp.Add(nextp);

                int xx = (ts.Count == 0 ? curpos.x : ts[ts.Count - 1].x) + 1;
                ts.Add(new Vector2Int(xx, (ts.Count == 0 ? curpos.y : ts[ts.Count - 1].y)));

            }
            if ((myp.Count == 0 ? curpos.x : ts[ts.Count - 1].x) - 9 > 0)
            {
                Vector3 nextp;

                if (myp.Count == 0)
                {
                    nextp = transform.position + new Vector3(-1, 0, 0);//cash
                }
                else
                {
                    nextp = myp[myp.Count - 1] + new Vector3(-1, 0, 0);//cash
                }

                myp.Add(nextp);

                int xx = (ts.Count == 0 ? curpos.x : ts[ts.Count - 1].x) - 1;

                ts.Add(new Vector2Int(xx, (ts.Count == 0 ? curpos.y : ts[ts.Count - 1].y)));
            }
            if ((myp.Count == 0 ? curpos.y : ts[ts.Count - 1].y) - 9 < 0)
            {
                Vector3 nextp;
                if (myp.Count == 0)
                {
                    nextp = transform.position + new Vector3(0, 0, 1);//cash
                }
                else
                {
                    nextp = myp[myp.Count - 1] + new Vector3(0, 0, 1);//cash
                }

                myp.Add(nextp);
                int yy = (ts.Count == 0 ? curpos.y : ts[ts.Count - 1].y) + 1;
                ts.Add(new Vector2Int((ts.Count == 0 ? curpos.x : ts[ts.Count - 1].x), yy));
            }
            if ((myp.Count == 0 ? curpos.y : ts[ts.Count - 1].y) - 9 > 0)
            {
                Vector3 nextp;
                if (myp.Count == 0)
                {
                    nextp = transform.position + new Vector3(0, 0, -1);//cash
                }
                else
                {
                    nextp = myp[myp.Count - 1] + new Vector3(0, 0, -1);//cash
                }

                myp.Add(nextp);

                int yy = (ts.Count == 0 ? curpos.y : ts[ts.Count - 1].y) - 1;

                ts.Add(new Vector2Int((ts.Count == 0 ? curpos.x : ts[ts.Count - 1].x), yy));
            }

        }
    
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < myp.Count; i++)
        {
            if (i == 0) Gizmos.DrawLine(transform.position, myp[i]);
            else Gizmos.DrawLine(myp[i - 1], myp[i]);
        }
    }
}