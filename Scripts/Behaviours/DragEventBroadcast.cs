using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragEventBroadcast : MonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    public event Action<DragEventBroadcast,PointerEventData> onBeginDrag;
    public event Action<DragEventBroadcast,PointerEventData> onEndDrag;
    public event Action<DragEventBroadcast,PointerEventData> onDrag; 

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(this,eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(this,eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(this,eventData);
    }
}