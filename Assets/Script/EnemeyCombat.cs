using UnityEngine;

public class EnemeyCombat : MonoBehaviour
{
    public int damage = 1;
    public Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnCollisionEnter2D(Collision2D collision)
    { if (collision.gameObject.tag == "Player")
        {
        collision.gameObject.GetComponent<PlayerHealth>().changeHealth(-damage);
        
        }
        
    }
}
