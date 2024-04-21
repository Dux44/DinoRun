using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoMovement : MonoBehaviour
{
    [SerializeField] private Animator dinoAnimator;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip dieSFX;

    private bool isGameStarted = false;
    private bool isTochingGround = false;

    private bool isDead = false;

    // Update is called once per frame
    void Update()
    {
        bool isJumpButtonPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
        bool isDodgeButtonPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.DownArrow);

        if (!isDead)
        {
            if (isJumpButtonPressed)
            {
                if (isGameStarted == true && isTochingGround == true)
                {
                    //jump
                    Jump();
                }
                else
                {
                    isGameStarted = true;
                    //start game
                    GameMananger.Instance.gameStarted = true;
                }
            }
            else if (isDodgeButtonPressed && isTochingGround)
            {

            }
        }

        dinoAnimator.SetBool("Started Game",isGameStarted);
        dinoAnimator.SetBool("IsCrouching", isDodgeButtonPressed && isTochingGround && !isJumpButtonPressed);
        dinoAnimator.SetBool("IsDead", isDead);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isTochingGround = true;
        }
        else if (collision.gameObject.tag == "Obstacle")
        {
            isDead = true;
            GameMananger.Instance.gameEnded = true;
            GameMananger.Instance.ShowGameEndScreen();

            audioSource.clip =dieSFX;
            audioSource.Play();
        }

    }
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce);
        isTochingGround = false;

        audioSource.clip = jumpSFX;
        audioSource.Play();
    }
}
