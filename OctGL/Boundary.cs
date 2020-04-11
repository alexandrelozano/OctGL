using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctGL
{
    public class Boundary
    {
        public BoundingBox bb;

        public Boundary(BoundingBox bb)
        {
            this.bb = bb;
        }

        public void RenderToDevice(GraphicsDevice device, BasicEffect effect, EffectPass pass)
        {
            if (effect.TextureEnabled)
            {
                effect.TextureEnabled = false;
                effect.VertexColorEnabled = true;
                effect.LightingEnabled = false;
                pass.Apply();
            }
            var verticesX1 = new[] { new VertexPositionColor(new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Max.X, bb.Min.Y, bb.Min.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX1, 0, 1);
            var verticesX2 = new[] { new VertexPositionColor(new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Min.X, bb.Max.Y, bb.Min.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX2, 0, 1);
            var verticesX3 = new[] { new VertexPositionColor(new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Min.X, bb.Min.Y, bb.Max.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX3, 0, 1);
            var verticesX4 = new[] { new VertexPositionColor(new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Min.X, bb.Max.Y, bb.Max.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX4, 0, 1);
            var verticesX5 = new[] { new VertexPositionColor(new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Max.X, bb.Min.Y, bb.Max.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX5, 0, 1);
            var verticesX6 = new[] { new VertexPositionColor(new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Max.X, bb.Max.Y, bb.Min.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX6, 0, 1);
            var verticesX7 = new[] { new VertexPositionColor(new Vector3(bb.Min.X, bb.Min.Y, bb.Max.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Max.X, bb.Min.Y, bb.Max.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX7, 0, 1);
            var verticesX8 = new[] { new VertexPositionColor(new Vector3(bb.Min.X, bb.Min.Y, bb.Max.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Min.X, bb.Max.Y, bb.Max.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX8, 0, 1);
            var verticesX9 = new[] { new VertexPositionColor(new Vector3(bb.Min.X, bb.Max.Y, bb.Min.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Max.X, bb.Max.Y, bb.Min.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX9, 0, 1);
            var verticesX10 = new[] { new VertexPositionColor(new Vector3(bb.Min.X, bb.Max.Y, bb.Min.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Min.X, bb.Max.Y, bb.Max.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX10, 0, 1);
            var verticesX11 = new[] { new VertexPositionColor(new Vector3(bb.Max.X, bb.Min.Y, bb.Min.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Max.X, bb.Max.Y, bb.Min.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX11, 0, 1);
            var verticesX12 = new[] { new VertexPositionColor(new Vector3(bb.Max.X, bb.Min.Y, bb.Min.Z), Color.Yellow), new VertexPositionColor(new Vector3(bb.Max.X, bb.Min.Y, bb.Max.Z), Color.Yellow) };
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesX12, 0, 1);

        }
    }
}
