using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctGL
{
    public class Axis
    {
        public float size;
        public bool full;

        private Color Xcolor = Color.Red;
        private Color Ycolor = Color.Green;
        private Color Zcolor = Color.Blue;

        public Axis(float size)
        {
            this.size = size;
            full = false;
        }

        public void RenderToDevice(GraphicsDevice device, BasicEffect effect, EffectPass pass)
        {
            if (full)
            {
                var verticesX = new[] { new VertexPositionColor(new Vector3(size, 0f, 0f), Xcolor), new VertexPositionColor(new Vector3(-size, 0f, 0f), Xcolor) };
                device.DrawUserPrimitives(PrimitiveType.LineList, verticesX, 0, 1);
                var verticesY = new[] { new VertexPositionColor(new Vector3(0f, size, 0f), Ycolor), new VertexPositionColor(new Vector3(0f, -size, 0f), Ycolor) };
                device.DrawUserPrimitives(PrimitiveType.LineList, verticesY, 0, 1);
                var verticesZ = new[] { new VertexPositionColor(new Vector3(0f, 0f, size), Zcolor), new VertexPositionColor(new Vector3(0f, 0f, -size), Zcolor) };
                device.DrawUserPrimitives(PrimitiveType.LineList, verticesZ, 0, 1);
            }
            else
            {
                var verticesX = new[] { new VertexPositionColor(new Vector3(size, 0f, 0f), Xcolor), new VertexPositionColor(new Vector3(0f, 0f, 0f), Xcolor) };
                device.DrawUserPrimitives(PrimitiveType.LineList, verticesX, 0, 1);
                var verticesY = new[] { new VertexPositionColor(new Vector3(0f, size, 0f), Ycolor), new VertexPositionColor(new Vector3(0f, 0f, 0f), Ycolor) };
                device.DrawUserPrimitives(PrimitiveType.LineList, verticesY, 0, 1);
                var verticesZ = new[] { new VertexPositionColor(new Vector3(0f, 0f, size), Zcolor), new VertexPositionColor(new Vector3(0f, 0f, 0f), Zcolor) };
                device.DrawUserPrimitives(PrimitiveType.LineList, verticesZ, 0, 1);
            }

            var verticesLabelX = new[] { new VertexPositionColor(new Vector3(size + (size * 0.2f), size * 0.1f, 0f), Xcolor), new VertexPositionColor(new Vector3(size + size * 0.1f, -size * 0.1f, 0f), Xcolor),
                new VertexPositionColor(new Vector3(size + (size * 0.2f), -size * 0.1f, 0f), Xcolor), new VertexPositionColor(new Vector3(size + size * 0.1f, size * 0.1f, 0f), Xcolor)};
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesLabelX, 0, 2);

            var verticesLabelY = new[] { new VertexPositionColor(new Vector3(size * 0.1f, size + (size * 0.2f), 0f), Ycolor), new VertexPositionColor(new Vector3(0f, size + (size * 0.15f), 0f), Ycolor),
                new VertexPositionColor(new Vector3(-size * 0.1f, size + (size * 0.2f), 0f), Ycolor), new VertexPositionColor(new Vector3(0f, size + (size * 0.15f), 0f), Ycolor),
                new VertexPositionColor(new Vector3(0f, size + (size * 0.1f), 0f), Ycolor), new VertexPositionColor(new Vector3(0f, size + (size * 0.15f), 0f), Ycolor)};
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesLabelY, 0, 3);

            var verticesLabelZ = new[] { new VertexPositionColor(new Vector3(0f, size * 0.1f,size + (size * 0.2f)), Zcolor), new VertexPositionColor(new Vector3(0f, -size * 0.1f, size + (size * 0.2f)), Zcolor),
                new VertexPositionColor(new Vector3(0f, size * 0.1f, size + (size * 0.2f)), Zcolor), new VertexPositionColor(new Vector3(0f, -size * 0.1f, size + size * 0.1f), Zcolor),
                new VertexPositionColor(new Vector3(0f, size * 0.1f, size + size * 0.1f), Zcolor), new VertexPositionColor(new Vector3(0f, -size * 0.1f, size + size * 0.1f), Zcolor)};
            device.DrawUserPrimitives(PrimitiveType.LineList, verticesLabelZ, 0, 3);
        }
    }
}
