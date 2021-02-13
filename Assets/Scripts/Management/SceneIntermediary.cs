using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneTransitionSystem;

public class SceneIntermediary : MonoBehaviour
{
    public SceneName target;

    public void GoToTargetedScene() => STSSceneManager.LoadScene(target.ToString());
    public void Quit() => Application.Quit();
}