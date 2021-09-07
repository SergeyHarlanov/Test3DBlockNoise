using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Scale : MonoBehaviour
{
    [SerializeField] Image scalebar;
    public float scale = 0;
    [Header("Player")]
    [SerializeField] Player player;
    [Header("Time")]
    public float time;
    public async void ScaleIncrease()//увл шкалу
    {
        time = 0;
        for (int i=0;i<30;i++)
        {
            scale = Mathf.Clamp(scale + 0.1f, 0, 10);
            scalebar.fillAmount = scale * 0.1f;
            await Task.Delay(10);
        }
    }
    public async void ScaleDicrease()//умен шкалу
    {
        time = 0;
        for (int i = 0; i < 10; i++)
        {
            scale = Mathf.Clamp(scale - 0.1f, 0, 10);
            scalebar.fillAmount = scale * 0.1f;
            await Task.Delay(45);
        }
    }
    private void Update()
    {
        if(!player.move_now)//проверяем в каком состоянии у нас игрок, нам нужно состояние idle
        {
            time += Time.deltaTime;
            if (time >= 0.5f) ScaleDicrease();
        }
      
    }
}
