using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Grid : MonoBehaviour
{
     public Vector2Int Size;

     public GameObject prefab;
    [SerializeField] private GameObject enemys;
    [SerializeField] private GameObject parent;

    [SerializeField]  private Player player;

    public int[,] fields;//поле игры x, y

    List<Path> paths = new List<Path>();

    public List<Enemy> enemies;

    public delegate void Enemyacive();//делегат события
    public event Enemyacive enemyactive;//событие для активации бега за игроком всем проивникам

    // Start is called before the first frame update
    public void Awake()
    {
        fields = new int[Size.x * 2, Size.y * 2];//указывам поле 10x10

        player.Starter();//Старт у игрока там мы определяем маршрут к точке Exit

        fields[player.curpos.x, player.curpos.y] = 1;//место для игрока должно быть пустым

      //  paths.Add(new Path(player.transform.position, this, 30, new Vector2Int(player.curpos.x, player.curpos.y)));//

        int enemycount = 2;//количество противников

        List<Vector2Int> enemy = new List<Vector2Int>();//Позиция для спавна противника

        for (int i = 0; i < enemycount; i++)
        {
            enemy.Add(new Vector2Int(Random.RandomRange(3+i, Size.x * 2), Random.RandomRange(3 + i, Size.y * 2)));//маршрут для противника
        }

     
        for (int x = -Size.x; x < Size.x; x++)//проходимся по всем ячейкам
        {
            for (int y = -Size.y; y < Size.y; y++)
            {
                int w = (Size.y - x) - 1;
                int h = (Size.x - y) - 1;

                for(int i = 0;i < enemy.Count;i++)//проходим по каждому врагу и спавним его
                {
                    if (w == enemy[i].x && h == enemy[i].y)
                    {

                        GameObject en = Instantiate(enemys);

                        en.transform.position = (transform.position - (new Vector3(x + 0.5f, -0.4f, y + 0.5f)));

                        paths.Add(new Path(en.transform.position, this, 14, new Vector2Int(w, h)));

                        en.GetComponent<Enemy>().mypath = paths[paths.Count - 1];

                        en.GetComponent<FieldOfView>().player = player.transform;

                        en.GetComponent<Enemy>().player = player;

                        en.GetComponent<Enemy>().grid = this;

                        en.GetComponent<Enemy>().curpos = new Vector2Int(w, h);

                        en.name += Random.Range(0, 99);

                        enemies.Add(en.GetComponent<Enemy>());

                   
                    }
                }
          
            }
        }
   
 

        for (int x = -Size.x; x < Size.x; x++)//проходим по всем ячейкам и спавним препятствия
        {
            for (int y = -Size.y; y < Size.y; y++)
            {

                int w = (Size.y - x) - 1;
                int h = (Size.x - y) - 1;

                for (int i = 0; i < enemy.Count; i++)//проходим по каждому врагу и спавним его
                {
                    if (w == enemy[i].x && h == enemy[i].y || player.ts.Contains(new Vector2Int(w, h)))//enemy
                    {
                        fields[w, h] = 1;
                        continue;
                    }
                }
                int done = Random.RandomRange(0,3);

                if(fields[(w), (h)] == 0)
                {
                    if (done == 0)
                    {
                        GameObject gameObject = Instantiate(prefab);
                        gameObject.transform.position = (transform.position - (new Vector3(x + 0.5f, 0, y + 0.5f)));
                        fields[w, h] = 0;
                        gameObject.transform.parent = (parent.transform);
                        gameObject.transform.localScale = new Vector3(1, 1, 1);
                        gameObject.transform.transform.position += new Vector3(0, 0.5f, 0);
                    }
                    else
                    {
                        fields[w, h] = 1;
                    }
                }

            }
        }

    }


    private void Update()
    {
        if (player.scale.scale == 10) enemyactive?.Invoke();//Если шкала равна 10 тогда активируем событие
    }
}
