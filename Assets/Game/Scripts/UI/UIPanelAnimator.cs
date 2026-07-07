using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelAnimator : MonoBehaviour
{
    [SerializeField] 
    private Vector2 hiddenOffset = new(0, -100);
    [SerializeField] 
    private float duration = 0.25f;
    [SerializeField] 
    private PanelDirection direction;

    private CanvasGroup cg;
    private RectTransform rect;

    private Vector2 visiblePos;
    private Vector2 hiddenPos;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();

        switch(direction)
        {
            case PanelDirection.Left:
                hiddenOffset = new Vector2(-300,0);
                break;

            case PanelDirection.Right:
                hiddenOffset = new Vector2(300,0);
                break;

            case PanelDirection.Up:
                hiddenOffset = new Vector2(0,300);
                break;

            case PanelDirection.Down:
                hiddenOffset = new Vector2(0,-300);
                break;
        }

        visiblePos = rect.anchoredPosition;
        hiddenPos = visiblePos + hiddenOffset;
    }

    public void Show()
    {
        rect.DOKill();
        cg.DOKill();

        rect.anchoredPosition = hiddenPos;
        cg.alpha = 0;

        cg.interactable = true;
        cg.blocksRaycasts = true;

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOAnchorPos(visiblePos, duration));
        seq.Join(cg.DOFade(1, duration));
        seq.SetUpdate(true);
    }

    public void Hide()
    {
        rect.DOKill();
        cg.DOKill();

        cg.interactable = false;
        cg.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOAnchorPos(hiddenPos, duration));
        seq.Join(cg.DOFade(0, duration));
        seq.SetUpdate(true);
    }

    public void HideImmediate()
    {
        rect.anchoredPosition = hiddenPos;
        cg.alpha = 0;
    }
}

public enum PanelDirection
{
    Left,
    Right,
    Up,
    Down
}