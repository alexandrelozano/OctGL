using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctGL
{
    public class Camera
    {
        public const float rotationSpeed = 2.0f;
        public const float distanceSpeed = 0.1f;

        private bool dirty;

        public Vector3 camPos;
        public Vector3 camTarget;
        public Vector3 camUp;

        private double _distance;
        public double distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
                dirty = true;
            }
        }

        private double _rotationv;
        public double rotationv
        {
            get
            {
                return _rotationv;
            }
            set
            {
                _rotationv = value;

                if (_rotationv > 360)
                {
                    _rotationv -= 360;
                }
                if (_rotationv < 0)
                {
                    _rotationv += 360;
                }

                dirty = true;
            }
        }

        private double _rotationh;
        public double rotationh {
            get {
                return _rotationh;
            }
            set {
                _rotationh = value;

                if (_rotationh > 360)
                {
                    _rotationh -= 360;
                }
                if (_rotationh < 0)
                {
                    _rotationh += 360;
                }

                dirty = true;
            }
        }

        public Camera()
        {
            rotationh = 0;
            rotationv = 0;
            distance = 10;
            dirty = true;
        }

        public Matrix ViewMatrix()
        {
            if (dirty)
            {
                double radh = (rotationh * MathHelper.TwoPi) / 360.0f;
                double radv = (rotationv * MathHelper.TwoPi) / 360.0f;

                camPos = new Vector3((float)(distance * System.Math.Cos(radv) * System.Math.Cos(radh)), (float)(distance * System.Math.Cos(radv) * System.Math.Sin(radh)), (float)(distance * System.Math.Sin(radv)));
                camTarget = new Vector3((float)(-distance * System.Math.Cos(radv) * System.Math.Cos(radh)), (float)(-distance * System.Math.Cos(radv) * System.Math.Sin(radh)), (float)(-distance * System.Math.Sin(radv)));
                camUp = new Vector3((float)(-System.Math.Sin(radv) * System.Math.Cos(radh)), (float)(-System.Math.Sin(radv) * System.Math.Sin(radh)), (float)(System.Math.Cos(radv)));

                dirty = false;
            }

            return Matrix.CreateLookAt(camPos, camTarget, camUp);
        }

    }
}
