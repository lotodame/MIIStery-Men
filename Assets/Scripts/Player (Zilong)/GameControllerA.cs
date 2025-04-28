using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Dialog}

public class GameControllerA : MonoBehaviour
{
    [SerializeField] PlayerControllerA playerControllerA;

    GameState state;

    private void Start()
    {
        DialogManagerA.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManagerA.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerControllerA.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManagerA.Instance.HandleUpdate();
        }
    } 
}
