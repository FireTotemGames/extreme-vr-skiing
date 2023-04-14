using System;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class VolumeSlider : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private SliderType sliderType;

    private Slider _slider;
    
    public enum SliderType
    {
        MusicVolume,
        SoundVolume,
    }

    public TMP_Text SliderLabel;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    public void OnEnable()
    {
        _slider = GetComponent<Slider>();
        switch (sliderType)
        {
            case SliderType.MusicVolume:
                _slider.SetValueWithoutNotify(MusicController.Instance.MusicVolume);
                SetVolume(MusicController.Instance.MusicVolume);
                break;
        
            case SliderType.SoundVolume:
                _slider.SetValueWithoutNotify(MusicController.Instance.SoundVolume);
                SetVolume(MusicController.Instance.SoundVolume);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void SetNormalizedVolume(float value)
    { // use this for sliders that do NOT go from 0.0 to 1.0
        float normalizedValue = value / (_slider.maxValue - _slider.minValue);
        SetVolume(normalizedValue);
    }

    public void SetVolume(float value)
    { // use this for sliders that go from 0.0 to 1.0
        _slider = _slider ? _slider : GetComponent<Slider>();
        switch (sliderType)
        {
            case SliderType.MusicVolume:
                MusicController.Instance.MusicVolume = value;
                break;
        
            case SliderType.SoundVolume:
                MusicController.Instance.SoundVolume = value;
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        float sliderValue = value * (_slider.maxValue - _slider.minValue);
        _slider.SetValueWithoutNotify(sliderValue);
        if (SliderLabel)
            SliderLabel.text = Math.Round(value * 100.0f, 0).ToString(); // Alternatively use for 0.00% format: .ToString("P", CultureInfo.InvariantCulture);
    }
    
    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */
}
