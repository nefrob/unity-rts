using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{   
    public SpriteRenderer selection_sprite;

    protected virtual void Awake()
    {
        selection_sprite.enabled = false;
    }

    public void SelectObject()
    {
        selection_sprite.enabled = true;
    }

    public void DeselectObject()
    {
        selection_sprite.enabled = false;
    }

    protected bool IsSelected()
    {
        return selection_sprite.enabled;
    }
}
