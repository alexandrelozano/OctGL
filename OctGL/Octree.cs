﻿using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OctGL
{
    public class Octree
    {
        public enum OctreeStates
        {
            Empty = 0,
            Full = 1,
            Mixted = 2
        }

        public class NodePositions
        {
            public const int xyz = 0;
            public const int xyZ = 1;
            public const int xYz = 2;
            public const int xYZ = 3;
            public const int Xyz = 4;
            public const int XyZ = 5;
            public const int XYz = 6;
            public const int XYZ = 7;
        }

        public int level;

        public OctreeStates state;
        public Octree[] childs;
        public Octree parent;
        public Octree root;
        public short position;
        public bool faceUP;
        public bool fillingOctant;
        public string id;

        public BoundingBox bb;
        public Vector2[] textureCoord;

        public Game game;

        public short depthMax;

        public short octantTextureCoordinates;
        public bool optimizeOctantFaces;
        public string fillDirection;

        public double octants;
        public double octantsMax;
        public double octantsFilled;

        public string buildingStage;

        public double textureCoordinates;
        public double textureCoordinatesMax;

        public DateTime? startTime;
        public DateTime? endTime;

        private BModel bModel;
        private Cube cube;

        public int verticesNumber;
        public VertexPositionNormalTexture[] verticesTriMesh;
        public VertexPositionColor[] verticesQuadMesh;

        private AABBTriangleIntersection aabbint;

        public Octree(Game game)
        {

            this.game = game;
            root = this;
            state = OctreeStates.Empty;
            aabbint = new AABBTriangleIntersection();
            startTime = null;
            endTime = null;
        }

        public Octree(Game game, short depthMax, short octantTextureCoordinates, bool optimizeOctantFaces, string fillDirection) : this(game)
        {
            this.depthMax = depthMax;
            octants = 0;
            octantsMax = System.Math.Pow(8, depthMax);
            textureCoordinates = 0;
            this.octantTextureCoordinates = octantTextureCoordinates;
            this.optimizeOctantFaces = optimizeOctantFaces;
            this.fillDirection = fillDirection;
        }

        public void OpenOctree(String filePath)
        {
            int i = 0;
            String serialization = "";
            Octree current = this;
            Stack<Octree> st = new Stack<Octree>();

            System.IO.StreamReader file = new System.IO.StreamReader(filePath);

            serialization = file.ReadLine();
            String[] pointsMax = serialization.Split('#');
            serialization = file.ReadLine();
            String[] pointsMin = serialization.Split('#');

            BoundingBox bb = new BoundingBox(new Vector3(float.Parse(pointsMin[0]), float.Parse(pointsMin[1]), float.Parse(pointsMin[2])), new Vector3(float.Parse(pointsMax[0]), float.Parse(pointsMax[1]), float.Parse(pointsMax[2])));

            serialization = file.ReadLine();

            current.bb = bb;
            current.root = this;
            st.Push(current);

            while (i < serialization.Length)
            {
                switch (serialization[i])
                {
                    case '0':
                        current.state = OctreeStates.Empty;
                        break;
                    case '1':
                        current.state = OctreeStates.Full;
                        octantsFilled++;
                        break;
                    case '(':
                        current.state = OctreeStates.Mixted;
                        current.CreateChilds(current.bb);
                        for (int j = 0; j < 8; j++)
                        {
                            st.Push(current.childs[j]);
                        }

                        break;
                }

                i++;
                current = st.Pop();
            }

            file.Close();
        }


        public void SaveOctree(String filePath)
        {
            Stack<Octree> st = new Stack<Octree>();
            Octree current = this;
            String serialization = "";

            serialization = current.bb.Max.X + "#" + current.bb.Max.Y + "#" + current.bb.Max.Z + "\r\n";
            serialization += current.bb.Min.X + "#" + current.bb.Min.Y + "#" + current.bb.Min.Z + "\r\n";

            st.Push(current);
            while (st.Count > 0)
            {
                current = st.Pop();

                if (current.state == OctreeStates.Mixted)
                {
                    serialization += "(";

                    if (current.childs != null)
                    {
                        for (short i = 0; i < 8; i++)
                        {
                            st.Push(current.childs[i]);
                        }
                    }
                }
                else if (current.state == OctreeStates.Full)
                {
                    serialization += "1";
                }
                else
                {
                    serialization += "0";
                }
            }

            System.IO.File.WriteAllText(filePath, serialization);
        }

        public void Optimize()
        {
            buildingStage = "Optimizing...";

            Stack<Octree> st = new Stack<Octree>();
            Octree current = this;
            int emptyChilds;
            int fullChilds;
            octantsFilled = 0;

            st.Push(current);
            while (st.Count > 0)
            {
                current = st.Pop();

                if (current.state == OctreeStates.Mixted)
                {
                    emptyChilds = 0;
                    fullChilds = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        if (current.childs[i].state == OctreeStates.Empty)
                        {
                            emptyChilds++;
                        }
                        else if (current.childs[i].state == OctreeStates.Full)
                        {
                            fullChilds++;
                        }
                    }

                    if (fullChilds == 8)
                    {
                        current.state = OctreeStates.Full;

                        switch (root.octantTextureCoordinates)
                        {
                            case 0:
                                current.textureCoord = null;
                                break;
                            case 1:
                                current.textureCoord = new Vector2[8];
                                for (int j = 0; j < 8; j++)
                                {
                                    current.textureCoord[j] = current.childs[0].textureCoord[j];
                                }
                                break;
                            case 8:
                                current.textureCoord = new Vector2[8];
                                current.textureCoord[0] = current.childs[7].textureCoord[0];
                                current.textureCoord[1] = current.childs[1].textureCoord[1];
                                current.textureCoord[2] = current.childs[5].textureCoord[2];
                                current.textureCoord[3] = current.childs[3].textureCoord[3];
                                current.textureCoord[4] = current.childs[6].textureCoord[4];
                                current.textureCoord[5] = current.childs[0].textureCoord[5];
                                current.textureCoord[6] = current.childs[4].textureCoord[6];
                                current.textureCoord[7] = current.childs[2].textureCoord[7];
                                break;
                        }

                        for (int j = 0; j < 8; j++)
                        {
                            current.childs[j] = null;
                        }

                        octantsFilled++;
                        if (current.level > 0)
                        {
                            st.Push(current.parent);
                        }
                    }
                    else if (emptyChilds == 8)
                    {
                        current.state = OctreeStates.Empty;

                        for (int j = 0; j < 8; j++)
                        {
                            current.childs[j] = null;
                        }

                        st.Push(current.parent);
                    }
                    else
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            st.Push(current.childs[j]);
                        }
                    }
                }
                else if (current.state == OctreeStates.Full)
                {
                    octantsFilled++;
                }
            }
        }

        public void Fill()
        {
            buildingStage = "Filling...";

            Stack<Octree> st = new Stack<Octree>();
            Octree current = this;

            st.Push(current);
            while (st.Count > 0)
            {
                current = st.Pop();

                if (current.state == OctreeStates.Mixted)
                {
                    if (current.childs != null)
                    {
                        for (short i = 0; i < 8; i++)
                        {
                            st.Push(current.childs[i]);
                        }
                    }
                }
                else if (current.state == OctreeStates.Full)
                {
                    if (current.faceUP)
                    {
                        Octree neighborDown = null;

                        List<Octree> lstFillOctants = new List<Octree>();
                        neighborDown = current.FindNeighborDown();
                        while (neighborDown != null && neighborDown.faceUP)
                        {
                            lstFillOctants.Add(neighborDown);
                            neighborDown = neighborDown.FindNeighborDown();
                        }

                        // Set empty all new octants if we arrive at octree outer space
                        if (neighborDown == null)
                        {
                            foreach (Octree octant in lstFillOctants)
                            {
                                if (octant.fillingOctant == true)
                                {
                                    octant.state = OctreeStates.Empty;
                                }
                            }
                        }
                    }
                }
            }
        }

        public Octree FindNeighborDown()
        {
            Octree neighborDown=null;

            switch (root.fillDirection)
            {
                case "Z-":
                    neighborDown = FindNeighborZPlusEqualSize();
                    break;
                case "Z+":
                    neighborDown = FindNeighborZPlusEqualSize();
                    break;
                case "X-":
                    neighborDown = FindNeighborXMinusEqualSize();
                    break;
                case "X+":
                    neighborDown = FindNeighborXPlusEqualSize();
                    break;
                case "Y-":
                    neighborDown = FindNeighborYMinusEqualSize();
                    break;
                case "Y+":
                    neighborDown = FindNeighborYPlusEqualSize();
                    break;
            }

            return neighborDown;
        }

        public void BuildTextureCoordinates()
        {
            buildingStage = "Texture coordinates...";

            Stack<Octree> st = new Stack<Octree>();
            Octree current = this;

            textureCoordinates = 0;

            st.Push(current);
            while (st.Count > 0)
            {
                current = st.Pop();

                if (current.state == OctreeStates.Full)
                {
                    current.textureCoord = new Vector2[8];

                    if (root.octantTextureCoordinates == 1)
                    {
                        textureCoordinates ++;
                        var pos = new Vector3();
                        pos.X = current.bb.Min.X + ((current.bb.Max.X - current.bb.Min.X) * 0.5f);
                        pos.Y = current.bb.Min.Y + ((current.bb.Max.Y - current.bb.Min.Y) * 0.5f);
                        pos.Z = current.bb.Min.Z + ((current.bb.Max.Z - current.bb.Min.Z) * 0.5f);

                        Vector2 textCoordInt = CalculateTextureCoordinates(pos.X, pos.Y, pos.Z, current);
                        for (int i = 0; i < 8; i++)
                        {
                            current.textureCoord[i].X = textCoordInt.X;
                            current.textureCoord[i].Y = textCoordInt.Y;
                        }
                    }
                    else if (root.octantTextureCoordinates == 8)
                    {
                        textureCoordinates += 8;
                        Vector2 textCoordInt = CalculateTextureCoordinates(current.bb.Min.X, current.bb.Min.Y, current.bb.Min.Z, current);
                        current.textureCoord[NodePositions.xyz].X = textCoordInt.X;
                        current.textureCoord[NodePositions.xyz].Y = textCoordInt.Y;
                        textCoordInt = CalculateTextureCoordinates(current.bb.Min.X, current.bb.Min.Y, current.bb.Max.Z, current);
                        current.textureCoord[NodePositions.xyZ].X = textCoordInt.X;
                        current.textureCoord[NodePositions.xyZ].Y = textCoordInt.Y;
                        textCoordInt = CalculateTextureCoordinates(current.bb.Min.X, current.bb.Max.Y, current.bb.Min.Z, current);
                        current.textureCoord[NodePositions.xYz].X = textCoordInt.X;
                        current.textureCoord[NodePositions.xYz].Y = textCoordInt.Y;
                        textCoordInt = CalculateTextureCoordinates(current.bb.Min.X, current.bb.Max.Y, current.bb.Max.Z, current);
                        current.textureCoord[NodePositions.xYZ].X = textCoordInt.X;
                        current.textureCoord[NodePositions.xYZ].Y = textCoordInt.Y;
                        textCoordInt = CalculateTextureCoordinates(current.bb.Max.X, current.bb.Min.Y, current.bb.Min.Z, current);
                        current.textureCoord[NodePositions.Xyz].X = textCoordInt.X;
                        current.textureCoord[NodePositions.Xyz].Y = textCoordInt.Y;
                        textCoordInt = CalculateTextureCoordinates(current.bb.Max.X, current.bb.Min.Y, current.bb.Max.Z, current);
                        current.textureCoord[NodePositions.XyZ].X = textCoordInt.X;
                        current.textureCoord[NodePositions.XyZ].Y = textCoordInt.Y;
                        textCoordInt = CalculateTextureCoordinates(current.bb.Max.X, current.bb.Max.Y, current.bb.Min.Z, current);
                        current.textureCoord[NodePositions.XYz].X = textCoordInt.X;
                        current.textureCoord[NodePositions.XYz].Y = textCoordInt.Y;
                        textCoordInt = CalculateTextureCoordinates(current.bb.Max.X, current.bb.Max.Y, current.bb.Max.Z, current);
                        current.textureCoord[NodePositions.XYZ].X = textCoordInt.X;
                        current.textureCoord[NodePositions.XYZ].Y = textCoordInt.Y;
                    }
                }
                else if (state == OctreeStates.Mixted)
                {
                    if (current.childs != null)
                    {
                        for (short i = 0; i < 8; i++)
                        {
                            st.Push(current.childs[i]);
                        }
                    }
                }
            }
        }

        public void BuildMesh()
        {
            buildingStage = "Building mesh...";

            Stack<Octree> st = new Stack<Octree>();
            Octree current = this;
            Octree nZplus = null;
            Octree nZminus = null;
            Octree nYplus = null;
            Octree nYminus = null;
            Octree nXplus = null;
            Octree nXminus = null;
            List<VertexPositionNormalTexture> lstVerticesTriMesh = new List<VertexPositionNormalTexture>();
            List<VertexPositionColor> lstVerticesQuadMesh = new List<VertexPositionColor>();

            verticesNumber = 0;

            st.Push(current);
            while (st.Count > 0)
            {
                current = st.Pop();

                if (current.state == OctreeStates.Full)
                {
                    cube = new Cube(current.bb, current.textureCoord, Color.White);

                    if (optimizeOctantFaces)
                    {
                        var tasks = new Task[6]
                        {
                        Task.Factory.StartNew(() => {nZplus = current.FindNeighborZPlus(); }),
                        Task.Factory.StartNew(() => {nZminus = current.FindNeighborZMinus(); }),
                        Task.Factory.StartNew(() => {nYplus = current.FindNeighborYPlus(); }),
                        Task.Factory.StartNew(() => {nYminus = current.FindNeighborYMinus(); }),
                        Task.Factory.StartNew(() => {nXplus = current.FindNeighborXPlus(); }),
                        Task.Factory.StartNew(() => {nXminus = current.FindNeighborXMinus(); })
                        };
                        Task.WaitAll(tasks);
                    }

                    cube.AddVertices(lstVerticesTriMesh, lstVerticesQuadMesh,
                        (nZplus == null || nZplus.state != OctreeStates.Full),
                        (nZminus == null || nZminus.state != OctreeStates.Full),
                        (nYplus == null || nYplus.state != OctreeStates.Full),
                        (nYminus == null || nYminus.state != OctreeStates.Full),
                        (nXplus == null || nXplus.state != OctreeStates.Full),
                        (nXminus == null || nXminus.state != OctreeStates.Full));

                    verticesNumber = lstVerticesTriMesh.Count;
                }
                else if (current.state == OctreeStates.Mixted)
                {

                    if (current.childs != null)
                    {
                        for (short i = 0; i < 8; i++)
                        {
                            st.Push(current.childs[i]);
                        }
                    }
                }
            }

            verticesTriMesh = lstVerticesTriMesh.ToArray();
            verticesQuadMesh = lstVerticesQuadMesh.ToArray();
        }

        public void Build(BModel bModel)
        {
            buildingStage = "Building...";
            this.bModel = bModel;
            bb = bModel.bb;
            textureCoordinates = 0;
            textureCoordinatesMax = 0;
            faceUP = true;
            BuildLevel();
        }

        private void BuildLevel()
        {
            if (bModel == null)
            {
                return;
            }

            bool intersect = false;
            float[] coordBBMin = aabbint.Coords(bb.Min);
            float[] coordBBMax = aabbint.Coords(bb.Max);
            Vector3[] bbCorners = bb.GetCorners();

            for (int r = 0; r < bModel.oScene.MeshCount; r++)
            {
                Mesh m = bModel.oScene.Meshes[r];
                for (int k = 0; k < m.FaceCount; k++)
                {
                    Face f = m.Faces[k];
                    Vector3[] tri = new Vector3[3];

                    for (int i = 0; i < 3; i++)
                    {
                        tri[i] = new Vector3(root.bModel.oScene.Meshes[r].Vertices[f.Indices[i]].X,
                            root.bModel.oScene.Meshes[r].Vertices[f.Indices[i]].Y,
                            root.bModel.oScene.Meshes[r].Vertices[f.Indices[i]].Z);
                    }

                    if (aabbint.IsIntersecting(tri, coordBBMin, coordBBMax, bbCorners))
                    {
                        intersect = true;

                        switch (root.fillDirection)
                        {
                            case ("Z-"):
                                if (root.bModel.oScene.Meshes[r].Normals[f.Indices[0]].Z < 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[1]].Z < 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[2]].Z < 0)
                                        faceUP = false;
                                    else
                                        faceUP = true;
                                break;
                            case ("Z+"):
                                if (root.bModel.oScene.Meshes[r].Normals[f.Indices[0]].Z > 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[1]].Z > 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[2]].Z > 0)
                                    faceUP = false;
                                else
                                    faceUP = true;
                                break;
                            case ("X-"):
                                if (root.bModel.oScene.Meshes[r].Normals[f.Indices[0]].X < 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[1]].X < 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[2]].X < 0)
                                    faceUP = false;
                                else
                                    faceUP = true;
                                break;
                            case ("X+"):
                                if (root.bModel.oScene.Meshes[r].Normals[f.Indices[0]].X > 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[1]].X > 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[2]].X > 0)
                                    faceUP = false;
                                else
                                    faceUP = true;
                                break;
                            case ("Y-"):
                                if (root.bModel.oScene.Meshes[r].Normals[f.Indices[0]].Y < 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[1]].Y < 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[2]].Y < 0)
                                    faceUP = false;
                                else
                                    faceUP = true;
                                break;
                            case ("Y+"):
                                if (root.bModel.oScene.Meshes[r].Normals[f.Indices[0]].Y > 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[1]].Y > 0 ||
                                    root.bModel.oScene.Meshes[r].Normals[f.Indices[2]].Y > 0)
                                    faceUP = false;
                                else
                                    faceUP = true;
                                break;
                        }

                        break;
                    }
                }

                if (intersect)
                {
                    break;
                }
            }

            
            if (intersect)
            {
                if (level == root.depthMax)
                {
                    state = OctreeStates.Full;

                    switch (root.octantTextureCoordinates)
                    {
                        case 1:
                            root.textureCoordinatesMax ++;
                            break;
                        case 8:
                            root.textureCoordinatesMax += 8;
                            break;
                    }

                    // If we are filling octree we don't need to increment counter
                    if (root.octants < root.octantsMax)
                    {
                        root.octants++;
                    }
                    root.octantsFilled++;
                }
                else
                {
                    state = OctreeStates.Mixted;
                    CreateChilds(bb);
                }
            }
            else
            {
                state = OctreeStates.Empty;

                // If we are filling octree we don't need to increment counter
                if (root.octants < root.octantsMax)
                {
                    root.octants += Math.Pow(8, (root.depthMax - level));
                }
            }
        }

        public void RenderToDevice(BasicEffect effect, GraphicsDevice device, bool wireframe)
        {
            if (wireframe)
            {
                if (verticesQuadMesh != null && verticesQuadMesh.Length > 0)
                {
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        device.DrawUserPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList, verticesQuadMesh, 0, verticesQuadMesh.Length / 2);
                    }
                }
            }
            else {
                if (verticesTriMesh != null && verticesTriMesh.Length > 0)
                {
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        device.DrawUserPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, verticesTriMesh, 0, verticesTriMesh.Length / 3);
                    }
                }
            }
        }

        private Vector2 CalculateTextureCoordinates(float X, float Y, float Z, Octree current)
        {
            if (root.bModel.oScene.Meshes[0].TextureCoordinateChannels[0].Count > 0)
            {

                Face closestFace = new Face();
                float minDistance = float.MaxValue;
                Vector3 closestPoint = new Vector3();
                Vector3 baryCoords = new Vector3();
                Vector3 point = new Vector3(X, Y, Z);
                Vector3 closestT0 = new Vector3();
                Vector3 closestT1 = new Vector3();
                Vector3 closestT2 = new Vector3();
                Vector2 textCoordT0 = new Vector2();
                Vector2 textCoordT1 = new Vector2();
                Vector2 textCoordT2 = new Vector2();

                for (int r = 0; r < current.bModel.oScene.MeshCount; r++)
                {
                    Mesh m = current.bModel.oScene.Meshes[r];
                    //Parallel.ForEach(m.Faces, (f, state) =>
                    foreach (Face f in m.Faces)
                    {
                        Vector3 t0 = new Vector3(root.bModel.oScene.Meshes[r].Vertices[f.Indices[0]].X,
                            root.bModel.oScene.Meshes[r].Vertices[f.Indices[0]].Y,
                            root.bModel.oScene.Meshes[r].Vertices[f.Indices[0]].Z);
                        Vector3 t1 = new Vector3(root.bModel.oScene.Meshes[r].Vertices[f.Indices[1]].X,
                                root.bModel.oScene.Meshes[r].Vertices[f.Indices[1]].Y,
                                root.bModel.oScene.Meshes[r].Vertices[f.Indices[1]].Z);
                        Vector3 t2 = new Vector3(root.bModel.oScene.Meshes[r].Vertices[f.Indices[2]].X,
                                root.bModel.oScene.Meshes[r].Vertices[f.Indices[2]].Y,
                                root.bModel.oScene.Meshes[r].Vertices[f.Indices[2]].Z);

                        float distance = Distance.PointToTriangle(point, t0, t1, t2, out closestPoint, out baryCoords);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestFace = f;
                            closestT0 = t0;
                            closestT1 = t1;
                            closestT2 = t2;
                            textCoordT0 = new Vector2(root.bModel.oScene.Meshes[r].TextureCoordinateChannels[0][f.Indices[0]].X,
                                root.bModel.oScene.Meshes[r].TextureCoordinateChannels[0][f.Indices[0]].Y);
                            textCoordT1 = new Vector2(root.bModel.oScene.Meshes[r].TextureCoordinateChannels[0][f.Indices[1]].X,
                                root.bModel.oScene.Meshes[r].TextureCoordinateChannels[0][f.Indices[1]].Y);
                            textCoordT2 = new Vector2(root.bModel.oScene.Meshes[r].TextureCoordinateChannels[0][f.Indices[2]].X,
                                root.bModel.oScene.Meshes[r].TextureCoordinateChannels[0][f.Indices[2]].Y);
                        }

                    }
                }

                var coord = new Barycentric(closestT0, closestT1, closestT2, closestPoint);

                return coord.Interpolate(textCoordT0, textCoordT1, textCoordT2);
            }
            else
            {
                return new Vector2(0, 0);
            }
        }

        private void CreateChilds(BoundingBox bb)
        {
            childs = new Octree[8];

            for (short i = 0; i < 8; i++)
            {
                childs[i] = new Octree(game);
                childs[i].parent = this;
                childs[i].root = this.root;
                childs[i].faceUP = true;
                childs[i].textureCoord = new Vector2[8];
                childs[i].id = this.id + i.ToString();
                childs[i].fillingOctant = false;
            }

            childs[NodePositions.xyz].bb.Min.X = bb.Min.X;
            childs[NodePositions.xyz].bb.Max.X = bb.Min.X + ((bb.Max.X - bb.Min.X) * 0.5f);
            childs[NodePositions.xyz].bb.Min.Y = bb.Min.Y;
            childs[NodePositions.xyz].bb.Max.Y = bb.Min.Y + ((bb.Max.Y - bb.Min.Y) * 0.5f);
            childs[NodePositions.xyz].bb.Min.Z = bb.Min.Z;
            childs[NodePositions.xyz].bb.Max.Z = bb.Min.Z + ((bb.Max.Z - bb.Min.Z) * 0.5f);
            childs[NodePositions.xyz].position = 0;

            childs[NodePositions.xyZ].bb.Min.X = bb.Min.X;
            childs[NodePositions.xyZ].bb.Max.X = childs[NodePositions.xyz].bb.Max.X;
            childs[NodePositions.xyZ].bb.Min.Y = bb.Min.Y;
            childs[NodePositions.xyZ].bb.Max.Y = childs[NodePositions.xyz].bb.Max.Y;
            childs[NodePositions.xyZ].bb.Min.Z = childs[NodePositions.xyz].bb.Max.Z;
            childs[NodePositions.xyZ].bb.Max.Z = bb.Max.Z;
            childs[NodePositions.xyZ].position = 1;

            childs[NodePositions.xYz].bb.Min.X = bb.Min.X;
            childs[NodePositions.xYz].bb.Max.X = childs[NodePositions.xyz].bb.Max.X;
            childs[NodePositions.xYz].bb.Min.Y = childs[NodePositions.xyz].bb.Max.Y;
            childs[NodePositions.xYz].bb.Max.Y = bb.Max.Y;
            childs[NodePositions.xYz].bb.Min.Z = bb.Min.Z;
            childs[NodePositions.xYz].bb.Max.Z = childs[NodePositions.xyz].bb.Max.Z;
            childs[NodePositions.xYz].position = 2;

            childs[NodePositions.xYZ].bb.Min.X = bb.Min.X;
            childs[NodePositions.xYZ].bb.Max.X = childs[NodePositions.xyz].bb.Max.X;
            childs[NodePositions.xYZ].bb.Min.Y = childs[NodePositions.xyz].bb.Max.Y;
            childs[NodePositions.xYZ].bb.Max.Y = bb.Max.Y;
            childs[NodePositions.xYZ].bb.Min.Z = childs[NodePositions.xyz].bb.Max.Z;
            childs[NodePositions.xYZ].bb.Max.Z = bb.Max.Z;
            childs[NodePositions.xYZ].position = 3;

            childs[NodePositions.Xyz].bb.Min.X = childs[NodePositions.xyz].bb.Max.X;
            childs[NodePositions.Xyz].bb.Max.X = bb.Max.X;
            childs[NodePositions.Xyz].bb.Min.Y = bb.Min.Y;
            childs[NodePositions.Xyz].bb.Max.Y = childs[NodePositions.xyz].bb.Max.Y;
            childs[NodePositions.Xyz].bb.Min.Z = bb.Min.Z;
            childs[NodePositions.Xyz].bb.Max.Z = childs[NodePositions.xyz].bb.Max.Z;
            childs[NodePositions.Xyz].position = 4;

            childs[NodePositions.XyZ].bb.Min.X = childs[NodePositions.xyz].bb.Max.X;
            childs[NodePositions.XyZ].bb.Max.X = bb.Max.X;
            childs[NodePositions.XyZ].bb.Min.Y = bb.Min.Y;
            childs[NodePositions.XyZ].bb.Max.Y = childs[NodePositions.xyz].bb.Max.Y;
            childs[NodePositions.XyZ].bb.Min.Z = childs[NodePositions.xyz].bb.Max.Z;
            childs[NodePositions.XyZ].bb.Max.Z = bb.Max.Z;
            childs[NodePositions.XyZ].position = 5;

            childs[NodePositions.XYz].bb.Min.X = childs[NodePositions.xyz].bb.Max.X;
            childs[NodePositions.XYz].bb.Max.X = bb.Max.X;
            childs[NodePositions.XYz].bb.Min.Y = childs[NodePositions.xyz].bb.Max.Y;
            childs[NodePositions.XYz].bb.Max.Y = bb.Max.Y;
            childs[NodePositions.XYz].bb.Min.Z = bb.Min.Z;
            childs[NodePositions.XYz].bb.Max.Z = childs[NodePositions.xyz].bb.Max.Z;
            childs[NodePositions.XYz].position = 6;

            childs[NodePositions.XYZ].bb.Min.X = childs[NodePositions.xyz].bb.Max.X;
            childs[NodePositions.XYZ].bb.Max.X = bb.Max.X;
            childs[NodePositions.XYZ].bb.Min.Y = childs[NodePositions.xyz].bb.Max.Y;
            childs[NodePositions.XYZ].bb.Max.Y = bb.Max.Y;
            childs[NodePositions.XYZ].bb.Min.Z = childs[NodePositions.xyz].bb.Max.Z;
            childs[NodePositions.XYZ].bb.Max.Z = bb.Max.Z;
            childs[NodePositions.XYZ].position = 7;

            for (short i = 0; i < 8; i++)
            {
                Octree ch = childs[i];
                ch.level = level + 1;
                CreateChild(ch, i);
            }
        }

        private void CreateChild(Octree child, short i)
        {
            child.bModel = RemoveNonIntersectingFaces(bModel, child.bb);
            child.BuildLevel();
        }

        private BModel RemoveNonIntersectingFaces(BModel bModel, BoundingBox bb)
        {
            if (bModel == null)
            {
                return null;
            }

            BModel bmodelCutted = new BModel(bModel.device);
            float[] coordBBMin = aabbint.Coords(bb.Min);
            float[] coordBBMax = aabbint.Coords(bb.Max);
            Vector3[] bbCorners = bb.GetCorners();

            bmodelCutted.oScene = new Scene();

            for (int r = 0; r < bModel.oScene.MeshCount; r++)
            {
                Mesh m = bModel.oScene.Meshes[r];
                Mesh mt = new Mesh();

                Parallel.ForEach(m.Faces, (f) =>
                {
                    if (f != null)
                    {
                        Vector3[] tri = new Vector3[3];

                        for (int i = 0; i < 3; i++)
                        {
                            tri[i] = new Vector3(root.bModel.oScene.Meshes[r].Vertices[f.Indices[i]].X,
                                root.bModel.oScene.Meshes[r].Vertices[f.Indices[i]].Y,
                                root.bModel.oScene.Meshes[r].Vertices[f.Indices[i]].Z);
                        }

                        if (aabbint.IsIntersecting(tri, coordBBMin, coordBBMax, bbCorners))
                        {
                            lock (mt)
                            {
                                mt.Faces.Add(f);
                            }
                        }
                    }
                });

                bmodelCutted.oScene.Meshes.Add(mt);
            }

            return bmodelCutted;
        }

        public TimeSpan elapsedTime()
        {
            if (startTime == null && endTime == null)
            {
                return new TimeSpan();
            }
            else if (endTime == null)
            {
                return DateTime.Now - startTime.Value;
            }
            else
            {
                return endTime.Value - startTime.Value;
            }
        }

        #region Node search functions

        public Octree FindNeighborZMinusEqualSize()
        {
            if (parent == null) return null;
            else if (position == 1) return parent.childs[NodePositions.xyz];
            else if (position == 3) return parent.childs[NodePositions.xYz];
            else if (position == 5) return parent.childs[NodePositions.Xyz];
            else if (position == 7) return parent.childs[NodePositions.XYz];
            else
            {
                Octree tmp = parent.FindNeighborZMinusEqualSize();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level < level)
                {
                    if (tmp.state == OctreeStates.Empty)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Empty;
                            tmp.childs[i].fillingOctant = true;
                        }
                    }
                    else if (tmp.state == OctreeStates.Full)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Full;
                        }
                    }

                    if (position == 0) return tmp.childs[NodePositions.xyZ];
                    else if (position == 2) return tmp.childs[NodePositions.xYZ];
                    else if (position == 4) return tmp.childs[NodePositions.XyZ];
                    else if (position == 6) return tmp.childs[NodePositions.XYZ];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborZMinus()
        {
            if (parent == null) return null;
            else if (position == 1) return parent.childs[NodePositions.xyz];
            else if (position == 3) return parent.childs[NodePositions.xYz];
            else if (position == 5) return parent.childs[NodePositions.Xyz];
            else if (position == 7) return parent.childs[NodePositions.XYz];
            else
            {
                Octree tmp = parent.FindNeighborZMinus();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level > level && tmp.state == OctreeStates.Mixted)
                {
                    if (position == 0) return tmp.childs[NodePositions.xyZ];
                    else if (position == 2) return tmp.childs[NodePositions.xYZ];
                    else if (position == 4) return tmp.childs[NodePositions.XyZ];
                    else if (position == 6) return tmp.childs[NodePositions.XYZ];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborZPlusEqualSize()
        {
            if (parent == null) return null;
            else if (position == 0) return parent.childs[NodePositions.xyZ];
            else if (position == 2) return parent.childs[NodePositions.xYZ];
            else if (position == 4) return parent.childs[NodePositions.XyZ];
            else if (position == 6) return parent.childs[NodePositions.XYZ];
            else
            {
                Octree tmp = parent.FindNeighborZPlusEqualSize();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level < level)
                {
                    if (tmp.state == OctreeStates.Empty)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Empty;
                        }
                    }
                    else if (tmp.state == OctreeStates.Full)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Full;
                        }
                    }

                    if (position == 1) return tmp.childs[NodePositions.xyz];
                    else if (position == 3) return tmp.childs[NodePositions.xYz];
                    else if (position == 5) return tmp.childs[NodePositions.Xyz];
                    else if (position == 7) return tmp.childs[NodePositions.XYz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborZPlus()
        {
            if (parent == null) return null;
            else if (position == 0) return parent.childs[NodePositions.xyZ];
            else if (position == 2) return parent.childs[NodePositions.xYZ];
            else if (position == 4) return parent.childs[NodePositions.XyZ];
            else if (position == 6) return parent.childs[NodePositions.XYZ];
            else
            {
                Octree tmp = parent.FindNeighborZPlus();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level > level && tmp.state == OctreeStates.Mixted)
                {
                    if (position == 1) return tmp.childs[NodePositions.xyz];
                    else if (position == 3) return tmp.childs[NodePositions.xYz];
                    else if (position == 5) return tmp.childs[NodePositions.Xyz];
                    else if (position == 7) return tmp.childs[NodePositions.XYz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborXPlusEqualSize()
        {
            if (parent == null) return null;
            else if (position == 1) return parent.childs[NodePositions.XyZ];
            else if (position == 3) return parent.childs[NodePositions.XYZ];
            else if (position == 0) return parent.childs[NodePositions.Xyz];
            else if (position == 2) return parent.childs[NodePositions.XYz];
            else
            {
                Octree tmp = parent.FindNeighborXPlusEqualSize();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level < level)
                {
                    if (tmp.state == OctreeStates.Empty)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Empty;
                        }
                    }
                    else if (tmp.state == OctreeStates.Full)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Full;
                        }
                    }

                    if (position == 5) return tmp.childs[NodePositions.xyZ];
                    else if (position == 7) return tmp.childs[NodePositions.xYZ];
                    else if (position == 4) return tmp.childs[NodePositions.xyz];
                    else if (position == 6) return tmp.childs[NodePositions.xYz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborXPlus()
        {
            if (parent == null) return null;
            else if (position == 1) return parent.childs[NodePositions.XyZ];
            else if (position == 3) return parent.childs[NodePositions.XYZ];
            else if (position == 0) return parent.childs[NodePositions.Xyz];
            else if (position == 2) return parent.childs[NodePositions.XYz];
            else
            {
                Octree tmp = parent.FindNeighborXPlus();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level > level && tmp.state == OctreeStates.Mixted)
                {
                    if (position == 5) return tmp.childs[NodePositions.xyZ];
                    else if (position == 7) return tmp.childs[NodePositions.xYZ];
                    else if (position == 4) return tmp.childs[NodePositions.xyz];
                    else if (position == 6) return tmp.childs[NodePositions.xYz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborXMinusEqualSize()
        {
            if (parent == null) return null;
            else if (position == 5) return parent.childs[NodePositions.xyZ];
            else if (position == 7) return parent.childs[NodePositions.xYZ];
            else if (position == 4) return parent.childs[NodePositions.xyz];
            else if (position == 6) return parent.childs[NodePositions.xYz];
            else
            {
                Octree tmp = parent.FindNeighborXMinusEqualSize();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level < level)
                {
                    if (tmp.state == OctreeStates.Empty)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Empty;
                        }
                    }
                    else if (tmp.state == OctreeStates.Full)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Full;
                        }
                    }

                    if (position == 1) return tmp.childs[NodePositions.XyZ];
                    else if (position == 3) return tmp.childs[NodePositions.XYZ];
                    else if (position == 0) return tmp.childs[NodePositions.Xyz];
                    else if (position == 2) return tmp.childs[NodePositions.XYz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborXMinus()
        {
            if (parent == null) return null;
            else if (position == 5) return parent.childs[NodePositions.xyZ];
            else if (position == 7) return parent.childs[NodePositions.xYZ];
            else if (position == 4) return parent.childs[NodePositions.xyz];
            else if (position == 6) return parent.childs[NodePositions.xYz];
            else
            {
                Octree tmp = parent.FindNeighborXMinus();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level > level && tmp.state == OctreeStates.Mixted)
                {
                    if (position == 1) return tmp.childs[NodePositions.XyZ];
                    else if (position == 3) return tmp.childs[NodePositions.XYZ];
                    else if (position == 0) return tmp.childs[NodePositions.Xyz];
                    else if (position == 2) return tmp.childs[NodePositions.XYz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborYPlusEqualSize()
        {
            if (parent == null) return null;
            else if (position == 1) return parent.childs[NodePositions.xYZ];
            else if (position == 5) return parent.childs[NodePositions.XYZ];
            else if (position == 4) return parent.childs[NodePositions.XYz];
            else if (position == 0) return parent.childs[NodePositions.xYz];
            else
            {
                Octree tmp = parent.FindNeighborYPlusEqualSize();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level < level)
                {
                    if (tmp.state == OctreeStates.Empty)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Empty;
                        }
                    }
                    else if (tmp.state == OctreeStates.Full)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Full;
                        }
                    }

                    if (position == 3) return tmp.childs[NodePositions.xyZ];
                    else if (position == 7) return tmp.childs[NodePositions.XyZ];
                    else if (position == 6) return tmp.childs[NodePositions.Xyz];
                    else if (position == 2) return tmp.childs[NodePositions.xyz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborYPlus()
        {
            if (parent == null) return null;
            else if (position == 1) return parent.childs[NodePositions.xYZ];
            else if (position == 5) return parent.childs[NodePositions.XYZ];
            else if (position == 4) return parent.childs[NodePositions.XYz];
            else if (position == 0) return parent.childs[NodePositions.xYz];
            else
            {
                Octree tmp = parent.FindNeighborYPlus();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level > level && tmp.state == OctreeStates.Mixted)
                {
                    if (position == 3) return tmp.childs[NodePositions.xyZ];
                    else if (position == 7) return tmp.childs[NodePositions.XyZ];
                    else if (position == 6) return tmp.childs[NodePositions.Xyz];
                    else if (position == 2) return tmp.childs[NodePositions.xyz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborYMinusEqualSize()
        {
            if (parent == null) return null;
            else if (position == 3) return parent.childs[NodePositions.xyZ];
            else if (position == 7) return parent.childs[NodePositions.XyZ];
            else if (position == 6) return parent.childs[NodePositions.Xyz];
            else if (position == 2) return parent.childs[NodePositions.xyz];
            else
            {
                Octree tmp = parent.FindNeighborYMinusEqualSize();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level < level)
                {
                    if (tmp.state == OctreeStates.Empty)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Empty;
                        }
                    }
                    else if (tmp.state == OctreeStates.Full)
                    {
                        tmp.state = OctreeStates.Mixted;
                        tmp.CreateChilds(tmp.bb);
                        for (int i = 0; i < 8; i++)
                        {
                            tmp.childs[i].state = OctreeStates.Full;
                        }
                    }

                    if (position == 3) return tmp.childs[NodePositions.xyZ];
                    else if (position == 5) return tmp.childs[NodePositions.XYZ];
                    else if (position == 4) return tmp.childs[NodePositions.XYz];
                    else if (position == 0) return tmp.childs[NodePositions.xYz];
                    else return null;
                }
                else return tmp;
            }
        }

        public Octree FindNeighborYMinus()
        {
            if (parent == null) return null;
            else if (position == 3) return parent.childs[NodePositions.xyZ];
            else if (position == 7) return parent.childs[NodePositions.XyZ];
            else if (position == 6) return parent.childs[NodePositions.Xyz];
            else if (position == 2) return parent.childs[NodePositions.xyz];
            else
            {
                Octree tmp = parent.FindNeighborYMinus();
                if (tmp == null || tmp.level == 0) return null;
                else if (tmp.level > level && tmp.state == OctreeStates.Mixted)
                {
                    if (position == 3) return tmp.childs[NodePositions.xyZ];
                    else if (position == 5) return tmp.childs[NodePositions.XYZ];
                    else if (position == 4) return tmp.childs[NodePositions.XYz];
                    else if (position == 0) return tmp.childs[NodePositions.xYz];
                    else return null;
                }
                else return tmp;
            }
        }

        #endregion

        #region Operations

        public Octree Intersection(Octree B)
        {
            return Intersection(this, B);
        }

        protected Octree Intersection(Octree A, Octree B)
        {
            Octree result = new Octree(game);
            result.root = A.root;
            result.bb = A.bb;
            result.level = A.level;
            result.parent = A.parent;

            if (A.state == OctreeStates.Full && B.state == OctreeStates.Full)
            {
                result.state = OctreeStates.Full;
            }else if (A.state == OctreeStates.Mixted && B.state == OctreeStates.Mixted)
            {
                result.state = OctreeStates.Mixted;
                result.CreateChilds(A.bb);
                for (int c = 0; c < 8; c++)
                {
                    result.childs[c] = result.childs[c].Intersection(A.childs[c], B.childs[c]);
                }
            }else if (A.state == OctreeStates.Full && B.state == OctreeStates.Mixted)
            {
                result.state = OctreeStates.Mixted;
                result.CreateChilds(A.bb);
                for (int c = 0; c < 8; c++)
                {
                    result.childs[c] = result.childs[c].Intersection(B.childs[c], B.childs[c]);
                }
            }
            else if (B.state == OctreeStates.Full && A.state == OctreeStates.Mixted)
            {
                result.state = OctreeStates.Mixted;
                result.CreateChilds(A.bb);
                for (int c = 0; c < 8; c++)
                {
                    result.childs[c] = result.childs[c].Intersection(A.childs[c], A.childs[c]);
                }
            }

            return result;
        }

        public Octree Substract(Octree B)
        {
            return Substract(this, B);
        }

        protected Octree Substract(Octree A, Octree B)
        {
            Octree result = new Octree(game);
            result.root = A.root;
            result.bb = A.bb;
            result.level = A.level;
            result.parent = A.parent;

            if (A.state == OctreeStates.Empty)
            {
                result.state = OctreeStates.Empty;
            }else if(B.state == OctreeStates.Full)
            {
                result.state = OctreeStates.Empty;
            }else if(A.state == OctreeStates.Full && B.state == OctreeStates.Empty)
            {
                result.state = OctreeStates.Full;
            }else if(A.state == OctreeStates.Mixted && B.state == OctreeStates.Mixted)
            {
                result.state = OctreeStates.Mixted;
                result.CreateChilds(A.bb);
                for (int c = 0; c < 8; c++)
                {
                    result.childs[c] = result.childs[c].Substract(A.childs[c], B.childs[c]);
                }
            }else if(A.state == OctreeStates.Mixted && B.state == OctreeStates.Empty)
            {
                result = A.Union(A);
            }
            else if (A.state == OctreeStates.Full && B.state == OctreeStates.Mixted)
            {
                result = B.Reverse();
            }

            return result;
        }

        public Octree Union(Octree B)
        {
            return Union(this, B);
        }

        protected Octree Union(Octree A, Octree B)
        {
            Octree result = new Octree(game);
            result.root = A.root;
            result.bb = A.bb;
            result.level = A.level;
            result.parent = A.parent;

            if (A.state == OctreeStates.Full)
            {
                result.state = OctreeStates.Full;
            }
            else if (B.state == OctreeStates.Full)
            {
                result.state = OctreeStates.Full;
            }
            else if (A.state == OctreeStates.Mixted)
            {
                result.state = OctreeStates.Mixted;
                result.CreateChilds(A.bb);
                for (int c = 0; c < 8; c++)
                {
                    if (B.state == OctreeStates.Empty)
                    {
                        result.childs[c] = result.childs[c].Union(A.childs[c], B);
                    }
                    else
                    {
                        result.childs[c] = result.childs[c].Union(A.childs[c], B.childs[c]);
                    }
                }

            }
            else if (B.state == OctreeStates.Mixted)
            {
                result.state = OctreeStates.Mixted;
                result.CreateChilds(B.bb);
                for (int c = 0; c < 8; c++)
                {
                    if (A.state == OctreeStates.Empty)
                    {
                        result.childs[c] = result.childs[c].Union(A, B.childs[c]);
                    }
                    else
                    {
                        result.childs[c] = result.childs[c].Union(A.childs[c], B.childs[c]);
                    }
                }
            }

            return result;
        }

        public Octree Reverse()
        {
            return Reverse(this);
        }

        protected Octree Reverse(Octree A)
        {
            Octree result = new Octree(game);
            result.root = A.root;
            result.bb = A.bb;
            result.level = A.level;
            result.parent = A.parent;

            switch (A.state)
            {
                case OctreeStates.Full:
                    result.state = OctreeStates.Empty;
                    break;
                case OctreeStates.Empty:
                    result.state = OctreeStates.Full;
                    break;
                case OctreeStates.Mixted:
                    result.state = OctreeStates.Mixted;
                    result.CreateChilds(A.bb);
                    for (int c = 0; c < 8; c++)
                    {
                        result.childs[c] = result.childs[c].Reverse(A.childs[c]);
                    }
                    break;
            }

            return result;
        }

        #endregion
    }
}
