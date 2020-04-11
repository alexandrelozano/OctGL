using Microsoft.Xna.Framework;

namespace OctGL
{
    public class Barycentric
    {
        public float u;
        public float v;
        public float w;
        public Barycentric(float aU, float aV, float aW)
        {
            u = aU;
            v = aV;
            w = aW;
        }
        public Barycentric(Vector2 aV1, Vector2 aV2, Vector2 aV3, Vector2 aP)
        {
            Vector2 a = aV2 - aV3, b = aV1 - aV3, c = aP - aV3;
            float aLen = a.X * a.X + a.Y * a.Y;
            float bLen = b.X * b.X + b.Y * b.Y;
            float ab = a.X * b.X + a.Y * b.Y;
            float ac = a.X * c.X + a.Y * c.Y;
            float bc = b.X * c.X + b.Y * c.Y;
            float d = aLen * bLen - ab * ab;
            u = (aLen * bc - ab * ac) / d;
            v = (bLen * ac - ab * bc) / d;
            w = 1.0f - u - v;
        }

        public Barycentric(Vector3 aV1, Vector3 aV2, Vector3 aV3, Vector3 aP)
        {
            Vector3 a = aV2 - aV3, b = aV1 - aV3, c = aP - aV3;
            float aLen = a.X * a.X + a.Y * a.Y + a.Z * a.Z;
            float bLen = b.X * b.X + b.Y * b.Y + b.Z * b.Z;
            float ab = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            float ac = a.X * c.X + a.Y * c.Y + a.Z * c.Z;
            float bc = b.X * c.X + b.Y * c.Y + b.Z * c.Z;
            float d = aLen * bLen - ab * ab;
            u = (aLen * bc - ab * ac) / d;
            v = (bLen * ac - ab * bc) / d;
            w = 1.0f - u - v;
        }
        public Barycentric(Vector4 aV1, Vector4 aV2, Vector4 aV3, Vector4 aP)
        {
            Vector4 a = aV2 - aV3, b = aV1 - aV3, c = aP - aV3;
            float aLen = a.X * a.X + a.Y * a.Y + a.Z * a.Z + a.W * a.W;
            float bLen = b.X * b.X + b.Y * b.Y + b.Z * b.Z + b.W * b.W;
            float ab = a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
            float ac = a.X * c.X + a.Y * c.Y + a.Z * c.Z + a.W * c.W;
            float bc = b.X * c.X + b.Y * c.Y + b.Z * c.Z + b.W * c.W;
            float d = aLen * bLen - ab * ab;
            u = (aLen * bc - ab * ac) / d;
            v = (bLen * ac - ab * bc) / d;
            w = 1.0f - u - v;
        }

        public bool IsInside
        {
            get
            {
                return (u >= 0.0f) && (u <= 1.0f) && (v >= 0.0f) && (v <= 1.0f) && (w >= 0.0f); //(w <= 1.0f)
            }
        }
        public Vector2 Interpolate(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return v1 * u + v2 * v + v3 * w;
        }
        public Vector3 Interpolate(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return v1 * u + v2 * v + v3 * w;
        }
        public Vector4 Interpolate(Vector4 v1, Vector4 v2, Vector4 v3)
        {
            return v1 * u + v2 * v + v3 * w;
        }
    }
}
