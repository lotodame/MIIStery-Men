using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunTurnState : State<BattleSystem>
{
    public static RunTurnState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    // Input
    public List<BattleAction> Actions { get; set; }

    BattleDialogBox dialogBox;
    PartyScreen partyScreen;
    bool isTrainerBattle;
    PokemonParty playerParty;
    PokemonParty trainerParty;

    BattleSystem bs;
    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        dialogBox = bs.DialogBox;
        partyScreen = bs.PartyScreen;
        isTrainerBattle = bs.IsTrainerBattle;
        playerParty = bs.PlayerParty;
        trainerParty = bs.TrainerParty;

        StartCoroutine(RunTurns());
    }

    IEnumerator RunTurns()
    {
        foreach (var action in Actions)
        {
            if (action.IsInvalid)
                continue;

            if (action.Type == BattleActionType.Move)
            {
                action.User.Pokemon.CurrentMove = action.SelectedMove;

                yield return RunMove(action.User, action.Target, action.SelectedMove);
                yield return RunAfterTurn(action.User);
            }
            else if (action.Type == BattleActionType.SwitchPokemon)
            {
                yield return bs.SwitchPokemon(action.SelectedPokemon, action.User);
            }
            else if (action.Type == BattleActionType.UseItem)
            {
                if (action.SelectedItem is PokeballItem)
                {
                    yield return bs.ThrowPokeball(action.SelectedItem as PokeballItem);
                }
                else
                {
                    // This is handled from item screen, so do nothing and skip to enemy move
                }
            }
            else if (action.Type == BattleActionType.Run)
            {
                yield return TryToEscape();
            }

            if (bs.IsBattleOver) break;
        }

        bs.ClearTurnData();

        if (!bs.IsBattleOver)
            bs.StateMachine.ChangeState(ActionSelectionState.i);
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {

            sourceUnit.PlayAttackAnimation();
            AudioManager.i.PlaySfx(move.Base.Sound);

            yield return new WaitForSeconds(1f);

            targetUnit.PlayHitAnimation();
            AudioManager.i.PlaySfx(AudioId.Hit);

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }

        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // Status Condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        // Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (bs.IsBattleOver) yield break;

        // Statuses like burn or psn will hurt the pokemon after the turn
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
        }
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            bool battlWon = true;
            if (isTrainerBattle)
                battlWon = trainerParty.GetHealthyPokemon() == null;

            if (battlWon)
                AudioManager.i.PlayMusic(bs.BattleVictoryMusic);

            // Exp Gain
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            expGain = expGain / bs.UnitCount;

            for (int i = 0; i < bs.UnitCount; i++)
            {
                var playerUnit = bs.PlayerUnits[i];

                playerUnit.Pokemon.Exp += expGain;
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp");
                yield return playerUnit.Hud.SetExpSmooth();

                // Check Level Up
                while (playerUnit.Pokemon.CheckForLevelUp())
                {
                    playerUnit.Hud.SetLevel();
                    yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}");

                    // Try to learn a new Move
                    var newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrLevel();
                    if (newMove != null)
                    {
                        if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                        {
                            playerUnit.Pokemon.LearnMove(newMove.Base);
                            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name}");
                            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                        }
                        else
                        {
                            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} trying to learn {newMove.Base.Name}");
                            yield return dialogBox.TypeDialog($"But it cannot learn more than {PokemonBase.MaxNumOfMoves} moves");
                            yield return dialogBox.TypeDialog($"Choose a move a move to forget");

                            MoveToForgetState.i.CurrentMoves = playerUnit.Pokemon.Moves.Select(x => x.Base).ToList();
                            MoveToForgetState.i.NewMove = newMove.Base;
                            yield return GameController.Instance.StateMachine.PushAndWait(MoveToForgetState.i);

                            var moveIndex = MoveToForgetState.i.Selection;
                            if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
                            {
                                // Don't learn the new move
                                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} did not learn {newMove.Base.Name}");
                            }
                            else
                            {
                                // Forget the selected move and learn new move
                                var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.Name} and learned {newMove.Base.Name}");

                                playerUnit.Pokemon.Moves[moveIndex] = new Move(newMove.Base);
                            }
                        }
                    }

                    yield return playerUnit.Hud.SetExpSmooth(true);
                }
            }

            yield return new WaitForSeconds(1f);
        }

        yield return NextStepsAfterFainting(faintedUnit);
    }

    IEnumerator NextStepsAfterFainting(BattleUnit faintedUnit)
    {
        // Remove the action of the fainted
        var actionToRemove = Actions.FirstOrDefault(a => a.User == faintedUnit);
        if (actionToRemove != null)
            actionToRemove.IsInvalid = true;

        if (faintedUnit.IsPlayerUnit)
        {
            var activePokemons = bs.PlayerUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

            var nextPokemon = playerParty.GetHealthyPokemon(dontInclude: activePokemons);
            if (nextPokemon == null && activePokemons.Count == 0)
            {
                // End the battle
                bs.BattleOver(false);
            }
            else if (nextPokemon == null && activePokemons.Count > 0)
            {
                // No new pokemon to send out, but we can continue the battle with the active pokemon
                bs.PlayerUnits.Remove(faintedUnit);
                faintedUnit.Hud.gameObject.SetActive(false);

                // Attacks targeted at the fainted unit should be changed
                var actionsToChange = Actions.Where(a => a.Target == faintedUnit).ToList();
                actionsToChange.ForEach(a => a.Target = bs.PlayerUnits.First());
            }
            else if (nextPokemon != null)
            {
                // Send out the next pokemon
                yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
                yield return bs.SwitchPokemon(PartyState.i.SelectedPokemon, faintedUnit);
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                bs.BattleOver(true);
                yield break;
            }

            var activePokemons = bs.EnemyUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

            var nextPokemon = trainerParty.GetHealthyPokemon(dontInclude: activePokemons);
            if (nextPokemon == null && activePokemons.Count == 0)
            {
                // End the battle
                bs.BattleOver(false);
            }
            else if (nextPokemon == null && activePokemons.Count > 0)
            {
                // No new pokemon to send out, but we can continue the battle with the active pokemon
                bs.EnemyUnits.Remove(faintedUnit);
                faintedUnit.Hud.gameObject.SetActive(false);

                // Attacks targeted at the fainted unit should be changed
                var actionsToChange = Actions.Where(a => a.Target == faintedUnit).ToList();
                actionsToChange.ForEach(a => a.Target = bs.EnemyUnits.First());
            }
            else if (nextPokemon != null)
            {
                // Send out the next pokemon
                if (bs.UnitCount == 1)
                {
                    AboutToUseState.i.NewPokemon = nextPokemon;
                    yield return bs.StateMachine.PushAndWait(AboutToUseState.i);
                }
                else
                {
                    bs.SendNextTrainerPokemon();
                }
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    IEnumerator TryToEscape()
    {

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from trainer battles!");
            yield break;
        }

        ++bs.EscapeAttempts;

        int playerSpeed = bs.PlayerUnits[0].Pokemon.Speed;
        int enemySpeed = bs.EnemyUnits[0].Pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            bs.BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * bs.EscapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                bs.BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Can't escape!");
            }
        }
    }
}
