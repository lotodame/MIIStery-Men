using UnityEngine;

public class ItemController : MonoBehaviour, InteractableA
{
    [SerializeField] DialogA dialog;

    public void Interact()
    {
        StartCoroutine(DialogManagerA.Instance.ShowDialog(dialog));
    }
}
