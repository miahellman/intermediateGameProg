using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectsWrapping : MonoBehaviour
{ 
  private void Update()
{
    //making player + objects wrap so they stay in frame
    //convert world point to camera viewport 
    Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
    Vector3 moveAdjustment = Vector3.zero;


    if (viewportPosition.x < 0)
    {
        moveAdjustment.x += 1;
    }
    else if (viewportPosition.x > 1)
    {
        moveAdjustment.x -= 1;
    }
    else if (viewportPosition.y < 0)
    {
        moveAdjustment.y += 1;
    }
    else if (viewportPosition.y > 1)
    {
        moveAdjustment.y -= 1;
    }

    //convert back to normal pos w/ new value
    transform.position = Camera.main.ViewportToWorldPoint(viewportPosition + moveAdjustment);
}
}
