using UnityEngine;

public class AnchoringStrechUI : AspectRatioDetector
{
    [SerializeField] private RectTransform _rectTr;

    [SerializeField] private Vector2 _tabletLeftTop = Vector2.zero;
    [SerializeField] private Vector2 _tabletRightBottom = Vector2.zero;
    [SerializeField] private Vector2 _newPhoneLeftTop = Vector2.zero;
    [SerializeField] private Vector2 _newPhoneRightBottom = Vector2.zero;

    private Vector2 _originalLeftTop = Vector2.zero;
    private Vector2 _originalRightBottom = Vector2.zero;

    private void Awake()
    {
        _originalLeftTop = new(_rectTr.offsetMin.x, -_rectTr.offsetMax.y);
        _originalRightBottom = new(-_rectTr.offsetMax.x, _rectTr.offsetMin.y);
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
                _rectTr.offsetMin = new Vector2(_originalLeftTop[0], _originalRightBottom[1]);
                _rectTr.offsetMax = new Vector2(-_originalRightBottom[0], -_originalLeftTop[1]);
                break;
            case DeviceType.NewPhone:
                _rectTr.offsetMin = new Vector2(_newPhoneLeftTop[0], _newPhoneRightBottom[1]); // left & bottom
                _rectTr.offsetMax = new Vector2(-_newPhoneRightBottom[0], -_newPhoneLeftTop[1]); // -top & -right
                break;
            case DeviceType.Tablet:
                _rectTr.offsetMin = new Vector2(_tabletLeftTop[0], _tabletRightBottom[1]); // left & bottom
                _rectTr.offsetMax = new Vector2(-_tabletRightBottom[0], -_tabletLeftTop[1]); // -top & -right
                break;
            default:
                _rectTr.offsetMin = new Vector2(_originalLeftTop[0], _originalRightBottom[1]);
                _rectTr.offsetMax = new Vector2(-_originalRightBottom[0], -_originalLeftTop[1]);
                break;
        }
    }
}