using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCountdown : MonoBehaviour
{
    [SerializeField] private float m_deathCountdown = 5f;

    private void Start()
    {
        StartCoroutine(DeathCountdownRoutine());
    }

    private IEnumerator DeathCountdownRoutine()
    {
        yield return new WaitForSeconds(m_deathCountdown);
        Destroy(gameObject);
    }
}
