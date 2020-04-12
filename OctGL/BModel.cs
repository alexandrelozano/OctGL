using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Assimp;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System;

namespace OctGL
{
    public class BModel
    {
        public Scene oScene;

        public BoundingBox bb;
        public Texture2D tex;

        VertexBuffer[] vertexBuffers;
        IndexBuffer[] indexBuffers;

        public GraphicsDevice device;

        public BModel(GraphicsDevice device)
        {
            this.device = device;
        }

        public String Load(string file)
        {
            string result  = "";

            bb.Max = Vector3.Zero;
            bb.Min = Vector3.Zero;

            var ctx = new Assimp.AssimpContext();

            var conf = new Assimp.Configs.MeshVertexLimitConfig(ushort.MaxValue);

            ctx.SetConfig(conf);

            try
            {
                oScene = ctx.ImportFile(file,
                    PostProcessSteps.GenerateNormals |
                    PostProcessSteps.FindInstances |
                    PostProcessSteps.FindInvalidData |
                    PostProcessSteps.FlipUVs |
                    PostProcessSteps.JoinIdenticalVertices |
                    PostProcessSteps.ImproveCacheLocality |
                    PostProcessSteps.OptimizeMeshes |
                    PostProcessSteps.OptimizeGraph |
                    PostProcessSteps.RemoveRedundantMaterials |
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.SplitLargeMeshes
                );

                SetUpVertices();

                if (oScene.MaterialCount > 0)
                {
                    var lstMtl = oScene.Materials[0].GetAllMaterialTextures();

                    if (lstMtl != null && lstMtl.Length > 0)
                        tex = LoadTextureStream(string.Concat(Path.GetDirectoryName(file), "\\", lstMtl[0].FilePath));
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }

        private Texture2D LoadTextureStream(string filePath)
        {

            Texture2D file = null;
            Texture2D resultTexture;
            RenderTarget2D result = null;

            try
            {
                if (Path.GetExtension(filePath).ToUpper() == ".BMP")
                {
                    var qualityEncoder = Encoder.Quality;
                    var quality = (long)100;
                    var ratio = new EncoderParameter(qualityEncoder, quality);
                    var codecParams = new EncoderParameters(1);
                    codecParams.Param[0] = ratio;

                    ImageCodecInfo jpegCodecInfo = null;
                    var imgEnc = ImageCodecInfo.GetImageEncoders();
                    for (int i = 0; i < imgEnc.Length; i++)
                    {
                        if (imgEnc[i].MimeType == "image/jpeg")
                        {
                            jpegCodecInfo = imgEnc[i];
                        }
                    }

                    var bmp = new Bitmap(filePath);

                    filePath = filePath.ToUpper().Replace(".BMP", ".JPG");
                    bmp.Save(filePath, jpegCodecInfo, codecParams);
                }

                using (Stream titleStream = System.IO.File.Open(filePath, System.IO.FileMode.Open))
                {
                    file = Texture2D.FromStream(device, titleStream);
                }
            }
            catch(Exception e)
            {
                throw new System.IO.FileLoadException("Cannot load '" + filePath + "' file! " + e.Message);
            }
            PresentationParameters pp = device.PresentationParameters;

            //Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(device, file.Width, file.Height, true, pp.BackBufferFormat, pp.DepthStencilFormat);

            device.SetRenderTarget(result);
            device.Clear(Microsoft.Xna.Framework.Color.Black);

            //Multiply each color by the source alpha, and write in just the color values into the final texture
            BlendState blendColor = new BlendState();
            blendColor.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;

            blendColor.AlphaDestinationBlend = Blend.Zero;
            blendColor.ColorDestinationBlend = Blend.Zero;

            blendColor.AlphaSourceBlend = Blend.SourceAlpha;
            blendColor.ColorSourceBlend = Blend.SourceAlpha;

            SpriteBatch spriteBatch = new SpriteBatch(device);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
            spriteBatch.Draw(file, file.Bounds, Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();

            //Now copy over the alpha values from the PNG source texture to the final one, without multiplying them
            BlendState blendAlpha = new BlendState();
            blendAlpha.ColorWriteChannels = ColorWriteChannels.Alpha;

            blendAlpha.AlphaDestinationBlend = Blend.Zero;
            blendAlpha.ColorDestinationBlend = Blend.Zero;

            blendAlpha.AlphaSourceBlend = Blend.One;
            blendAlpha.ColorSourceBlend = Blend.One;

            spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
            spriteBatch.Draw(file, file.Bounds, Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();

            //Release the GPU back to drawing to the screen
            device.SetRenderTarget(null);

            resultTexture = new Texture2D(device, result.Width, result.Height);
            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[result.Height * result.Width];
            Microsoft.Xna.Framework.Color[] textureColor = new Microsoft.Xna.Framework.Color[result.Height * result.Width];

            result.GetData<Microsoft.Xna.Framework.Color>(textureColor);

            for (int i = 0; i < result.Height; i++)
            {
                for (int j = 0; j < result.Width; j++)
                {
                    data[j + i * result.Width] = textureColor[j + i * result.Width];
                }
            }

            resultTexture.SetData(data);

            return resultTexture;
        }

        private void SetUpVertices()
        {
            int count = oScene.MeshCount;
            vertexBuffers = new VertexBuffer[count];
            indexBuffers = new IndexBuffer[count];
            VertexPositionNormalTexture[] vertices;
            short[] indices;
            Mesh mMesh;

            CheckBoundary();
            Center();
            CheckBoundary();

            for (int m = 0; m < count; m++)
            {
                mMesh = oScene.Meshes[m];
                vertices = new VertexPositionNormalTexture[mMesh.VertexCount];
                indices = new short[mMesh.FaceCount * 3];

                for (int v = 0; v< mMesh.VertexCount; v++)
                {
                    var mVec = mMesh.Vertices[v];
                    var mNor = mMesh.Normals[v];
                    var mTex = Vector2.Zero;

                    if (mMesh.HasTextureCoords(0))
                    {
                        mTex = new Vector2(mMesh.TextureCoordinateChannels[0][v].X, mMesh.TextureCoordinateChannels[0][v].Y) ;
                    }

                    vertices[v] = new VertexPositionNormalTexture(new Vector3(mVec.X, mVec.Y, mVec.Z), 
                        new Vector3(mNor.X, mNor.Y, mNor.Z), mTex);
                }

                int f = 0;
                int i = 0;
                Face mFace;
                for (i = 0; i < mMesh.FaceCount * 3; i = i + 3)
                {
                    mFace = mMesh.Faces[f];
                    if (mFace.IndexCount != 3)
                    {
                        f = f + 1 - 1;
                    }
                    f++;
                    indices[i] = (short)mFace.Indices[0];
                    indices[i + 1] = (short)mFace.Indices[1];
                    indices[i + 2] = (short)mFace.Indices[2];
                }
                
                var vertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData(vertices);
                
                var indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.None);
                indexBuffer.SetData(indices);

                vertexBuffers[m] = vertexBuffer;
                indexBuffers[m] = indexBuffer;
            }

        }

        public void Center()
        {
            Mesh mMesh;
            for (int m = 0; m < oScene.MeshCount; m++)
            {
                mMesh = oScene.Meshes[m];

                for (int v = 0; v < mMesh.VertexCount; v++)
                {
                    var mVec = mMesh.Vertices[v];

                    mVec.X -= bb.Min.X + ((bb.Max.X - bb.Min.X) / 2.0f);
                    mVec.Y -= bb.Min.Y + ((bb.Max.Y - bb.Min.Y) / 2.0f);
                    mVec.Z -= bb.Min.Z + ((bb.Max.Z - bb.Min.Z) / 2.0f);

                    mMesh.Vertices[v] = mVec;
                }

                oScene.Meshes[m] = mMesh;
            }
        }

        public void CheckBoundary()
        {
            bool firstTime = true;

            Mesh mMesh;
            for (int m = 0; m < oScene.MeshCount; m++)
            {
                mMesh = oScene.Meshes[m];

                for (int v = 0; v < mMesh.VertexCount; v++)
                {
                    var mVec = mMesh.Vertices[v];

                    if (firstTime)
                    {
                        bb.Max = new Vector3(mVec.X, mVec.Y, mVec.Z);
                        bb.Min = new Vector3(mVec.X, mVec.Y, mVec.Z);
                        firstTime = false;
                    }

                    if (mVec.X > bb.Max.X) bb.Max.X = mVec.X;
                    if (mVec.Y > bb.Max.Y) bb.Max.Y = mVec.Y;
                    if (mVec.Z > bb.Max.Z) bb.Max.Z = mVec.Z;

                    if (mVec.X < bb.Min.X) bb.Min.X = mVec.X;
                    if (mVec.Y < bb.Min.Y) bb.Min.Y = mVec.Y;
                    if (mVec.Z < bb.Min.Z) bb.Min.Z = mVec.Z;
                }
            }

        }

        public void RenderToDevice(BasicEffect effect)
        {

            for (int i = 0; i < vertexBuffers.Length; i++)
            {
                device.SetVertexBuffer(vertexBuffers[i]);
                device.Indices = indexBuffers[i];

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, indexBuffers[i].IndexCount);
                }
            }

        }

        public long totalVertices()
        {
            long totalV = 0;
            if (oScene != null)
            {
                for (int i = 0; i < oScene.MeshCount; i++)
                {
                    totalV += oScene.Meshes[i].VertexCount;
                }
            }

            return totalV;
        }

        public long totalFaces()
        {
            long totalF = 0;
            if (oScene != null)
            {
                for (int i = 0; i < oScene.MeshCount; i++)
                {
                    totalF += oScene.Meshes[i].FaceCount;
                }
            }

            return totalF;
        }

    }
}
