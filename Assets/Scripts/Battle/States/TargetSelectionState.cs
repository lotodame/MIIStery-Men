using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionState : State<BattleSystem>
{
    int selectedTarget = 0;

    // Output
    public bool SelectionMade { get; private set; }
    public int SelectedTarget => selectedTarget;

    public static TargetSelectionState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;
    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        SelectionMade = false;
        selectedTarget = 0;
        UpdateSelectionInUI();
    }

    public override void Execute()
    {
        int prevSelection = selectedTarget;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedTarget;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedTarget;

        selectedTarget = Mathf.Clamp(selectedTarget, 0, bs.EnemyUnits.Count - 1);

        if (selectedTarget != prevSelection)
            UpdateSelectionInUI();

        if (Input.GetButtonDown("Action"))
        {
            SelectionMade = true;
            bs.StateMachine.Pop();
        }
        else if (Input.GetButtonDown("Back"))
        {
            SelectionMade = false;
            bs.StateMachine.Pop();
        }
    }

    public override void Exit()
    {
        bs.EnemyUnits[selectedTarget].SetSelected(false);
    }

    void UpdateSelectionInUI()
    {
        for (int i = 0; i < bs.EnemyUnits.Count; i++)
        {
            bs.EnemyUnits[i].SetSelected(i == selectedTarget);
        }
    }
}
