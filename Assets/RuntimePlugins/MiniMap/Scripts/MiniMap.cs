using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 鼠标交互使用，移动端应该没有 光标 Enter/Exit 的概念
/// </summary>
public class MiniMap : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Camera cam;
    [SerializeField] private RawImage  rawImage;
    
    //[SerializeField] Sprite preventInteractiveCursor;//不可监控光标，点击时，发现光标下没有可交互 Collider 时展示该光标 0.5S                                                                                                                                                       
    [Serializable] public class MiniMapEvent : UnityEvent<Vector3> { }
    [SerializeField ]private int MINIMAP_LAYER=25;
    [SerializeField] Text text;
    [SerializeField]float duration = 0.8f;
    public MiniMapEvent OnMiniMapClicked = new MiniMapEvent();
    /// <summary>
    /// 点击 MiniMap
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        var pos = GetNormalizedPointerPosition(eventData);
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(pos);
        Debug.DrawRay(ray.origin, ray.direction*50, Color.red,5);
        if (Physics.Raycast(ray, out hit, cam.farClipPlane, 1 << MINIMAP_LAYER, QueryTriggerInteraction.Collide))
        {
            GameObject hitObject = hit.collider.transform.gameObject;
            text.text =$"{ hit.point:x: y: z}";
            Debug.Log($"{nameof(MiniMap)}: 打到了 {hit.collider.name}  -  位置 {hit.point:x:y:z}！");
            OnMiniMapClicked.Invoke(hit.point);
        }
        else
        {
            Debug.Log($"{nameof(MiniMap)}: 该区域不支持寻路！");
            text.text = "该区域不支持寻路！";
        }
    }
    float counted=0;
    private void Update()
    {
        if (!string.IsNullOrEmpty(text.text))
        {
            if (counted<duration)
            {
                counted += Time.deltaTime;
            }
            else
            {
                counted = 0;
                text.text = string.Empty;
            }
        }
    }
    private Vector3 GetNormalizedPointerPosition(PointerEventData eventData)
    {
        Vector2 localPos;
        Vector2 size = rawImage.rectTransform.rect.size;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, eventData.position, null, out localPos);
        return new Vector3(1f + localPos.x / size.x, 1f + localPos.y / size.y, 0f);
    }

}
