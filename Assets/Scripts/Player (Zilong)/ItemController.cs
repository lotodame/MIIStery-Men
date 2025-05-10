using UnityEngine;

public class ItemController : MonoBehaviour, InteractableA
{
    [SerializeField] DialogA correctDialog;
    [SerializeField] DialogA fallbackDialog;
    [SerializeField] int sequenceIndex;

    [SerializeField] SequenceManager sequenceManager;
    [SerializeField] PhoneRingController phoneRinger; // Only assign this on the phone item

    public void Interact()
    {
        if (sequenceManager == null)
        {
            Debug.LogWarning("SequenceManager not assigned.");
            return;
        }

        if (sequenceManager.IsCorrectStep(sequenceIndex))
        {
            sequenceManager.Advance();

            // ✅ Stop phone ringing only if this item has the ringer assigned
            if (phoneRinger != null)
            {
                phoneRinger.StopRinging();
            }

            StartCoroutine(DialogManagerA.Instance.ShowDialog(correctDialog));
        }
        else
        {
            StartCoroutine(DialogManagerA.Instance.ShowDialog(fallbackDialog));
        }
    }
}
