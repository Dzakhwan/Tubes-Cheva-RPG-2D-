using UnityEngine;

public class EnemeyCombat : MonoBehaviour
{
    public int damage = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<PlayerHealth>().changeHealth(-damage);
    }
}
