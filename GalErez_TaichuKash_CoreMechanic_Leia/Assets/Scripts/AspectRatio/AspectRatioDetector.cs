using UnityEngine;

public enum DeviceType
{
    OldPhone, // 9:16 - from 0.51 to 0.6
    NewPhone, // 9:19.5 - from 0.45 to 0.5
    Tablet, // 3:4 - bigger than 0.6
}

public class AspectRatioDetector : MonoBehaviour
{
    /* Aspect Ratio Values Explained
     * new phone ratio varies from 0.45 to 0.5 |(19.5:9)|
     * old phone ratio varies from 0.51 to 0.6 |(16:9)|
     * tablet ratio is bigger than 0.6 |(4:3)|
     */

    [SerializeField] protected DeviceType _currentAspectRatio = DeviceType.OldPhone;
    public DeviceType CurrentAspectRatio => _currentAspectRatio;

    private const float _aspectRatioThresholdNewPhoneTablet = 0.6f;
    private const float _aspectRatioThresholdOldPhoneNewPhone = 0.5f;

    protected virtual void OnEnable()
    {
        _currentAspectRatio = DetectAspectRatio();
    }
    protected virtual void Start()
    {
        _currentAspectRatio = DetectAspectRatio();
    }

    public static DeviceType DetectAspectRatio()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        Debugger.Log(aspectRatio);

        if (aspectRatio < _aspectRatioThresholdOldPhoneNewPhone)
        {
            // Debugger.Log("DeviceType: NewPhone " + aspectRatio);
            return DeviceType.NewPhone;
        }
        else if (aspectRatio > _aspectRatioThresholdNewPhoneTablet)
        {
            // Debugger.Log("DeviceType: Tablet " + aspectRatio);
            return DeviceType.Tablet;
        }
        else
        {
            // Debugger.Log("DeviceType: OldPhone " + aspectRatio);
            return DeviceType.OldPhone;
        }
    }
}