using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Image healthBar;

    [SerializeField]
    public float maxHealth;
    public bool isPlayer = false;

    public float currentHealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    [SerializeField]
    public bool isDead = false;

    [Header("Damage feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.1f;

    

    Color originalColor;
    SpriteRenderer sr;
    Agent agent;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        agent = GetComponent<Agent>();

        currentHealth = maxHealth;
        if(healthBar == null)
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                healthBar = canvas.GetComponentInChildren<Image>();
                if(healthBar == null)
                {
                    Debug.Log("Image for health bar not found");
                }
            }
        }
        UpdateHealthBar();
    }
    public void InitializeHealth(float healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
    }

    public void GetHit(float amount, GameObject sender, 
        Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)

    {
        if (isDead)
            return;
        if (sender.layer == gameObject.layer)
            return;

        currentHealth -= amount;
        StartCoroutine(DamageFlash());

        GameManager.GenerateFloatingText(Mathf.FloorToInt(amount).ToString(), transform);

        if (knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            agent.Knockback(dir.normalized * knockbackForce,knockbackDuration);
        }

        if (currentHealth > 0)
        {
            OnHitWithReference?.Invoke(sender);
        }
        else
        {
            OnDeathWithReference?.Invoke(sender);
            isDead = true;
            StartCoroutine(KillFade());
            //Destroy(gameObject);
        }
        UpdateHealthBar();
    }

    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    IEnumerator KillFade() // не уверен надо ли оно...
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float time = 0, origAlpha = sr.color.a;

        while(time < deathFadeTime)
        {
            yield return wait; // Пауза в 1 кадр
            time += Time.deltaTime;

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1-time/deathFadeTime)*origAlpha);
        }
        Destroy(gameObject);
    }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

}
