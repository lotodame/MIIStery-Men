using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonStorageBoxes : MonoBehaviour, ISavable
{
    const int numberOfBoxes = 16;
    const int numberOfSlots = 30;

    public int NumOfBoxes => numberOfBoxes;
    public int NumOfSlots => numberOfSlots;

    Pokemon[,] boxes = new Pokemon[numberOfBoxes, numberOfSlots];

    public void AddPokemon(Pokemon pokemon, int boxIndex, int slotIndex)
    {
        boxes[boxIndex, slotIndex] = pokemon;
    }

    public void RemovePokemon(int boxIndex, int slotIndex)
    {
        boxes[boxIndex, slotIndex] = null;
    }

    public Pokemon GetPokemon(int boxIndex, int slotIndex)
    {
        return boxes[boxIndex, slotIndex];
    }

    public void AddPokemonToEmptySlot(Pokemon pokemon)
    {
        for (int boxIndex = 0; boxIndex < numberOfBoxes; boxIndex++)
        {
            for (int slotIndex = 0; slotIndex < numberOfSlots; slotIndex++)
            {
                if (boxes[boxIndex, slotIndex] == null)
                {
                    boxes[boxIndex, slotIndex] = pokemon;
                    return;
                }
            }
        }
    }

    public static PokemonStorageBoxes GetPlayerStorageBoxes()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonStorageBoxes>();
    }

    public object CaptureState()
    {
        var saveData = new BoxSaveData()
        {
            boxSlots = new List<BoxSlotSaveData>()
        };

        for (int boxIndex = 0; boxIndex < numberOfBoxes; boxIndex++)
        {
            for (int slotIndex = 0; slotIndex < numberOfSlots; slotIndex++)
            {
                if (boxes[boxIndex, slotIndex] != null)
                {
                    var boxSlot = new BoxSlotSaveData()
                    {
                        pokemonData = boxes[boxIndex, slotIndex].GetSaveData(),
                        boxIndex = boxIndex,
                        slotIndex = slotIndex
                    };

                    saveData.boxSlots.Add(boxSlot);
                }
            }
        }

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as BoxSaveData;

        // Clear all the data in boxes
        for (int boxIndex = 0; boxIndex < numberOfBoxes; boxIndex++)
        {
            for (int slotIndex = 0; slotIndex < numberOfSlots; slotIndex++)
            {
                boxes[boxIndex, slotIndex] = null;
            }
        }

        // Load data
        foreach (var slot in saveData.boxSlots)
        {
            boxes[slot.boxIndex, slot.slotIndex] = new Pokemon(slot.pokemonData);
        }
    }
}

[System.Serializable]
public class BoxSaveData
{
    public List<BoxSlotSaveData> boxSlots;
}

[System.Serializable]
public class BoxSlotSaveData
{
    public PokemonSaveData pokemonData;
    public int boxIndex;
    public int slotIndex;
}
