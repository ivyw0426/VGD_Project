using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BugManager : MonoBehaviour
{   
    public int bugCount;
    public TextMeshProUGUI bugText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bugText.text = "Bug Collected: " + bugCount.ToString();
    }
}
