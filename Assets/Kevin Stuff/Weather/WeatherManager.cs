using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    //public WeatherState CurrentWeather
    //{
    //    get => m_CurrentState;
    //    set
    //    {
    //        switch (value)
    //        {
    //            case WeatherState.Snow:
    //                OnGrounded();
    //                m_UpdateHandler = GroundedBehavior;
    //                break;
    //            case WeatherState.Sandstorm:
    //                OnMidair();
    //                m_UpdateHandler = MidairBehavior;
    //                break;
    //            default:
    //                break;
    //        }

    //        m_CurrentState = value;
    //    }
    //}
    //private WeatherState m_CurrentState;

    private delegate void UpdateHandler();
    private UpdateHandler m_UpdateHandler;

    public enum WeatherState
    {
        Snow,
        Sandstorm
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}