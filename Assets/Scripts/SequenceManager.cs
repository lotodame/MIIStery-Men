using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    public int currentStep = 0;

    public bool IsCorrectStep(int step)
    {
        return step == currentStep;
    }

    public void Advance()
    {
        currentStep++;
    }

    public void ResetSequence()
    {
        currentStep = 0;
    }
}
