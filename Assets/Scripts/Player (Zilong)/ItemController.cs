using UnityEngine;

public class ItemController : MonoBehaviour, InteractableA
{
    [SerializeField] DialogA dialog;
    [SerializeField] PhoneRingController phoneRinger; // drag in the Inspector

    public void Interact()
    {
        phoneRinger.StopRinging(); // stop sound when interacted with
        StartCoroutine(DialogManagerA.Instance.ShowDialog(dialog));
    }
}
