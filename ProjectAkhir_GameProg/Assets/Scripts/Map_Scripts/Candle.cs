using UnityEngine;

public class Candle : MonoBehaviour
{
    [Header("Animator Settings")]
    public Animator animator;      // Assign the Animator on your candle prefab
    public string litBoolName = "IsLit"; // Boolean parameter name in Animator

    public void SetLit(bool lit)
    {
        if (animator != null)
        {
            animator.SetBool(litBoolName, lit);
        }
    }
}
