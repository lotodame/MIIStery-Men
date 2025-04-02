using UnityEngine;

public class ItemController : MonoBehaviour, InteractableA
{
    [SerializeField] Dialog dialog;

    public void Interact()
    {
        Debug.Log("Conversation with the tree");
        //DialogueManager.Instance.ShowDialog();
    }
}
