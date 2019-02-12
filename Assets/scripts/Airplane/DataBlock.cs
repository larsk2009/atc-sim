using System;
using UnityEngine;

namespace Assets.scripts.Airplane
{
    [RequireComponent(typeof(TextMesh))]
    public class DataBlock : MonoBehaviour
    {
        private Airplane _plane;
        private Side _side;
        private TextMesh _textField;

        private int _count;

        private enum Side
        {
            Primary = 0,
            Secondary = 1
        }

        public bool UpdateDataBlock { get; set; } = true;

        // Start is called before the first frame update
        void Start()
        {
            _plane = transform.parent.gameObject.GetComponent<Airplane>();
            _side = Side.Primary;
            _textField = GetComponent<TextMesh>();
            _count = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (UpdateDataBlock)
            {
                _count++;
                UpdateDataBlock = false;
                if (_count > 3)
                {
                    if (_side == Side.Primary)
                    {
                        string altitude = (_plane.Altitude / 100).ToString();
                        altitude = altitude.PadLeft(3, '0');
                        _textField.text = _plane.Callsign + "\n" + altitude + " " + _plane.Speed;
                        _side = Side.Secondary;
                    }
                    else
                    {
                        _textField.text = _plane.Callsign + "\n" + _plane.Scratchpad + " " + _plane.Type;
                        _side = Side.Primary;
                    }

                    _count = 0;
                }
            }
        }
    }
}
