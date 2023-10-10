using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    public Animator animatior;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {

        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // get mouse location
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // check the mouse's location
        bool isMouseOnLeft = mousePosition.x < transform.position.x;



        // Flip the character
        if (isMouseOnLeft)
        {
            FlipCharacter(-1f);
        }
        else
        {
            FlipCharacter(1f);
        }

        //Move the character
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized * moveSpeed;
        transform.position += movement * Time.deltaTime;
        animatior.SetFloat("speed", movement.magnitude);

    }

    private void FlipCharacter(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = direction;
        transform.localScale = scale;
    }


}
