using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctGL
{
    class Arrow
    {
        private Vector3 A;
        private Vector3 Dir;
        private float length;
        private Color color;

        public Arrow(Vector3 A, Vector3 Dir, float length, Color color)
        {
            this.A = A;
            this.Dir = Dir;
            this.length = length;
            this.color = color;
        }

        public VertexPositionColor[] Create()
        {
            List<VertexPositionColor> lstVertices = new List<VertexPositionColor>();

            Vector3 B = new Vector3(A.X + Dir.X * length, A.Y + Dir.Y * length, A.Z + Dir.Z * length);
            Vector3 mVecN2_75 = new Vector3(A.X + Dir.X * length * 0.75f, A.Y + Dir.Y * length * 0.75f, A.Z + Dir.Z * length * 0.75f);

            lstVertices.Add(new VertexPositionColor(A, color));
            lstVertices.Add(new VertexPositionColor(B, color));

            Vector3 dir90CW = Vector3.Normalize(new Vector3(Dir.Z, 0, -Dir.X));
            Vector3 mVecN2L = new Vector3(mVecN2_75.X + dir90CW.X * length / 4.0f, mVecN2_75.Y + dir90CW.Y * length / 4.0f, mVecN2_75.Z + dir90CW.Z * length / 4.0f);

            lstVertices.Add(new VertexPositionColor(B, color));
            lstVertices.Add(new VertexPositionColor(mVecN2L, color));

            Vector3 dir90CCW = Vector3.Normalize(new Vector3(-Dir.Z, 0, Dir.X));
            Vector3 mVecN2R = new Vector3(mVecN2_75.X + dir90CCW.X * length / 4.0f, mVecN2_75.Y + dir90CCW.Y * length / 4.0f, mVecN2_75.Z + dir90CCW.Z * length / 4.0f);

            lstVertices.Add(new VertexPositionColor(B, color));
            lstVertices.Add(new VertexPositionColor(mVecN2R, color));

            return lstVertices.ToArray();
        }

    }
}
