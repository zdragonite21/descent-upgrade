using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{

    public WeatherState CurrentWeather
    {
        get => m_CurrentState;
        set
        {
            switch (value)
            {
                case WeatherState.Snow:
                    OnSnow();
                    m_UpdateHandler = SnowBehavior;
                    break;
                case WeatherState.Sandstorm:
                    OnSand();
                    m_UpdateHandler = SandBehavior;
                    break;
                default:
                    break;
            }

            m_CurrentState = value;
        }
    }
    private WeatherState m_CurrentState;

    private delegate void UpdateHandler();
    private UpdateHandler m_UpdateHandler;

    public enum WeatherState
    {
        Snow,
        Sandstorm
    }


    #region Snow
    void OnSnow()
    {

    }

    void SnowBehavior()
    {

    }
    #endregion

    #region Sand
    void OnSand()
    {

    }

    void SandBehavior()
    {

    }
    #endregion


    // Update is called once per frame
    void Update()
    {
        m_UpdateHandler?.Invoke();
    }
}
