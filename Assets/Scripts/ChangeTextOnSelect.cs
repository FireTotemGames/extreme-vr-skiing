using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeTextOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */
    [SerializeField] private Color textActive;
    [SerializeField] private Color textIdle;
    [SerializeField] private TextMeshProUGUI[] labels;
    [SerializeField] private Image[] images;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void OnEnable()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            return;
        }
        
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].color = textIdle;
        }
        
        // for (int i = 0; i < images.Length; i++)
        // {
        //     images[i].color = textIdle;
        // }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ActiveColor();
        }
        else
        {
            InActiveColor();
        }
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void ActiveColor()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].color = textActive;
        }
        
        // for (int i = 0; i < images.Length; i++)
        // {
        //     images[i].color = textActive;
        // }
    }
    
    private void InActiveColor()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].color = textIdle;
        }
        
        // for (int i = 0; i < images.Length; i++)
        // {
        //     images[i].color = textIdle;
        // }
    }
    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

    public void OnSelect(BaseEventData eventData)
    {
        ActiveColor();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        InActiveColor();
    }
}