using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private static readonly int WALK_PROPERTY = Animator.StringToHash("Walk");


    public RectTransform pad;
    public RectTransform stick;

    public Transform player;
    private Vector3 input;
    public float playerSpeed;
    public Animator anim = null;
    public SpriteRenderer SR = null;

  
    public void OnDrag(PointerEventData eventData)
    {
        stick.position = eventData.position;
        stick.localPosition =
            Vector2.ClampMagnitude(eventData.position -
                                   (Vector2)pad.position, pad.rect.width * 0.5f);
        input = new Vector3(stick.localPosition.x, 0, stick.localPosition.y).normalized;
        //if(stick.localPosition.x == 1)
        //{
        //    SR.flipX = false;
        //}
        //else if(stick.localPosition.y == -1)
        //{
        //    SR.flipY = true;
        //}
        //anim.SetBool(WALK_PROPERTY, Mathf.Abs(input.sqrMagnitude) > Mathf.Epsilon);
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
            yield return null;
        }
    }
}

