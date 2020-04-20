﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection;

namespace OctGL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        FrameCounter frameCounter = new FrameCounter();

        UI ui;

        public Axis axis;
        public bool showAxis;
        public string projection;
        public Boundary boundary;
        public bool showBoundary;
        public bool showModelNormals;
        public bool wireframe;

        //Camera
        public Camera camera;
        Matrix perspectiveMatrix;
        Matrix ortographicMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        public GraphicsDeviceManager _graphics;
        public int originalWidth;
        public int originalHeight;

        private MouseState oldState;

        //BasicEffect for rendering
        BasicEffect basicEffect;

        public Texture2D textureCrate;

        public BModel bModel;
        public short octreeDepth;
        public short octantTextureCoordinates;
        public bool optimizeOctantFaces;
        public bool optimizeOctree;
        public string fillDirection;
        public Octree octree;
        
        public bool showModel;
        public bool showOctree;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            graphics.ApplyChanges();
            _graphics = graphics;
            originalWidth = graphics.PreferredBackBufferWidth;
            originalHeight = graphics.PreferredBackBufferHeight;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            ui = new UI(this);
            projection = "P";

            octreeDepth = 4;
            octantTextureCoordinates = 1;
            optimizeOctantFaces = true;
            optimizeOctree = true;
            fillDirection = "Z-";

            octree = new Octree(this);
            
            showModel = true;
            showOctree = true;
            showModelNormals = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            //Setup Camera
            camera = new Camera();

            perspectiveMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               GraphicsDevice.DisplayMode.AspectRatio,
                               1f, 10000f);
            ortographicMatrix = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width*(float)camera.distance*0.001f, GraphicsDevice.Viewport.Height * (float)camera.distance * 0.001f, 1f, 100f);

            worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            axis = new Axis(2f);
            showAxis = true;

            boundary = new Boundary(new BoundingBox());
            showBoundary = true;
            wireframe = false;

            //BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.AmbientLightColor = new Vector3(0.0f, 1.0f, 0.0f);

            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            basicEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            basicEffect.LightingEnabled = true;

            bModel = new BModel(GraphicsDevice);

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Window.Title = "OctGL v" + version;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            using (var stream = TitleContainer.OpenStream("Content/crate.jpg"))
            {
                textureCrate = Texture2D.FromStream(this.GraphicsDevice, stream);
            }

            ui.CreateUI();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (!Myra.Graphics2D.UI.Desktop.IsMouseOverGUI)
            {
                MouseState newState = Mouse.GetState();
                if (newState.LeftButton == ButtonState.Pressed && newState.Y != oldState.Y)
                {
                    camera.rotationv += newState.Y - oldState.Y;
                }
                if (newState.LeftButton == ButtonState.Pressed && newState.X != oldState.X)
                {
                    camera.rotationh -= newState.X - oldState.X;
                }
                if (newState.RightButton == ButtonState.Pressed && newState.Y != oldState.Y)
                {
                    camera.distance += (camera.distance * 0.10f) * ((newState.Y - oldState.Y) / 10.0f);

                    if (camera.distance < 0)
                        camera.distance = 0f;

                    if (projection == "O")
                        ortographicMatrix = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width * (float)camera.distance * 0.001f, GraphicsDevice.Viewport.Height * (float)camera.distance * 0.001f, 1f, 10000f);
                }
                oldState = newState;

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                    ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                    Keys.Escape))
                    Exit();

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    camera.rotationh += Camera.rotationSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    camera.rotationh -= Camera.rotationSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    camera.rotationv -= Camera.rotationSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    camera.rotationv += Camera.rotationSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                {
                    if (projection == "P") camera.distance += Camera.distanceSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
                {
                    if (projection == "P") camera.distance -= Camera.distanceSpeed;
                }

                viewMatrix = camera.ViewMatrix();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            frameCounter.Update(deltaTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (projection=="P")
                basicEffect.Projection = perspectiveMatrix;
            else
                basicEffect.Projection = ortographicMatrix;

            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;
            
            RasterizerState rasterizerStateWF = new RasterizerState();
            rasterizerStateWF.FillMode = FillMode.WireFrame;
            rasterizerStateWF.CullMode = CullMode.None;
            rasterizerStateWF.ScissorTestEnable = true;
            GraphicsDevice.RasterizerState = rasterizerStateWF;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                basicEffect.TextureEnabled = false;
                basicEffect.VertexColorEnabled = true;
                basicEffect.LightingEnabled = false;
                pass.Apply();

                if (showAxis)
                {
                    axis.RenderToDevice(GraphicsDevice, basicEffect, pass);
                }

                if (showBoundary)
                {
                    boundary.RenderToDevice(GraphicsDevice, basicEffect, pass);
                }
            }

            if (showModelNormals && bModel.oScene != null)
            {
                bModel.RenderToDeviceNormals(basicEffect, GraphicsDevice);
            }

            RasterizerState rasterizerStateSolid = new RasterizerState();
            rasterizerStateSolid.FillMode = FillMode.Solid;
            rasterizerStateSolid.CullMode = CullMode.None;
            rasterizerStateSolid.ScissorTestEnable = true;
            GraphicsDevice.RasterizerState = rasterizerStateSolid;

            basicEffect.VertexColorEnabled = false;
            basicEffect.TextureEnabled = true;
            basicEffect.LightingEnabled = true;
            basicEffect.EnableDefaultLighting();
            basicEffect.Parameters["Texture"].SetValue(textureCrate);

            if (showModel && bModel.oScene != null)
            {
                bModel.RenderToDevice(basicEffect);
            }

            if (wireframe)
            {
                GraphicsDevice.RasterizerState = rasterizerStateWF;

                basicEffect.TextureEnabled = false;
                basicEffect.VertexColorEnabled = true;
                basicEffect.LightingEnabled = false;
            }

            if (showOctree && octree != null)
            {
                octree.RenderToDevice(basicEffect, GraphicsDevice, wireframe);
            }

            ui.txtFPS.Text = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond.ToString("000.00"));
            ui.txtRotationH.Text = string.Format("H: {0}", camera.rotationh.ToString("000."));
            ui.txtRotationV.Text = string.Format("V: {0}", camera.rotationv.ToString("000."));
            ui.txtDistance.Text = string.Format("D: {0}", camera.distance.ToString("00.00"));
            ui.txtOctreeBuild.Text = string.Format("Octants: {0} of {1}", octree.octants, octree.octantsMax);
            ui.txtOctreeTextureCoordinates.Text = string.Format("Texture coordinates: {0} of {1}", octree.textureCoordinates, octree.textureCoordinatesMax);
            ui.txtOctreeVertices.Text = string.Format("Vertices: {0}", octree.verticesNumber);
            ui.txtOctreeElapsedTime.Text = string.Format("{0}ms", octree.elapsedTime().TotalMilliseconds.ToString("0."));
            ui.Render();

            base.Draw(gameTime);
        }
    }
}
