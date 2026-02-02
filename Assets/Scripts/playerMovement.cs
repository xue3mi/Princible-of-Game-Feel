using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    private Vector3 velocity;
    [SerializeField] private float speed;
    [SerializeField] private List<Transform> wallList = new List<Transform>();
    [SerializeField] private float movementIncrement = 0.01f;

    private void FixedUpdate()
    {
        velocity = Vector3.zero;//reset velocity each frame

        if (Input.GetKey(KeyCode.W)) velocity.y += 1;
        if (Input.GetKey(KeyCode.S)) velocity.y -= 1;
        if (Input.GetKey(KeyCode.D)) velocity.x += 1;
        if (Input.GetKey(KeyCode.A)) velocity.x -= 1;

        velocity = velocity.normalized;
        velocity *= speed;
        velocity *= Time.fixedDeltaTime;

        Vector3 intendedNextPosition = transform.position + velocity;

        /*if (!IsThisTransformInAWall(intendedNextPosition))
        {
            //transform.position += velocity; 
        }*/

        Vector3 velocityThisFrame = velocity; // how much velocity we use this frame
        Vector3 microVelocity; //tiny inctement of velocity we use in each while loop
        Vector3 positionNextFrame = transform.position;


        while (velocityThisFrame.magnitude > 0f)
        {
            float step = Mathf.Min(movementIncrement, velocityThisFrame.magnitude);
            microVelocity = velocityThisFrame.normalized * step;

            if (!IsThisTransformInAWall(positionNextFrame + microVelocity))
            {
                positionNextFrame += microVelocity;
                velocityThisFrame -= microVelocity;
            }
            else //if put us into wall stop
            {
                bool movedSuccessfully = false;
                if (Mathf.Abs(microVelocity.x) > 0f)
                {
                    float microXstep = Mathf.Sign(microVelocity.x) * step;
                    //microXstep = Mathf.Min(microXstep, velocity.x);
                    if (!IsThisTransformInAWall(positionNextFrame + new Vector3(microXstep, 0, 0)))
                    {
                        positionNextFrame += new Vector3(microXstep, 0, 0);
                        velocityThisFrame -= new Vector3(microXstep, 0, 0);
                        movedSuccessfully = true;
                    }
                    else
                    {
                        velocityThisFrame.x = 0;
                    }
                }

                if (!movedSuccessfully && Mathf.Abs(microVelocity.y) > 0f)
                {
                    float microYstep = Mathf.Sign(microVelocity.y) * step;
                    //microXstep = Mathf.Min(microXstep, velocity.y);
                    if (!IsThisTransformInAWall(positionNextFrame + new Vector3(microYstep, 0, 0)))
                    {
                        positionNextFrame += new Vector3(microYstep, 0, 0);
                        velocityThisFrame -= new Vector3(microYstep, 0, 0);
                        movedSuccessfully = true;
                    }
                    else 
                    {
                        velocityThisFrame.y = 0;
                    }
                }

                if (!movedSuccessfully)
                {
                    break;
                }
            }
        }
        //break will take us to this position

        transform.position = positionNextFrame;


        //check next position if in wall
        bool IsThisTransformInAWall(Vector3 positionToCheck)
        {
            foreach (Transform currentWall in wallList)
            {
                //calculate the distance between the player and the wall in X & Y
                float xDistance = Mathf.Abs(positionToCheck.x - currentWall.position.x);
                float yDistance = Mathf.Abs(positionToCheck.y - currentWall.position.y);

                //Find maximum distance in X & Y by adding half Player scale & half other player scale
                float xMaxDistance = (transform.localScale.x / 2) + (currentWall.localScale.x / 2);
                float yMaxDistance = (transform.localScale.y / 2) + (currentWall.localScale.y / 2);

                //if player is closer to wall, max distance in both x & y return true
                if (xDistance < xMaxDistance && yDistance < yMaxDistance) return true;
            }
            return false;
        }
    }
}
