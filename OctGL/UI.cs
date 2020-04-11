using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System;
using System.Threading;

namespace OctGL
{
    class UI
    {
        Game1 game;

        public TextBox txtFPS;
        public TextBox txtRotationV;
        public TextBox txtRotationH;
        public TextBox txtDistance;
        public TextBox txtOctreeBuild;
        public TextBox txtOctreeTextureCoordinates;
        public TextBox txtOctreeVertices;
        public TextBox txtOctreeElapsedTime;

        public UI(Game1 game)
        {
            this.game = game;

            MyraEnvironment.Game = game;
        }

        public void Render()
        {
            Desktop.Render();
        }

        public void CreateUI()
        {
            var horizontalBox = new Myra.Graphics2D.UI.HorizontalStackPanel();
            horizontalBox.ShowGridLines = true;
            horizontalBox.Spacing = 8;
            horizontalBox.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
            horizontalBox.VerticalAlignment = VerticalAlignment.Bottom;
            horizontalBox.Background = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Black);

            txtFPS = new TextBox();
            horizontalBox.Widgets.Add(txtFPS);

            txtRotationH = new TextBox();
            horizontalBox.Widgets.Add(txtRotationH);

            txtRotationV = new TextBox();
            horizontalBox.Widgets.Add(txtRotationV);

            txtDistance = new TextBox();
            horizontalBox.Widgets.Add(txtDistance);

            txtOctreeBuild = new TextBox();
            horizontalBox.Widgets.Add(txtOctreeBuild);

            txtOctreeTextureCoordinates = new TextBox();
            horizontalBox.Widgets.Add(txtOctreeTextureCoordinates);

            txtOctreeVertices = new TextBox();
            horizontalBox.Widgets.Add(txtOctreeVertices);

            txtOctreeElapsedTime = new TextBox();
            horizontalBox.Widgets.Add(txtOctreeElapsedTime);

            horizontalBox.Proportions.Add(new Proportion());
            horizontalBox.Proportions.Add(new Proportion());
            horizontalBox.Proportions.Add(new Proportion());
            horizontalBox.Proportions.Add(new Proportion());
            horizontalBox.Proportions.Add(new Proportion());
            horizontalBox.Proportions.Add(new Proportion());
            horizontalBox.Proportions.Add(new Proportion());
            horizontalBox.Proportions.Add(new Proportion
            {
                Type = ProportionType.Fill,
            });

            var _menuFile = new MenuItem();
            _menuFile.Id = "_menuFile";
            _menuFile.Text = "File";

            _menuFile.Items.Add(CreateBuildFromModel());

            var _mnuOpenOctree = new MenuItem();
            _mnuOpenOctree.Id = "_menuOpenOctree";
            _mnuOpenOctree.Text = "Open octree...";
            _mnuOpenOctree.Selected += (s, a) =>
            {
                var ofd = new FileDialog(FileDialogMode.OpenFile);
                ofd.Folder = "d:\\develop\\NET\\Monogame\\Models\\";
                ofd.Filter = "*.oct";
                ofd.ShowModal();
                ofd.Closed += (s1, a1) =>
                {
                    if (ofd.FilePath != "" && ofd.Result)
                    {
                        game.octree.startTime = DateTime.Now;
                        game.bModel = new BModel(game.GraphicsDevice);
                        game.octree.OpenOctree(ofd.FilePath);
                        game.octree.BuildMesh();
                        game.camera.rotationh = 0f;
                        game.camera.rotationv = 0f;
                        game.camera.distance = game.octree.bb.Max.Length() * 3;
                        game.axis.size = game.octree.bb.Max.Length();
                        game.boundary.bb = game.octree.bb;
                        game.octree.endTime = DateTime.Now;
                    }
                };
            };
            _menuFile.Items.Add(_mnuOpenOctree);

