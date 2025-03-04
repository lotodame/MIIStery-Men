using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public event Action OnUpdated;

    public List<Pokemon> Pokemons {
        get {
            return pokemons;
        }
        set {
            pokemons = value;
            OnUpdated?.Invoke();
        }
    }

    PokemonStorageBoxes storageBoxes;
    private void Awake()
    {
        storageBoxes = GetComponent<PokemonStorageBoxes>();
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    private void Start()
    {

    }

    public Pokemon GetHealthyPokemon(List<Pokemon> dontInclude = null)
    {
        var healthyPokemons = pokemons.Where(x => x.HP > 0).ToList();
        if (dontInclude != null)
            healthyPokemons = healthyPokemons.Where(p => !dontInclude.Contains(p)).ToList();

        return healthyPokemons.FirstOrDefault();
    }

    public List<Pokemon> GetHealthyPokemons(int unitCount)
    {
        return pokemons.Where(x => x.HP > 0).Take(unitCount).ToList();
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
            OnUpdated?.Invoke();
        }
        else
        {
            storageBoxes.AddPokemonToEmptySlot(newPokemon);
        }
    }

    public bool CheckForEvolutions()
    {
        return pokemons.Any(p => p.CheckForEvolution() != null);
    }

    public IEnumerator RunEvolutions()
    {
        foreach (var pokemon in pokemons)
        {
            var evoution = pokemon.CheckForEvolution();
            if (evoution != null)
            {
                yield return EvolutionManager.i.Evolve(pokemon, evoution);
            }
        }
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }
}
