using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

[RequireComponent(typeof(LineRenderer))]
public class Airplane : MonoBehaviour
{
    private bool _updateLocation;
    private float _speed; //In knots
    private int _heading; //In Degrees
    private int _ptlLength; //Times in mintues

    private const double MilesPerNm = 1.15077945;
    private const double DeltaTime = RadarManager.RadarUpdateTime / 3600;

    private LineRenderer _ptlLine;

    public int Heading
    {
        get => _heading;
        set
        {
            if (value <= 360 && value >= 0)
            {
                _heading = value;
            }
        }
    }

    public float Speed
    {
        get => _speed;
        set
        {
            if (Speed >= 0)
            {
                _speed = value;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _ptlLine = GetComponent<LineRenderer>();
        _updateLocation = false;
        _speed = 200;
        _heading = 360;
        _ptlLength = 5;

        _ptlLine.widthMultiplier = 0.01f;
        _ptlLine.positionCount = 2;
        _ptlLine.startColor = Color.white;
        _ptlLine.endColor = Color.white;
        _ptlLine.material = new Material(Shader.Find("Sprites/Default"));
        _ptlLine.SetPosition(1, GetPtlEndPos());
    }

    // Update is called once per frame
    void Update()
    {
        if (_updateLocation)
        {
            _updateLocation = false;
            Vector2 start = transform.position;

            transform.position = start + GetTraveledDistance();

            //Draw predicted track line (PTL)
            _ptlLine.SetPosition(0, new Vector2(0, 0));
            Vector2 endPos = GetPtlEndPos();
            _ptlLine.SetPosition(1, endPos);
        }
    }

    public void UpdateLocation()
    {
        _updateLocation = true;
    }

    Vector2 GetTraveledDistance()
    {
        float headingRad = (Heading - 90) * Mathf.Deg2Rad;
        double x = Mathf.Cos(headingRad) * -(_speed * MilesPerNm * RadarManager.WorldPointPerMile * DeltaTime);
        double y = Mathf.Sin(headingRad) * -(_speed * MilesPerNm * RadarManager.WorldPointPerMile * DeltaTime);

        return new Vector2((float)x, (float)y);
    }

    Vector2 GetPtlEndPos()
    {
        float headingRad = (Heading - 90) * Mathf.Deg2Rad;
        double x = Mathf.Cos(headingRad) * -(_speed * MilesPerNm * RadarManager.WorldPointPerMile * DeltaTime * 60 * _ptlLength);
        double y = Mathf.Sin(headingRad) * -(_speed * MilesPerNm * RadarManager.WorldPointPerMile * DeltaTime * 60 * _ptlLength);

        //x += transform.position.x;
        //y += transform.position.y;

        return new Vector2((float)x, (float)y);
    }
}
