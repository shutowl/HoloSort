using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour
{
    private Talent talent;

    private void Awake()
    {
        talent = GetComponent<Talent>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        talent.ChangeState(2);  //grabbed
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        talent.ChangeState(1);  //let go
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
