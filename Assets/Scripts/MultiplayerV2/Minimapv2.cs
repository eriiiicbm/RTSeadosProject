using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimapv2 : MonoBehaviour,IPointerDownHandler,IDragHandler
{
   [SerializeField] private RectTransform minimapRect;
   [SerializeField] private float mapScale = 20f;
   [SerializeField] private float offset = -6f;
   private Transform playerCameraTransform;

   private void Update()
   {
      if (playerCameraTransform!=null)
      {return; }

      if (NetworkClient.connection==null)
      {
         return;
      }
      if (NetworkClient.connection.identity==null)
      {
         return;
      }

      playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>().GetCameraTransform();
   }

   private void MoveCamera()
   {
      Vector2 mousePos = Mouse.current.position.ReadValue();
      if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect,mousePos,null,out Vector2 localPoint))
      {
         return;
      }

      Vector2 lerp = new Vector2((localPoint.x-minimapRect.rect.x)/minimapRect.rect.width,(localPoint.y-minimapRect.rect.y)/minimapRect.rect.height);
  
      Vector3 newCameraPos = new Vector3(Mathf.Lerp(-mapScale,mapScale,lerp.x),playerCameraTransform.position.y,Mathf.Lerp(-mapScale,mapScale,lerp.y));
      playerCameraTransform.position = newCameraPos-new Vector3(0,0,offset);
   }

   public void OnPointerDown(PointerEventData eventData)
   {
MoveCamera();   }

   public void OnDrag(PointerEventData eventData)
   {
MoveCamera();   }
}
