using UnityEngine;

public class End : MonoBehaviour
{   
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HIT: " + other.name);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
