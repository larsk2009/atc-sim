using System.Collections;
using System.Collections.Generic;
using Assets.scripts.Airplane;
using UnityEngine;

public class RadarManager : MonoBehaviour
{
    public const float RadarUpdateTime = 1f;   // in Seconds
    private float TimeSinceStart = 0;

    private static RadarManager instance = null;

    private const int standardRange = 40; //Standard range of the radar screen in miles
    private LineRenderer lineRenderer;

    public RadarRing Circle;
    public static float WorldPointPerNauticalMile
    {
        get;
        private set;
    }

    void Start()
    {
        instance = this;
        lineRenderer = GetComponent<LineRenderer>();

        int width = Screen.width;
        int height = Screen.height;
        Debug.Log("Screen Width: " + width);
        Debug.Log("Screen Height: " + height);

        //Determine how much miles one pixel needs to be to fit 40 miles (by default) on the screen.
        WorldPointPerNauticalMile = Camera.main.ScreenToWorldPoint(new Vector2(width, height)).y / (float)standardRange;

        DrawRadarUi();
    }

    private void DrawRadarUi()
    {
        //Draw radar rings
        var clone = Instantiate(Circle);
        clone.DrawCircle(10 * WorldPointPerNauticalMile);
        clone = Instantiate(Circle);
        clone.DrawCircle(20 * WorldPointPerNauticalMile);
        clone = Instantiate(Circle);
        clone.DrawCircle(30 * WorldPointPerNauticalMile);
        clone = Instantiate(Circle);
        clone.DrawCircle(40 * WorldPointPerNauticalMile);
    }

    void Awake()
    {
        if (instance == null)
        {     // initialize only one instance.
            instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
        else
        {                // Destroy unused instances.
            GameObject.Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceStart = TimeSinceStart + Time.deltaTime;
        if (TimeSinceStart > RadarUpdateTime)
        {
            TimeSinceStart = 0;
            UpdateRadarScreen();
        }
    }

    void UpdateRadarScreen()
    {
        var planes = GameObject.FindObjectsOfType(typeof(Airplane));

        foreach (var o in planes)
        {
            var plane = (Airplane)o;
            plane.UpdateLocation();
        }
    }
}
