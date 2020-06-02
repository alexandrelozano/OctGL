using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctGL
{
    public class Cube
    {
        BoundingBox bb;
        Vector3 size;
        Color color;

        public Cube(BoundingBox bb, Color color)
        {
            this.bb = bb;
            this.color = color;
            size = new Vector3((bb.Max.X - bb.Min.X) * 0.5f, (bb.Max.Y - bb.Min.Y) * 0.5f, (bb.Max.Z - bb.Min.Z) * 0.5f);
        }

        public void AddVertices(List<VertexPositionColorNormal> lstVertexTriColorMesh, List<VertexPositionColor> lstVertexQuadMesh, bool ZFace, bool zFace, bool YFace, bool yFace, bool XFace, bool xFace)
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

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYz, color, normalFront));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyz, color, normalFront));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYz, color, normalFront));

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyz, color, normalFront));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyz, color, normalFront));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYz, color, normalFront));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, Color.White));

            }

            if (ZFace)
            {
                Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f) * size;

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYZ, color, normalBack));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYZ, color, normalBack));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyZ, color, normalBack));

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyZ, color, normalBack));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYZ, color, normalBack));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyZ, color, normalBack));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, Color.White));
            }

            if (YFace)
            {
                Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f) * size;

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYz, color, normalTop));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYZ, color, normalTop));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYZ, color, normalTop));

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYz, color, normalTop));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYz, color, normalTop));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYZ, color, normalTop));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, Color.White));
            }

            if (yFace)
            {
                Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f) * size;

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyz, color, normalBottom));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyZ, color, normalBottom));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyZ, color, normalBottom));

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyz, color, normalBottom));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyZ, color, normalBottom));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyz, color, normalBottom));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, Color.White));
            }

            if (xFace)
            {
                Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f) * size;

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYz, color, normalLeft));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyZ, color, normalLeft));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyz, color, normalLeft));

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYZ, color, normalLeft));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxyZ, color, normalLeft));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vxYz, color, normalLeft));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxYz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vxyZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vxYZ, Color.White));
            }

            if (XFace)
            {
                Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f) * size;

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYz, color, normalRight));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyz, color, normalRight));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyZ, color, normalRight));

                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYZ, color, normalRight));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXYz, color, normalRight));
                lstVertexTriColorMesh.Add(new VertexPositionColorNormal(vXyZ, color, normalRight));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXYz, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, Color.White));

                lstVertexQuadMesh.Add(new VertexPositionColor(vXyZ, Color.White));
                lstVertexQuadMesh.Add(new VertexPositionColor(vXYZ, Color.White));
            }
        }

        public VertexPositionColorNormal[] SolidVertices()
        {
            VertexPositionColorNormal[] arrVerticesTriColorMesh= new VertexPositionColorNormal[36];

            Vector3 vxYz = new Vector3(bb.Min.X, bb.Max.Y, bb.Min.Z);
            Vector3 vxYZ = new Vector3(bb.Min.X, bb.Max.Y, bb.Max.Z);
            Vector3 vXYz = new Vector3(bb.Max.X, bb.Max.Y, bb.Min.Z);
            Vector3 vXYZ = new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z);
            Vector3 vxyz = new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z);
            Vector3 vxyZ = new Vector3(bb.Min.X, bb.Min.Y, bb.Max.Z);
            Vector3 vXyz = new Vector3(bb.Max.X, bb.Min.Y, bb.Min.Z);
            Vector3 vXyZ = new Vector3(bb.Max.X, bb.Min.Y, bb.Max.Z);

            Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f) * size;

            arrVerticesTriColorMesh[0]=new VertexPositionColorNormal(vxYz, color, normalFront);
            arrVerticesTriColorMesh[1] = new VertexPositionColorNormal(vxyz, color, normalFront);
            arrVerticesTriColorMesh[2] = new VertexPositionColorNormal(vXYz, color, normalFront);

            arrVerticesTriColorMesh[3]=new VertexPositionColorNormal(vxyz, color, normalFront);
            arrVerticesTriColorMesh[4]=new VertexPositionColorNormal(vXyz, color, normalFront);
            arrVerticesTriColorMesh[5]=new VertexPositionColorNormal(vXYz, color, normalFront);

            Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f) * size;

            arrVerticesTriColorMesh[6]=new VertexPositionColorNormal(vxYZ, color, normalBack);
            arrVerticesTriColorMesh[7]=new VertexPositionColorNormal(vXYZ, color, normalBack);
            arrVerticesTriColorMesh[8]=new VertexPositionColorNormal(vxyZ, color, normalBack);

            arrVerticesTriColorMesh[9]=new VertexPositionColorNormal(vxyZ, color, normalBack);
            arrVerticesTriColorMesh[10]=new VertexPositionColorNormal(vXYZ, color, normalBack);
            arrVerticesTriColorMesh[11]=new VertexPositionColorNormal(vXyZ, color, normalBack);


            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f) * size;

            arrVerticesTriColorMesh[12]=new VertexPositionColorNormal(vxYz, color, normalTop);
            arrVerticesTriColorMesh[13]=new VertexPositionColorNormal(vXYZ, color, normalTop);
            arrVerticesTriColorMesh[14]=new VertexPositionColorNormal(vxYZ, color, normalTop);

            arrVerticesTriColorMesh[15]=new VertexPositionColorNormal(vxYz, color, normalTop);
            arrVerticesTriColorMesh[16]=new VertexPositionColorNormal(vXYz, color, normalTop);
            arrVerticesTriColorMesh[17]=new VertexPositionColorNormal(vXYZ, color, normalTop);

            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f) * size;

            arrVerticesTriColorMesh[18]=new VertexPositionColorNormal(vxyz, color, normalBottom);
            arrVerticesTriColorMesh[19]=new VertexPositionColorNormal(vxyZ, color, normalBottom);
            arrVerticesTriColorMesh[20]=new VertexPositionColorNormal(vXyZ, color, normalBottom);

            arrVerticesTriColorMesh[21]=new VertexPositionColorNormal(vxyz, color, normalBottom);
            arrVerticesTriColorMesh[22]=new VertexPositionColorNormal(vXyZ, color, normalBottom);
            arrVerticesTriColorMesh[23]=new VertexPositionColorNormal(vXyz, color, normalBottom);

            Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f) * size;

            arrVerticesTriColorMesh[24]=new VertexPositionColorNormal(vxYz, color, normalLeft);
            arrVerticesTriColorMesh[25]=new VertexPositionColorNormal(vxyZ, color, normalLeft);
            arrVerticesTriColorMesh[26]=new VertexPositionColorNormal(vxyz, color, normalLeft);

            arrVerticesTriColorMesh[27]=new VertexPositionColorNormal(vxYZ, color, normalLeft);
            arrVerticesTriColorMesh[28]=new VertexPositionColorNormal(vxyZ, color, normalLeft);
            arrVerticesTriColorMesh[29]=new VertexPositionColorNormal(vxYz, color, normalLeft);
                
            Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f) * size;

            arrVerticesTriColorMesh[30]=new VertexPositionColorNormal(vXYz, color, normalRight);
            arrVerticesTriColorMesh[31]=new VertexPositionColorNormal(vXyz, color, normalRight);
            arrVerticesTriColorMesh[32]=new VertexPositionColorNormal(vXyZ, color, normalRight);

            arrVerticesTriColorMesh[33]=new VertexPositionColorNormal(vXYZ, color, normalRight);
            arrVerticesTriColorMesh[34]=new VertexPositionColorNormal(vXYz, color, normalRight);
            arrVerticesTriColorMesh[35]=new VertexPositionColorNormal(vXyZ, color, normalRight);

            return arrVerticesTriColorMesh;
        }
    }
}
