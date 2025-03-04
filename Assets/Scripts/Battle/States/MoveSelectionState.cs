using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State<BattleSystem>
{
    [SerializeField] MoveSelectionUI selectionUI;
    [SerializeField] GameObject moveDetailsUI;

    // Input
    public List<Move> Moves { get; set; }

    public static MoveSelectionState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;
    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        selectionUI.SetMoves(Moves);

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnMoveSelected;
        selectionUI.OnBack += OnBack;

        moveDetailsUI.SetActive(true);
        bs.DialogBox.EnableDialogText(false);
    }

    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnMoveSelected;
        selectionUI.OnBack -= OnBack;

        selectionUI.ClearItems();

        moveDetailsUI.SetActive(false);
        bs.DialogBox.EnableDialogText(true);
    }

    void OnMoveSelected(int selection)
    {
        StartCoroutine(OnMoveSelectedAsync(selection));
    }

    IEnumerator OnMoveSelectedAsync(int selection)
    {
        int moveTarget = 0;
        if (bs.UnitCount > 1)
        {
            yield return bs.StateMachine.PushAndWait(TargetSelectionState.i);
            if (!TargetSelectionState.i.SelectionMade) yield break;

            moveTarget = TargetSelectionState.i.SelectedTarget;
        }

        bs.AddBattleAction(new BattleAction()
        {
            Type = BattleActionType.Move,
            SelectedMove = Moves[selection],
            Target = bs.EnemyUnits[moveTarget]
        });
    }

    void OnBack()
    {
        bs.StateMachine.ChangeState(ActionSelectionState.i);
    }
}
