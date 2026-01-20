using Unity.VisualScripting;
using UnityEngine;

public class Antivirus : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            other.GetComponent<PlayerMovement>().hasAntivirus = true;
        }
    }

}
