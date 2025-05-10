using UnityEngine;

public class ItemController : MonoBehaviour, InteractableA
{
    [SerializeField] DialogA dialog;

    // Optional: Only assign this if the item is the Phone
    [SerializeField] PhoneRingController phoneRinger;

    public void Interact()
    {
        // Only stop ringing if this item has a reference to the phone ringer
        if (phoneRinger != null)
        {
            phoneRinger.StopRinging();
        }

        StartCoroutine(DialogManagerA.Instance.ShowDialog(dialog));
    }
}
