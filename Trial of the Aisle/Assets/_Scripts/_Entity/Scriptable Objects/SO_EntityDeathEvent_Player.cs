using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Player Death Event", menuName = "Scriptable Objects/Events/Player Death Event Default")]
public class SO_EntityDeathEvent_Player : SO_EntityHealthEventBase
{

    public override void Initialize(GameObject gameObject)
    {

        DestroyPlayer();
    }

    private void DestroyPlayer()
    {
        GameManager.Instance.LoadSpecificSceneString("MainMenu");
    }

    public override void StartEventMethod()
    {
        throw new System.NotImplementedException();
    }



}

