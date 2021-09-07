using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public void loadingScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
