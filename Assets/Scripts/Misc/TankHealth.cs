using UnityEngine;

public class TankHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // Referencia al controlador de IA del tanque enemigo
    private EnemyTankController enemyController;

    void Start()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyTankController>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " recibió daño: " + amount + ". Salud actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        if (enemyController != null)
        {
            enemyController.SetState(EnemyTankController.State.Dead);
        }
        // Aquí puedes agregar efectos adicionales como sonidos o partículas
        Destroy(gameObject, 2.0f); // Destruir el tanque después de 2 segundos para ver el efecto de explosión
    }
}
