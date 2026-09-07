using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlUI: MonoBehaviour
{
    [Header("Tiles Menu")]
    public InputActionReference tilesMenuAction;
    public GameObject tilesMenu;
    public GridSystem gridSystem;

    private void OnEnable()
    {
        tilesMenuAction.action.started += OpenTilesMenu;
    }

    private void OpenTilesMenu(InputAction.CallbackContext context)
    {
        if(!tilesMenu.activeSelf)
        {
            tilesMenu.SetActive(true);
            Cursor.visible = true;

            gridSystem.DeactivatePlacement();
        }
        else
        {
            tilesMenu.SetActive(false);
            Cursor.visible = false;

            gridSystem.ActivatePlacement();
        }
    }
}
