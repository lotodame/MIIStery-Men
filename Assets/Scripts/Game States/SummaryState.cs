using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummaryState : State<GameController>
{
    [SerializeField] SummaryScreenUI summaryScreen;

    int selectedPage = 0;

    // Input
    public int SelectedPokemonIndex { get; set; }

    public static SummaryState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    List<Pokemon> playerParty;
    private void Start()
    {
        playerParty = PlayerController.i.GetComponent<PokemonParty>().Pokemons;
    }

    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

        summaryScreen.gameObject.SetActive(true);
        summaryScreen.SetBasicDetails(playerParty[SelectedPokemonIndex]);
        summaryScreen.ShowPage(selectedPage);
    }

    public override void Execute()
    {

        if (!summaryScreen.InMoveSelection)
        {
            // Page Selection
            int prevPage = selectedPage;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedPage = Mathf.Abs((selectedPage - 1) % 2);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedPage = (selectedPage + 1) % 2;
            }

            if (selectedPage != prevPage)
            {
                summaryScreen.ShowPage(selectedPage);
            }

            // Pokemon Selection
            int prevSelection = SelectedPokemonIndex;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SelectedPokemonIndex += 1;

                if (SelectedPokemonIndex >= playerParty.Count)
                    SelectedPokemonIndex = 0;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SelectedPokemonIndex -= 1;

                if (SelectedPokemonIndex < 0)
                    SelectedPokemonIndex = playerParty.Count - 1;
            }

            if (SelectedPokemonIndex != prevSelection)
            {
                summaryScreen.SetBasicDetails(playerParty[SelectedPokemonIndex]);
                summaryScreen.ShowPage(selectedPage);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (selectedPage == 1 && !summaryScreen.InMoveSelection)
            {
                summaryScreen.InMoveSelection = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (summaryScreen.InMoveSelection)
            {
                summaryScreen.InMoveSelection = false;
            }
            else
            {
                gc.StateMachine.Pop();
                return;
            }
        }

        summaryScreen.HandleUpdate();
    }

    public override void Exit()
    {
        summaryScreen.gameObject.SetActive(false);
    }
}
