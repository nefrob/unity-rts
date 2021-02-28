using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSprite : MonoBehaviour
{
    // Sprite image to toggle
    public SpriteRenderer select_sprite;

    void Awake()
    {
        select_sprite.enabled = false;
    }

    public void SelectObject()
    {
        select_sprite.enabled = true;
    }

    public void DeselectObject()
    {
        select_sprite.enabled = false;
    }
}
