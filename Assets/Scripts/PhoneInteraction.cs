using UnityEngine;

public class PhoneInteraction : MonoBehaviour
{
    public PhoneRingController phoneRingController; // Assign in Inspector

    public void Interact()
    {
        if (phoneRingController != null)
        {
            phoneRingController.StopRinging();
            Debug.Log("Phone answered!");
        }
    }
}
