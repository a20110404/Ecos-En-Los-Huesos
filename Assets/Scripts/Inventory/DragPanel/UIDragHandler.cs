using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform panelToDrag;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector2 minPosition;
    private Vector2 maxPosition;

    private void Awake()
    {
        if (panelToDrag == null)
            panelToDrag = transform.parent.GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();
        CalculateScreenBounds();
    }

    private void CalculateScreenBounds()
    {
        if (canvas == null || panelToDrag == null) return;

        // Calcular límites de la pantalla
        Vector2 panelSize = panelToDrag.rect.size;
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().rect.size;

        // Calcular límites mínimos/máximos (centro del canvas como referencia)
        minPosition = new Vector2(
            (-canvasSize.x / 2) + (panelSize.x / 2),
            (-canvasSize.y / 2) + (panelSize.y / 2)
        );

        maxPosition = new Vector2(
            (canvasSize.x / 2) - (panelSize.x / 2),
            (canvasSize.y / 2) - (panelSize.y / 2)
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originalPanelLocalPosition = panelToDrag.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out originalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (panelToDrag == null || canvas == null)
            return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
            Vector3 newPosition = originalPanelLocalPosition + offsetToOriginal;

            // Aplicar restricciones de pantalla
            newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

            panelToDrag.localPosition = newPosition;
        }
    }

    // Actualizar límites si cambia el tamaño de la pantalla
    private void OnRectTransformDimensionsChange()
    {
        CalculateScreenBounds();
    }
}