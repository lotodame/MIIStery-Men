using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI selectionUI;

    public static ActionSelectionState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;
    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnActionSelected;

        bs.DialogBox.SetDialog($"Choose an action for {bs.UnitInSelection.Pokemon.Base.Name}");
    }

    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnActionSelected;
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0)
        {
            // Fight
            MoveSelectionState.i.Moves = bs.UnitInSelection.Pokemon.Moves;
            bs.StateMachine.ChangeState(MoveSelectionState.i);
        }
        else if (selection == 1)
        {
            // Bag
            StartCoroutine(GoToInventoryState());
        }
        else if (selection == 2)
        {
            // Pokemon
            StartCoroutine(GoToPartyState());
        }
        else if (selection == 3)
        {
            // Run
            bs.AddBattleAction(new BattleAction()
            {
                Type = BattleActionType.Run
            });
        }
    }

    IEnumerator GoToPartyState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedPokemon = PartyState.i.SelectedPokemon;
        if (selectedPokemon != null)
        {
            bs.AddBattleAction(new BattleAction()
            {
                Type = BattleActionType.SwitchPokemon,
                SelectedPokemon = selectedPokemon
            });
        }
    }

    IEnumerator GoToInventoryState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(InventoryState.i);
        var selectedItem = InventoryState.i.SelectedItem;
        if (selectedItem != null)
        {
            bs.AddBattleAction(new BattleAction()
            {
                Type = BattleActionType.UseItem,
                SelectedItem = selectedItem
            });
        }
    }
}
