using UnityEngine;

public class AnchoringPosSizeUI : AspectRatioDetector
{
    [SerializeField] private RectTransform _rectTr;

    [SerializeField] private Vector2 _tabletPos = Vector2.zero; // pos to be in tablet
    [SerializeField] private Vector2 _tabletSize = Vector2.zero; // size to be in tablet
    [SerializeField] private Vector2 _newPhonePos = Vector2.zero; // pos to be in new phone
    [SerializeField] private Vector2 _newPhoneSize = Vector2.zero; // size to be in new phone

    private Vector2 _originalPos = Vector2.zero;
    private Vector2 _originalSize = Vector2.zero;

    private void Awake()
    {
        _originalPos = _rectTr.anchoredPosition;
        _originalSize = _rectTr.sizeDelta;
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        ChangeRectByAspectRatio();
    }

    private void ChangeRectByAspectRatio()
    {
        switch (_currentAspectRatio)
        {
            case DeviceType.OldPhone:
                _rectTr.anchoredPosition = _originalPos;
                _rectTr.sizeDelta = _originalSize;
                break;
            case DeviceType.NewPhone:
                _rectTr.anchoredPosition = _newPhonePos;
                _rectTr.sizeDelta = _newPhoneSize;
                break;
            case DeviceType.Tablet:
                _rectTr.anchoredPosition = _tabletPos;
                _rectTr.sizeDelta = _tabletSize;
                break;
            default:
                _rectTr.anchoredPosition = _originalPos;
                _rectTr.sizeDelta = _originalSize;
                break;
        }
    }
}