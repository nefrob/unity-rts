using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{   
    public SpriteRenderer select_sprite;

    protected virtual void Awake()
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

    public bool IsSelected()
    {
        return select_sprite.enabled;
    }
}
