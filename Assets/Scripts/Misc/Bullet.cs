using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    // Explosion Effect
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private float speed = 600.0f;
    [SerializeField]
    private float lifeTime = 3.0f;
   
    public int damage = 50;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += 
            transform.forward * speed * Time.deltaTime;       
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Instantiate(explosion, contact.point, Quaternion.identity);
        
        // Verificar si el objeto colisionado tiene el script TankHealth
        TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
        if (tankHealth != null)
        {
            tankHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