            var _mnuSaveOctree = new MenuItem();
            _mnuSaveOctree.Id = "_menuSaveOctree";
            _mnuSaveOctree.Text = "Save octree...";
            _mnuSaveOctree.Selected += (s, a) =>
            {
                var ofd = new FileDialog(FileDialogMode.SaveFile);
                ofd.FilePath = "d:\\develop\\NET\\Monogame\\Models\\";
                ofd.Filter = "*.oct";
                ofd.AutoAddFilterExtension = true;
                ofd.ShowModal();

                ofd.Closed += (s1, a1) =>
                {
                    if (ofd.FilePath != "" && ofd.Result)
                    {
                        game.octree.SaveOctree(ofd.FilePath);
                    }
                };
                
            };
            _menuFile.Items.Add(_mnuSaveOctree);

            var _menuOptions = new MenuItem();
            _menuOptions.Id = "_menuOptions";
            _menuOptions.Text = "Options";

            _menuOptions.Items.Add(CreateAxisOptions());

            var _menuView = new MenuItem();
            _menuView.Id = "_menuView";
            _menuView.Text = "View";

            var _mnuShowModel = new MenuItem();
            _mnuShowModel.Id = "_mnuShowModel";
            _mnuShowModel.Text = "Hide model";
            _mnuShowModel.Selected += (s, a) =>
            {
                if (_mnuShowModel.Text == "Show model")
                {
                    _mnuShowModel.Text = "Hide model";
                    game.showModel = true;
                }
                else
                {
                    _mnuShowModel.Text = "Show model";
                    game.showModel = false;
                }
            };
            _menuView.Items.Add(_mnuShowModel);

            var _mnuShowOctree = new MenuItem();
            _mnuShowOctree.Id = "_mnuShowOctree";
            _mnuShowOctree.Text = "Hide octree";
            _mnuShowOctree.Selected += (s, a) =>
            {
                if (_mnuShowOctree.Text == "Show octree")
                {
                    _mnuShowOctree.Text = "Hide octree";
                    game.showOctree = true;
                }
                else
                {
                    _mnuShowOctree.Text = "Show octree";
                    game.showOctree = false;
                }
            };
            _menuView.Items.Add(_mnuShowOctree);

            var _mnuAxis = new MenuItem();
            _mnuAxis.Id = "_menuAxis";
            _mnuAxis.Text = "Hide axis";
            _mnuAxis.Selected += (s, a) =>
            {
                if (_mnuAxis.Text == "Show axis")
                {
                    _mnuAxis.Text = "Hide axis";
                    game.showAxis = true;
                }
                else
                {
                    _mnuAxis.Text = "Show axis";
                    game.showAxis = false;
                }
            };
            _menuView.Items.Add(_mnuAxis);

            var _menuBoundary = new MenuItem();
            _menuBoundary.Id = "_menuBoundary";
            _menuBoundary.Text = "Hide boundary";
            _menuBoundary.Selected += (s, a) =>
            {
                if (_menuBoundary.Text == "Show boundary")
                {
                    _menuBoundary.Text = "Hide boundary";
                    game.showBoundary = true;
                }
                else
                {
                    _menuBoundary.Text = "Show boundary";
                    game.showBoundary = false;
                }
            };
            _menuView.Items.Add(_menuBoundary);

            _menuView.Items.Add(CreateModelInfo());

            var _menuProjection = new MenuItem();
            _menuProjection.Id = "_menuRenderMode";
            _menuProjection.Text = "Ortographic";
            _menuProjection.Selected += (s, a) =>
            {
                if (_menuProjection.Text == "Ortographic")
                {
                    _menuProjection.Text = "Perspective";
                    game.projection = "O";
                }
                else
                {
                    _menuProjection.Text = "Ortographic";
                    game.projection = "P";
                }
            };
            _menuView.Items.Add(_menuProjection);

            var _menuFullScreen = new MenuItem();
            _menuFullScreen.Id = "_menuFullScreen";
            _menuFullScreen.Text = "Fullscreen On";
            _menuFullScreen.Selected += (s, a) =>
            {
                if (_menuFullScreen.Text == "Fullscreen On")
                {
                    _menuFullScreen.Text = "Fullscreen Off";
                    game._graphics.PreferredBackBufferWidth = game.GraphicsDevice.DisplayMode.Width;
                    game._graphics.PreferredBackBufferHeight = game.GraphicsDevice.DisplayMode.Height;
                    game._graphics.IsFullScreen = true;
                    game._graphics.ApplyChanges();
                }
                else
                {
                    _menuFullScreen.Text = "Fullscreen On";
                    game._graphics.PreferredBackBufferWidth = game.originalWidth;
                    game._graphics.PreferredBackBufferHeight = game.originalHeight;
                    game._graphics.IsFullScreen = false;
                    game._graphics.ApplyChanges();
                }
            };
            _menuView.Items.Add(_menuFullScreen);

