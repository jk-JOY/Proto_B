using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{ 
    public RectTransform pad;
    public RectTransform stick;

    public Transform player;
    private Vector3 input;
    public float playerSpeed;

    public void OnDrag(PointerEventData eventData)
    {
        stick.position = eventData.position;
        stick.localPosition =
            Vector2.ClampMagnitude(eventData.position -
                                   (Vector2)pad.position, pad.rect.width * 0.5f);
        input = new Vector3(stick.localPosition.x, 0, stick.localPosition.y).normalized;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pad.position = eventData.position;
        pad.gameObject.SetActive(true);
        StartCoroutine("Movement");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stick.localPosition = Vector2.zero;
        pad.gameObject.SetActive(false);
        StopCoroutine("Movement");
        input = Vector3.zero;
    }

    IEnumerator Movement()
    {
        while (true)
        {
            player.Translate(input * playerSpeed * Time.deltaTime, Space.World);
            //if(input != Vector3.zero)
            //    player.rotation = Quaternion.Slerp(player.rotation,Quaternion.LookRotation(input), 5* Time.deltaTime);
            yield return null;
        }
    }
}

