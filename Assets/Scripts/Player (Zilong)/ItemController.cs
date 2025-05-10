using UnityEngine;

public class ItemController : MonoBehaviour, InteractableA
{
    [SerializeField] DialogA correctDialog;
    [SerializeField] DialogA fallbackDialog;
    [SerializeField] int sequenceIndex;

    [Header("Optional")]
    [SerializeField] PhoneRingController phoneRinger; // Assign only if this is the phone
    [SerializeField] bool isFinalItem = false; // Enable if this is the last item in sequence
    [SerializeField] SceneOutroController outroController; // Assign only on the final item
    [SerializeField] SequenceManager sequenceManager; // Drag in the SequenceManager from scene

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

            // Stop phone ringing if applicable
            if (phoneRinger != null)
            {
                phoneRinger.StopRinging();
            }

            StartCoroutine(DialogManagerA.Instance.ShowDialog(correctDialog));

            // If this is the final item, prepare for fade/transition AFTER dialog ends
            if (isFinalItem && outroController != null)
            {
                outroController.PrepareForSceneEnd();
            }
        }
        else
        {
            StartCoroutine(DialogManagerA.Instance.ShowDialog(fallbackDialog));
        }
    }
}
