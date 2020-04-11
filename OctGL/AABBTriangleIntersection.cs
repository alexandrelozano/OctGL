using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OctGL
{
    class AABBTriangleIntersection
    {
        public static Vector3[] boxNormals;

        public AABBTriangleIntersection()
        {
            if (boxNormals==null)
            {
                boxNormals = new Vector3[] {
                    new Vector3(1,0,0),
                    new Vector3(0,1,0),
                    new Vector3(0,0,1)
                };
            }
        }

        public bool IsIntersecting(Vector3[] tri, float[] coordsBBMin, float[] coordsBBMax, Vector3[] bbCorners)
        {
            var dir = Vector3.Cross(tri[1] - tri[0], tri[2] - tri[0]);
            var norm = Vector3.Normalize(dir);
            
            return IsIntersecting(tri, norm, coordsBBMin, coordsBBMax, bbCorners);
        }

        bool IsIntersecting(Vector3[] tri, Vector3 triNormal, float[] coordsBBMin, float[] coordsBBMax, Vector3[] bbCorners)
        {
            float triangleMin, triangleMax;
            
            // Test the box normals (x-, y- and z-axes)
            for (int i = 0; i < 3; i++)
            {
                Project(tri, boxNormals[i], out triangleMin, out triangleMax);
                if (triangleMax < coordsBBMin[i] || triangleMin > coordsBBMax[i])
                    return false; // No intersection possible.
            }

            // Test the triangle normal
            float boxMin, boxMax;
            double triangleOffset = Vector3.Dot(triNormal, tri[0]);
            Project(bbCorners, triNormal, out boxMin, out boxMax);
            if (boxMax < triangleOffset || boxMin > triangleOffset)
                return false; // No intersection possible.

            // Test the nine edge cross-products
            Vector3[] triangleEdges = new Vector3[] {
                Vector3.Subtract(tri[0],tri[1]),
                Vector3.Subtract(tri[1],tri[2]),
                Vector3.Subtract(tri[2],tri[0])
            };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    // The box normals are the same as it's edge tangents
                    Vector3 axis = Vector3.Cross(triangleEdges[i],boxNormals[j]);
                    Project(bbCorners, axis, out boxMin, out boxMax);
                    Project(tri, axis, out triangleMin, out triangleMax);
                    if (boxMax <= triangleMin || boxMin >= triangleMax)
                        return false; // No intersection possible
                }

            // No separating axis found.
            return true;
        }

        void Project(IEnumerable<Vector3> points, Vector3 axis,
                out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;
            foreach (var p in points)
            {
                float val = Vector3.Dot(axis,p);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        public float[] Coords(Vector3 v)
        {
            float[] c = new float[3];
            c[0] = v.X;
            c[1] = v.Y;
            c[2] = v.Z;

            return c;
        }
        
    }
}
