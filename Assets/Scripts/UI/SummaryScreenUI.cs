using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SummaryScreenUI : SelectionUI<TextSlot>
{
    [Header("Basic Details")]
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Image image;

    [Header("Pages")]
    [SerializeField] Text pageNameTxt;
    [SerializeField] GameObject skillsPage;
    [SerializeField] GameObject movesPage;

    [Header("Pokemon Skills")]
    [SerializeField] Text hpText;
    [SerializeField] Text attackText;
    [SerializeField] Text defenseText;
    [SerializeField] Text spAttackText;
    [SerializeField] Text spDefenseText;
    [SerializeField] Text speedText;
    [SerializeField] Text expPointsText;
    [SerializeField] Text nextLevelExpText;
    [SerializeField] Transform expBar;

    [Header("Pokemon Moves")]
    [SerializeField] List<Text> moveTypes;
    [SerializeField] List<Text> moveNames;
    [SerializeField] List<Text> movePPs;
    [SerializeField] Text moveDescriptionText;
    [SerializeField] Text movePowerText;
    [SerializeField] Text moveAccuracyText;
    [SerializeField] GameObject moveEffectsUI;

    List<TextSlot> moveSlots;
    private void Start()
    {
        moveSlots = moveNames.Select(m => m.GetComponent<TextSlot>()).ToList();
        moveEffectsUI.SetActive(false);
        moveDescriptionText.text = "";
    }

    bool inMoveSelection;
    public bool InMoveSelection 
    {
        get => inMoveSelection;
        set {
            inMoveSelection = value;

            if (inMoveSelection)
            {
                moveEffectsUI.SetActive(true);
                SetItems(moveSlots.Take(pokemon.Moves.Count).ToList());
            }
            else
            {
                moveEffectsUI.SetActive(false);
                moveDescriptionText.text = "";
                ClearItems();
            }
        }
    }

    Pokemon pokemon;
    public void SetBasicDetails(Pokemon pokemon)
    {
        this.pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv. " + pokemon.Level;
        image.sprite = pokemon.Base.FrontSprite;
    }

    public void ShowPage(int pageNum)
    {
        if (pageNum == 0)
        {
            // Show the skills page
            pageNameTxt.text = "Pokemon Skills";

            skillsPage.SetActive(true);
            movesPage.SetActive(false);

            SetSkills();
        }
        else if (pageNum == 1)
        {
            // Show the moves page
            pageNameTxt.text = "Pokemon Moves";

            skillsPage.SetActive(false);
            movesPage.SetActive(true);

            SetMoves();
        }
    }

    public void SetSkills()
    {
        hpText.text = $"{pokemon.HP}/{pokemon.MaxHp}";
        attackText.text = "" + pokemon.Attack;
        defenseText.text = "" + pokemon.Defense;
        spAttackText.text = "" + pokemon.SpAttack;
        spDefenseText.text = "" + pokemon.SpDefense;
        speedText.text = "" + pokemon.Speed;

        expPointsText.text = "" + pokemon.Exp;
        nextLevelExpText.text = "" + (pokemon.Base.GetExpForLevel(pokemon.Level + 1) - pokemon.Exp);
        expBar.localScale = new Vector2(pokemon.GetNormalizedExp(), 1);
    }

    public void SetMoves()
    {
        for (int i = 0; i < moveNames.Count; i++)
        {
            if (i < pokemon.Moves.Count)
            {
                var move = pokemon.Moves[i];

                moveTypes[i].text = move.Base.Type.ToString().ToUpper();
                moveNames[i].text = move.Base.Name.ToUpper();
                movePPs[i].text = $"PP {move.PP}/{move.Base.PP}";
            }
            else
            {
                moveTypes[i].text = "-";
                moveNames[i].text = "-";
                movePPs[i].text = "-";
            }
        }
    }

    public override void HandleUpdate()
    {
        if (InMoveSelection)
            base.HandleUpdate();
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        var move = pokemon.Moves[selectedItem];

        moveDescriptionText.text = move.Base.Description;
        movePowerText.text = "" + move.Base.Power;
        moveAccuracyText.text = "" + move.Base.Accuracy;
    }
}
