using UnityEngine;

public class Bug : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // add data
            Debug.Log("Collected!");

            //deactivate bug
            gameObject.SetActive(false);
        }
    }
}
