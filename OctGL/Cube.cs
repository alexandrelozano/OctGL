using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctGL
{
    public class Cube
    {
        BoundingBox bb;
        Vector2[] textureCoord;
        Vector3 size;
        Color color;

        public Cube(BoundingBox bb, Vector2[] textureCoord, Color color)
        {
            this.bb = bb;
            this.textureCoord = textureCoord;
            this.color = color;
            size = new Vector3((bb.Max.X - bb.Min.X) * 0.5f, (bb.Max.Y - bb.Min.Y) * 0.5f, (bb.Max.Z - bb.Min.Z) * 0.5f);
        }

        public void AddVertices(List<VertexPositionNormalTexture> lstVertexTriMesh, List<VertexPositionColor> lstVertexQuadMesh, bool ZFace, bool zFace, bool YFace, bool yFace, bool XFace, bool xFace)
        {
            Vector3 vxYz = new Vector3(bb.Min.X, bb.Max.Y, bb.Min.Z);
            Vector3 vxYZ = new Vector3(bb.Min.X, bb.Max.Y, bb.Max.Z);
            Vector3 vXYz = new Vector3(bb.Max.X, bb.Max.Y, bb.Min.Z);
            Vector3 vXYZ = new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z);
            Vector3 vxyz = new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z);
            Vector3 vxyZ = new Vector3(bb.Min.X, bb.Min.Y, bb.Max.Z);
            Vector3 vXyz = new Vector3(bb.Max.X, bb.Min.Y, bb.Min.Z);
            Vector3 vXyZ = new Vector3(bb.Max.X, bb.Min.Y, bb.Max.Z);
            
            if (zFace)
            {
                Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f) * size;

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYz, normalFront, textureCoord[Octree.NodePositions.xYz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyz, normalFront, textureCoord[Octree.NodePositions.xyz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYz, normalFront, textureCoord[Octree.NodePositions.XYz]));

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyz, normalFront, textureCoord[Octree.NodePositions.xyz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyz, normalFront, textureCoord[Octree.NodePositions.Xyz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYz, normalFront, textureCoord[Octree.NodePositions.XYz]));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, color));


                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, color));

            }

            if (ZFace)
            {
                Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f) * size;

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYZ, normalBack, textureCoord[Octree.NodePositions.xYZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYZ, normalBack, textureCoord[Octree.NodePositions.XYZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyZ, normalBack, textureCoord[Octree.NodePositions.xyZ]));

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyZ, normalBack, textureCoord[Octree.NodePositions.xyZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYZ, normalBack, textureCoord[Octree.NodePositions.XYZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyZ, normalBack, textureCoord[Octree.NodePositions.XyZ]));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, color));


                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, color));
            }

            if (YFace)
            {
                Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f) * size;

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYz, normalTop, textureCoord[Octree.NodePositions.xYz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYZ, normalTop, textureCoord[Octree.NodePositions.XYZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYZ, normalTop, textureCoord[Octree.NodePositions.xYZ]));

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYz, normalTop, textureCoord[Octree.NodePositions.xYz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYz, normalTop, textureCoord[Octree.NodePositions.XYz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYZ, normalTop, textureCoord[Octree.NodePositions.XYZ]));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, color));
            }

            if (yFace)
            {
                Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f) * size;

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyz, normalBottom, textureCoord[Octree.NodePositions.xyz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyZ, normalBottom, textureCoord[Octree.NodePositions.xyZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyZ, normalBottom, textureCoord[Octree.NodePositions.XyZ]));

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyz, normalBottom, textureCoord[Octree.NodePositions.xyz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyZ, normalBottom, textureCoord[Octree.NodePositions.XyZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyz, normalBottom, textureCoord[Octree.NodePositions.Xyz]));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, color));
            }

            if (xFace)
            {
                Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f) * size;

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYz, normalLeft, textureCoord[Octree.NodePositions.xYz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyZ, normalLeft, textureCoord[Octree.NodePositions.xyZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyz, normalLeft, textureCoord[Octree.NodePositions.xyz]));

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYZ, normalLeft, textureCoord[Octree.NodePositions.xYZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxyZ, normalLeft, textureCoord[Octree.NodePositions.xyZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vxYz, normalLeft, textureCoord[Octree.NodePositions.xYz]));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, color));
            }

            if (XFace)
            {
                Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f) * size;

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYz, normalRight, textureCoord[Octree.NodePositions.XYz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyz, normalRight, textureCoord[Octree.NodePositions.Xyz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyZ, normalRight, textureCoord[Octree.NodePositions.XyZ]));

                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYZ, normalRight, textureCoord[Octree.NodePositions.XYZ]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXYz, normalRight, textureCoord[Octree.NodePositions.XYz]));
                lstVertexTriMesh.Add(new VertexPositionNormalTexture(vXyZ, normalRight, textureCoord[Octree.NodePositions.XyZ]));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, color));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, color));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, color));
            }
        }
    }
}
