using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoManager
{
    private const int MENU_SCENE_INDEX = 0;
    private const int GAME_SCENE_INDEX = 1;

    public void LoadGame() => UnityEngine.SceneManagement.SceneManager.LoadScene(GAME_SCENE_INDEX);
    public void LoadMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(MENU_SCENE_INDEX);
}
