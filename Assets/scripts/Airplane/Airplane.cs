using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Airplane
{
    [RequireComponent(typeof(LineRenderer))]
    public class Airplane : MonoBehaviour
    {
        private bool _updateLocation;
        private float _speed; //In knots
        private int _heading; //Heading in degrees as specified by ATC
        private float _currentHeading; //Actual heading of the airplane
        private int _altitude;

        //private const double MilesPerNm = 1.15077945;
        private const double DeltaTime = RadarManager.RadarUpdateTime / 3600;

        private double _ptlLength; //Times in mintues
        private LineRenderer _ptlLine;

        public HistoryBlip blip;
        private List<HistoryBlip> _history = new List<HistoryBlip>(5);

        enum TurnDirection
        {
            Left,
            Right
        }

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

        public int Altitude
        {
            get => _altitude;
            set
            {
                if (value >= 0)
                {
                    _altitude = value;
                }
            }
        }

        public string Callsign { get; private set; }

        public string Scratchpad { get; private set; }

        public string Type { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            _ptlLine = GetComponent<LineRenderer>();
            _updateLocation = false;
            _speed = 200;
            _heading = 180;
            _currentHeading = 10;
            _altitude = 8000;
            Callsign = "ENY3555";
            Scratchpad = "V3F";
            Type = "B747";

            _ptlLength = 1;

            _ptlLine.widthMultiplier = 0.01f;
            _ptlLine.positionCount = 2;
            _ptlLine.startColor = Color.white;
            _ptlLine.endColor = Color.white;
            _ptlLine.material = new Material(Shader.Find("Sprites/Default"));
            _ptlLine.SetPosition(1, new Vector2(0, 0));
            Vector2 endPos = GetPtlEndPos();
            endPos = transform.InverseTransformVector(endPos);
            _ptlLine.SetPosition(1, endPos);
        }

        // Update is called once per frame
        void Update()
        {
            if (_updateLocation)
            {
                _updateLocation = false;
                ExecuteTurn(TurnDirection.Left);
                Vector2 start = transform.position;

                transform.position = start + GetTraveledDistance();

                //Draw predicted track line (PTL)
                Vector2 endPos = GetPtlEndPos();
                endPos = transform.InverseTransformVector(endPos);
                _ptlLine.SetPosition(1, endPos);

                //DrawHistoryBlips();

                //Update datablock
                transform.GetComponentInChildren<DataBlock>().UpdateDataBlock = true;
            }
        }

        //Turns to angle specified by atc if different from current heading
        private void ExecuteTurn(TurnDirection direction)
        {
            var diff = Mathf.Abs(_heading - _currentHeading);
            float turnRadius;
            if (_speed > 250)
            {
                turnRadius = 1.5f;
            }
            else
            {
                turnRadius = 3;
            }

            if (diff < turnRadius)
            {
                _currentHeading = _heading;
            }
            else
            {
                if (direction == TurnDirection.Left)
                {
                    _currentHeading -= turnRadius;
                    if (_currentHeading < 0)
                    {
                        _currentHeading = 360 + _currentHeading;
                    }
                }
                else
                {
                    _currentHeading += turnRadius;
                    if (_currentHeading > 360)
                    {
                        _currentHeading -= 360;
                    }
                }
            }
        }

        private void DrawHistoryBlips()
        {
            //Store planes history blips
            if (_history.Count >= 5)
            {
                Destroy(_history[0].gameObject);
                _history.Remove(_history[0]);
            }

            var clone = Instantiate(blip);
            clone.transform.position = transform.position;
            _history.Add(clone);
        }

        public void UpdateLocation()
        {
            _updateLocation = true;
        }

        Vector2 GetTraveledDistance()
        {
            float headingRad = (_currentHeading - 90) * Mathf.Deg2Rad;
            double x = Mathf.Cos(headingRad) * (_speed * RadarManager.WorldPointPerNauticalMile * DeltaTime);
            double y = Mathf.Sin(headingRad) * -(_speed * RadarManager.WorldPointPerNauticalMile * DeltaTime);

            return new Vector2((float)x, (float)y);
        }

        Vector2 GetPtlEndPos()
        {
            float headingRad = (_currentHeading - 90) * Mathf.Deg2Rad;
            double x = Mathf.Cos(headingRad) * (_speed * RadarManager.WorldPointPerNauticalMile * DeltaTime * 60 * _ptlLength);
            double y = Mathf.Sin(headingRad) * -(_speed * RadarManager.WorldPointPerNauticalMile * DeltaTime * 60 * _ptlLength);

            //x += transform.position.x;
            //y += transform.position.y;

            return new Vector2((float)x, (float)y);
        }
    }
}
