using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickMarker : MonoBehaviour
{
    // [SerializeField] private SpriteRenderer arrowSprite;
    [SerializeField] private Image arrow;
    [SerializeField] private float displayTime = 1.0f;
    private float remainingDiplayTime;

    void Awake()
    {
        remainingDiplayTime = 0.0f;
    }

    void Update()
    {
        remainingDiplayTime -= Time.deltaTime;
        if (remainingDiplayTime <= 0)
        {
            // arrowSprite.enabled = false;
            arrow.enabled = false;
        }

        // Make arrow face the camera
        transform.eulerAngles = Camera.main.transform.eulerAngles;

        if (Input.GetMouseButtonDown(1))
        {
            // FIXME: add support for other sprite type, loop in with an action manager and selection manager
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 pos = hit.point;
                pos.y = transform.position.y;
                transform.position = pos;

                remainingDiplayTime = displayTime;
                // arrowSprite.enabled = true;
                arrow.enabled = true;

                // fixme tween bounce?
                // fixme 
            }

        }
    }
}
