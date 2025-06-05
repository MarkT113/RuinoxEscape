using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    public Transform[] pathPoints;
    public float speed = 2f;
    
    private int currentPointIndex = 0;

    private void Update()
    {
        if (pathPoints.Length == 0) return; // Check / handle empty array

        // Get position of upcoming point / the point to move towards
        Transform targetPoint = pathPoints[currentPointIndex];
        
        /* Lerp() also works but MoveTowards() ensures the maxDistanceDelta is not passed/exceeded
        while lerp moves/interpolates BY t. */
        /* Function: move 'transform.position' (i.e. the platform's Transform / world position) 'speed * Time.deltaTime'
         distance towards target point's position (starts at 0, then 1, 2, 3, .... 
         depending on the length of the array / number of points in the path). Thus, essentially, at each frame update, 
         the platform's new position becomes the 'starting/current point' and it moves towards targetPoint at a max. 
         distance change of 'speed * Time.deltaTime'. */
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        /* Once the distance becomes too small (< 0.1f), the next Transform in pathPoints (using the modulus operator
         ensures that if the last index in pathPoints [i.e. currentPointIndex == pathPoints.Length - 1] is reached, 
         then it will loop / start over at 0) is set as the new target point; meaning the platform will now start 
         moving towards it. This if statement only sets the currentPointIndex, in preparation for the upcoming 
         'Update()' call to set targetPoint.*/
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % pathPoints.Length;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}