            var _mnuStatusBar = new MenuItem();
            _mnuStatusBar.Id = "_menuStatusBar";
            _mnuStatusBar.Text = "Hide status bar";
            _mnuStatusBar.Selected += (s, a) =>
            {
                if (_mnuStatusBar.Text == "Show status bar")
                {
                    _mnuStatusBar.Text = "Hide status bar";
                    horizontalBox.Visible = true;
                }
                else
                {
                    _mnuStatusBar.Text = "Show status bar";
                    horizontalBox.Visible = false;
                }
            };
            _menuView.Items.Add(_mnuStatusBar);

            var verticalMenu1 = new HorizontalMenu();
            verticalMenu1.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
            verticalMenu1.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Top;
            verticalMenu1.Items.Add(_menuFile);
            verticalMenu1.Items.Add(_menuOptions);
            verticalMenu1.Items.Add(_menuView);

            Desktop.Widgets.Add(verticalMenu1);
            Desktop.Widgets.Add(horizontalBox);
        }

        public MenuItem CreateBuildFromModel()
        {
            var _mnuBuildFromModel = new MenuItem();
            _mnuBuildFromModel.Id = "_mnuBuildFromModel";
            _mnuBuildFromModel.Text = "Build from model...";
            _mnuBuildFromModel.Selected += (s, a) =>
            {
                var _window = new Window();
                _window.Title = "Build from model";
                _window.Width = 550;

                var grid = new Grid
                {
                    ShowGridLines = false,
                    ColumnSpacing = 8,
                    RowSpacing = 8,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                _window.Content = grid;

                // Set partitioning configuration
                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Auto,
                });
                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Auto,
                });
                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Auto,
                });
                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Fill,
                });

                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());

                // Add widgets
                var lblSize = new Label();
                lblSize.Text = "Depth";
                lblSize.GridRow = 0;
                lblSize.GridColumn = 0;
                grid.Widgets.Add(lblSize);

                var depth = new SpinButton();
                depth.HorizontalAlignment = HorizontalAlignment.Stretch;
                depth.Maximum = 10;
                depth.Minimum = 1;
                depth.DecimalPlaces = 0;
                depth.Increment = 1;
                depth.GridRow = 0;
                depth.GridColumn = 1;
                depth.Width = 50;
                depth.Value = game.octreeDepth;
                depth.ValueChanged += (s1, a1) =>
                {
                    game.octreeDepth = (short)depth.Value;
                };
                grid.Widgets.Add(depth);

                var lblOctantTextureCoordinates = new Label();
                lblOctantTextureCoordinates.Text = "Octant texture coordinates";
                lblOctantTextureCoordinates.GridRow = 1;
                lblOctantTextureCoordinates.GridColumn = 0;
                grid.Widgets.Add(lblOctantTextureCoordinates);

                var comboTextureCoodinates = new ComboBox
                {
                    GridColumn = 1,
                    GridRow = 1
                };
                
                comboTextureCoodinates.Items.Add(new ListItem("0"));
                comboTextureCoodinates.Items.Add(new ListItem("1"));
                comboTextureCoodinates.Items.Add(new ListItem("8"));
                for (int i=0;i<comboTextureCoodinates.Items.Count-1;i++)
                {
                    if (comboTextureCoodinates.Items[i].Text == game.octantTextureCoordinates.ToString())
                    {
                        comboTextureCoodinates.Items[i].IsSelected = true;
                    }
                }
                comboTextureCoodinates.SelectedIndexChanged += (s1, a1) =>
                {
                    game.octantTextureCoordinates = short.Parse(comboTextureCoodinates.SelectedItem.Text);
                };
                comboTextureCoodinates.Width = 50;
                grid.Widgets.Add(comboTextureCoodinates);

                var lblFillObject = new Label();
                lblFillObject.Text = "Fill object";
                lblFillObject.GridRow = 3;
                lblFillObject.GridColumn = 0;
                grid.Widgets.Add(lblFillObject);

                var chkFillObject = new CheckBox();
                chkFillObject.GridRow = 3;
                chkFillObject.GridColumn = 1;
                chkFillObject.IsPressed = game.fillObject;
                chkFillObject.Click += (s1, a1) =>
                {
                    game.fillObject = chkFillObject.IsPressed;
                };
                grid.Widgets.Add(chkFillObject);

                var lblOptimizeOctree = new Label();
                lblOptimizeOctree.Text = "Optimize octree";
                lblOptimizeOctree.GridRow = 4;
                lblOptimizeOctree.GridColumn = 0;
                grid.Widgets.Add(lblOptimizeOctree);

                var chkOptimizeOctree = new CheckBox();
                chkOptimizeOctree.GridRow = 4;
                chkOptimizeOctree.GridColumn = 1;
                chkOptimizeOctree.IsPressed = game.optimizeOctantFaces;
                chkOptimizeOctree.Click += (s1, a1) =>
                {
                    game.optimizeOctree = chkOptimizeOctree.IsPressed;
                };
                grid.Widgets.Add(chkOptimizeOctree);

                var lblOptimizeOctantFaces = new Label();
                lblOptimizeOctantFaces.Text = "Optimize octant faces";
                lblOptimizeOctantFaces.GridRow = 5;
                lblOptimizeOctantFaces.GridColumn = 0;
                grid.Widgets.Add(lblOptimizeOctantFaces);

                var chkOptimizeOctantFaces = new CheckBox();
                chkOptimizeOctantFaces.GridRow = 5;
                chkOptimizeOctantFaces.GridColumn = 1;
                chkOptimizeOctantFaces.IsPressed = game.optimizeOctantFaces;
                chkOptimizeOctantFaces.Click += (s1, a1) =>
                {
                    game.optimizeOctantFaces = chkOptimizeOctantFaces.IsPressed;
                };
                grid.Widgets.Add(chkOptimizeOctantFaces);

                var lblFile = new Label();
                lblFile.Text = "File";
                lblFile.GridRow = 6;
                lblFile.GridColumn = 0;
                grid.Widgets.Add(lblFile);

                var textFile = new TextBox();
                textFile.Text = "";
                textFile.GridRow = 6;
                textFile.GridColumn = 1;
                textFile.Width = 300;
                grid.Widgets.Add(textFile);

                var bttFile = new TextButton();
                bttFile.GridRow = 6;
                bttFile.GridColumn = 2;
                bttFile.Text = "...";
                bttFile.Click += (s1, a1) =>
                {
                    var ofd = new FileDialog(FileDialogMode.OpenFile);
                    ofd.ShowModal();
                    ofd.FilePath = "d:\\develop\\NET\\Monogame\\Models\\";
                    ofd.Closed += (s2, a2) =>
                    {
                        if (ofd.FilePath != "" && ofd.Result)
                        {
                            textFile.Text = ofd.FilePath;
                        }
                    };
                };
                grid.Widgets.Add(bttFile);

                var bttBuild = new TextButton();
                bttBuild.GridRow = 7;
                bttBuild.GridColumn = 1;
                bttBuild.Text = "Build";
                bttBuild.Width = 100;
                bttBuild.Click += (s1, a1) =>
                {
                    _window.Close();

                    String result = game.bModel.Load(textFile.Text);

                    if (result == "")
                    {
                        game.camera.rotationh = 0f;
                        game.camera.rotationv = 0f;
                        game.camera.distance = game.bModel.bb.Max.Length() * 3;
                        game.axis.size = game.bModel.bb.Max.Length();
                        game.boundary.bb = game.bModel.bb;
                        if (game.bModel.tex != null)
                            game.textureCrate = game.bModel.tex;

                        game.octree = new Octree(game, game.octreeDepth, game.octantTextureCoordinates, game.optimizeOctantFaces);

                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            game.octree.startTime = DateTime.Now;
                            game.octree.Build(game.bModel);
                            game.octree.BuildTextureCoordinates();
                            if (game.fillObject)
                            {
                                game.octree.Fill();
                            }
                            if (game.optimizeOctree)
                            {
                                game.octree.Optimize();
                            }
                            game.octree.BuildMesh();
                            game.octree.endTime = DateTime.Now;
                        }).Start();
                    };
                };

                grid.Widgets.Add(bttBuild);

                _window.ShowModal();
            };

            return _mnuBuildFromModel;
        }

        public MenuItem CreateModelInfo()
        {
            var _mnuAxisOptions = new MenuItem();
            _mnuAxisOptions.Id = "_mnuModelInfo";
            _mnuAxisOptions.Text = "Model information...";
            _mnuAxisOptions.Selected += (s, a) =>
            {
                var _window = new Window();
                _window.Title = "Model information";
                _window.Width = game.Window.ClientBounds.Width / 2;

                var grid = new Grid
                {
                    ShowGridLines = false,
                    ColumnSpacing = 8,
                    RowSpacing = 8,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                _window.Content = grid;

                // Set partitioning configuration
                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Auto,
                });
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());

                // Add widgets
                var lblOctantsFilled = new Label();
                lblOctantsFilled.Text = string.Format("Octants filled: {0}", game.octree.octantsFilled);
                lblOctantsFilled.GridRow = 0;
                lblOctantsFilled.GridColumn = 0;
                grid.Widgets.Add(lblOctantsFilled);

                var lblVertices = new Label();
                lblVertices.Text = string.Format("Vertices: {0}", game.bModel.totalVertices());
                lblVertices.GridRow = 1;
                lblVertices.GridColumn = 0;
                grid.Widgets.Add(lblVertices);

                var lblFaces = new Label();
                lblFaces.Text = string.Format("Faces: {0}", game.bModel.totalFaces());
                lblFaces.GridRow = 2;
                lblFaces.GridColumn = 0;
                grid.Widgets.Add(lblFaces);

                var lblSize = new Label();
                lblSize.Text = string.Format("Size: <{0},{1},{2}>", (game.bModel.bb.Max.X-game.bModel.bb.Min.X).ToString("00.00").Replace(",","."), (game.bModel.bb.Max.Y - game.bModel.bb.Min.Y).ToString("00.00").Replace(",", "."), (game.bModel.bb.Max.Z - game.bModel.bb.Min.Z).ToString("00.00").Replace(",", "."));
                lblSize.GridRow = 3;
                lblSize.GridColumn = 0;
                grid.Widgets.Add(lblSize);

                _window.ShowModal();
            };

            return _mnuAxisOptions;
        }

        public MenuItem CreateAxisOptions()
        {
            var _mnuAxisOptions = new MenuItem();
            _mnuAxisOptions.Id = "_mnuAxisOptions";
            _mnuAxisOptions.Text = "Axis options...";
            _mnuAxisOptions.Selected += (s, a) =>
            {
                var _window = new Window();
                _window.Title = "Axis options";
                _window.Width = game.Window.ClientBounds.Width / 4;

                var grid = new Grid
                {
                    ShowGridLines = false,
                    ColumnSpacing = 8,
                    RowSpacing = 8,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                _window.Content = grid;

                // Set partitioning configuration
                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Auto,
                });
                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Fill,
                });
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());

                // Add widgets
                var textSize = new TextBox();
                textSize.Text = "Size";
                textSize.GridRow = 0;
                textSize.GridColumn = 0;
                grid.Widgets.Add(textSize);

                var size = new HorizontalSlider();
                size.HorizontalAlignment = HorizontalAlignment.Stretch;
                size.Maximum = 10f;
                size.Minimum = 0.1f;
                size.GridRow = 0;
                size.GridColumn = 1;
                size.ValueChanged += (s1, a1) =>
                {
                    game.axis.size = size.Value;
                };
                grid.Widgets.Add(size);

                var textFull = new Label();
                textFull.Text = "Full axis";
                textFull.GridRow = 1;
                textFull.GridColumn = 0;
                grid.Widgets.Add(textFull);

                var full = new CheckBox();
                full.GridRow = 1;
                full.GridColumn = 1;
                full.Click += (s1, a1) =>
                {
                    game.axis.full = full.IsPressed;
                };
                grid.Widgets.Add(full);

                _window.ShowModal();
            };

            return _mnuAxisOptions;
        }
    }
}