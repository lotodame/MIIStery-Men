using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public Pokemon SelectedPokemon { get; private set; }

    bool isSwitchingPosition;
    int selectedIndexForSwitching = 0;

    public static PartyState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    PokemonParty playerParty;
    private void Start()
    {
        playerParty = PlayerController.i.GetComponent<PokemonParty>();
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

        SelectedPokemon = null;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;
    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    void OnPokemonSelected(int selection)
    {
        SelectedPokemon = partyScreen.SelectedMember;
        StartCoroutine(PokemonSelectedAction(selection));
    }

    IEnumerator PokemonSelectedAction(int selectedPokemonIndex)
    {
        var prevState = gc.StateMachine.GetPrevState();
        if (prevState == InventoryState.i)
        {
            StartCoroutine(GoToUseItemState());
        }
        else if (prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;

            DynamicMenuState.i.MenuItems = new List<string>() { "Shift", "Summary", "Cancel" };
            yield return gc.StateMachine.PushAndWait(DynamicMenuState.i);
            if (DynamicMenuState.i.SelectedItem == 0)
            {
                if (SelectedPokemon.HP <= 0)
                {
                    partyScreen.SetMessageText("You can't send out a fainted pokemon");
                    yield break;
                }
                if (battleState.BattleSystem.PlayerUnits.Any(u => u.Pokemon == SelectedPokemon))
                {
                    partyScreen.SetMessageText("You can't switch with the same pokemon");
                    yield break;
                }

                gc.StateMachine.Pop();
            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gc.StateMachine.PushAndWait(SummaryState.i);
            }
            else
            {
                yield break;
            }
        }
        else
        {
            if (isSwitchingPosition)
            {
                if (selectedIndexForSwitching == selectedPokemonIndex)
                {
                    partyScreen.SetMessageText("You can't switch with the same pokemon");
                    yield break;
                }

                isSwitchingPosition = false;

                var tmpPokemon = playerParty.Pokemons[selectedIndexForSwitching];
                playerParty.Pokemons[selectedIndexForSwitching] = playerParty.Pokemons[selectedPokemonIndex];
                playerParty.Pokemons[selectedPokemonIndex] = tmpPokemon;
                playerParty.PartyUpdated();

                yield break;
            }

            DynamicMenuState.i.MenuItems = new List<string>() { "Summary", "Switch Position", "Cancel" };
            yield return gc.StateMachine.PushAndWait(DynamicMenuState.i);
            if (DynamicMenuState.i.SelectedItem == 0)
            {
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gc.StateMachine.PushAndWait(SummaryState.i);
            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                // Switch Position
                isSwitchingPosition = true;
                selectedIndexForSwitching = selectedPokemonIndex;
                partyScreen.SetMessageText("Choose a pokemon to switch position with.");
            }
            else
            {
                yield break;
            }
        }
    }

    IEnumerator GoToUseItemState()
    {
        yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedPokemon = null;

        var prevState = gc.StateMachine.GetPrevState();
        if (prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;
            if (battleState.BattleSystem.PlayerUnits.Any(u => u.Pokemon.HP <= 0))
            {
                partyScreen.SetMessageText("You have to choose a pokemon to continue");
                return;
            }
        }

        gc.StateMachine.Pop();
    }
}
