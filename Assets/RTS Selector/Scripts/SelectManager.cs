using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SelectManager : MonoBehaviour
{
    [Tooltip("The camera used for highlighting")]
    public Camera selectCam;
    [Tooltip("The rectangle to modify for selection")]
    public RectTransform SelectingBoxRect;

    private Rect SelectingRect;
    private Vector3 SelectingStart;

    private bool selecting;
    private bool reSelecting;

    public SelectableCharacter[] selectableChars;
    private List<SelectableCharacter> selectedArmy = new List<SelectableCharacter>();

    private void Awake() {
        //This assumes that the manager is placed on the image used to select
        if (!SelectingBoxRect) {
            SelectingBoxRect = GetComponent<RectTransform>();
        }

        //Searches for all of the objects with the selectable character script
        selectableChars = FindObjectsOfType<SelectableCharacter>();
    }

    void Update() {
        if (SelectingBoxRect == null) {
            Debug.LogError("There is no Rect Transform to use for selection!");
            return;
        }

        //The input for mouse down. This can be changed
        if (Input.GetMouseButtonDown(0) && SelectingBoxRect) {
            ReSelect();
            SelectingStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            SelectingBoxRect.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            reSelecting = true;
        } else if (Input.GetMouseButtonUp(0)) {
            reSelecting = false;
        }

        selecting = Input.GetMouseButton(0);

        if (selecting)
            SelectingArmy();
        else
            SelectingBoxRect.sizeDelta = new Vector2(0, 0);
    }

    //Resets what is currently being selected
    void ReSelect() {
        for (int i = 0; i <= (selectedArmy.Count - 1); i++) {
            selectedArmy[i].TurnOffSelector();
            selectedArmy.Remove(selectedArmy[i]);
        }

        reSelecting = false;
    }

    //Does the calculation for mouse dragging on screen
    void SelectingArmy() {
        if (-(SelectingStart.x - Input.mousePosition.x) > 0) {
            if (SelectingStart.y - Input.mousePosition.y > 0) {
                if (SelectingBoxRect.pivot != new Vector2(0, 1))
                    SelectingBoxRect.pivot = new Vector2(0, 1);

                SelectingBoxRect.sizeDelta = new Vector3(-(SelectingStart.x - Input.mousePosition.x), SelectingStart.y - Input.mousePosition.y, 0);
                SelectingRect = new Rect(SelectingStart.x, SelectingStart.y - SelectingBoxRect.sizeDelta.y, SelectingBoxRect.sizeDelta.x, SelectingBoxRect.sizeDelta.y);
            } else if (SelectingStart.y - Input.mousePosition.y < 0) {
                if (SelectingBoxRect.pivot != new Vector2(0, 0))
                    SelectingBoxRect.pivot = new Vector2(0, 0);

                SelectingBoxRect.sizeDelta = new Vector3(-(SelectingStart.x - Input.mousePosition.x), -(SelectingStart.y - Input.mousePosition.y), 0);
                SelectingRect = new Rect(SelectingStart.x, SelectingStart.y, SelectingBoxRect.sizeDelta.x, SelectingBoxRect.sizeDelta.y);
            }
        } else if (-(SelectingStart.x - Input.mousePosition.x) < 0)
            if (SelectingStart.y - Input.mousePosition.y > 0) {
                if (SelectingBoxRect.pivot != new Vector2(1, 1))
                    SelectingBoxRect.pivot = new Vector2(1, 1);

                SelectingBoxRect.sizeDelta = new Vector3((SelectingStart.x - Input.mousePosition.x), SelectingStart.y - Input.mousePosition.y, 0);
                SelectingRect = new Rect(SelectingStart.x - SelectingBoxRect.sizeDelta.x, SelectingStart.y - SelectingBoxRect.sizeDelta.y, SelectingBoxRect.sizeDelta.x, SelectingBoxRect.sizeDelta.y);
            } else if (SelectingStart.y - Input.mousePosition.y < 0) {
                if (SelectingBoxRect.pivot != new Vector2(1, 0))
                    SelectingBoxRect.pivot = new Vector2(1, 0);

                SelectingBoxRect.sizeDelta = new Vector3((SelectingStart.x - Input.mousePosition.x), -(SelectingStart.y - Input.mousePosition.y), 0);
                SelectingRect = new Rect(SelectingStart.x - SelectingBoxRect.sizeDelta.x, SelectingStart.y, SelectingBoxRect.sizeDelta.x, SelectingBoxRect.sizeDelta.y);
            }

        CheckForSelectedCharacters();
    }

    //Checks if the correct characters can be selected and then "selects" them
    void CheckForSelectedCharacters() {
        foreach (SelectableCharacter soldier in selectableChars) {
            Vector2 screenPos = selectCam.WorldToScreenPoint(soldier.transform.position);
            if (SelectingRect.Contains(screenPos)) {
                if (!selectedArmy.Contains(soldier))
                    selectedArmy.Add(soldier);

                soldier.TurnOnSelector();
            } else if (!SelectingRect.Contains(screenPos)) {
                soldier.TurnOffSelector();

                if (selectedArmy.Contains(soldier))
                    selectedArmy.Remove(soldier);
            }
        }
    }
}
