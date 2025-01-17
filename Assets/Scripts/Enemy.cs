using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Enemy : MovingObject
{
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LoseHp(playerDamage);
        GameManager.instance.GameOver();
    }
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        /*if(target == null)
        {
            Debug.LogError("Target is not assigned.");
            return;
        }*/
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player> (xDir, yDir);
        
        /*
        // Calcula la dirección hacia el objetivo (jugador)
        Vector2 direction = (target.position - transform.position).normalized;

        // Mueve en el eje X o Y prioritariamente
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction = new Vector2(Mathf.Sign(direction.x), 0);
        }
        else
        {
            direction = new Vector2(0, Mathf.Sign(direction.y));
        }

        // Calcula la nueva posición
        Vector2 targetPosition = (Vector2)transform.position + direction;

        // Verifica si el enemigo está al lado del jugador
        if (Vector2.Distance(targetPosition, target.position) < 1f)
        {
            Debug.Log("Enemy attacks the player!");
            AttackPlayer();
            return;
        }

        // Realiza el movimiento suave hacia la posición objetivo
        StartCoroutine(SmoothMovement(targetPosition));*/
    }

    private void AttackPlayer()
    {
        // Finaliza el juego si el enemigo toca al jugador
        Debug.Log("Player has been touched by an enemy! Game Over.");

        // Llama al método para finalizar el juego
        GameManager.instance.GameOver();
    }



    private IEnumerator SmoothMovement(Vector2 targetPosition)
    {
        // Set a speed for smooth movement
        float speed = 3f;
        float distance;

        do
        {
            // Calculate the distance remaining to the target position
            distance = (targetPosition - (Vector2)transform.position).sqrMagnitude;

            // Move closer to the target position over time
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Yield until the next frame
            yield return null;
        }
        while (distance > float.Epsilon); // Stop moving once close enough

        // Snap to the exact target position to avoid floating-point errors
        transform.position = targetPosition;
    }

}