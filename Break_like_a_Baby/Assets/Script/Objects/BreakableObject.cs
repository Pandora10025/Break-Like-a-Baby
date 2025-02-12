using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private int health = 10;
    void Start()
    {
        
    }

    void Update()
    {
       
    }

    public void takeDamage()
    {
        health--;
        Debug.Log(health);

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
