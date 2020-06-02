﻿using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.File;
using System;
using System.IO;
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
        public TextBox txtOctreeBuildInfo;
        public TextBox txtOctreeVertices;
        public TextBox txtOctreeElapsedTime;

        private MenuItem _menuView;
        private MenuItem _mnuModelHide;
        private MenuItem _mnuModelShow;
        private MenuItem _mnuOctreeHide;
        private MenuItem _mnuOctreeShow;
        private MenuItem _mnuAxisHide;
        private MenuItem _mnuAxisShow;
        private MenuItem _mnuBoundaryHide;
        private MenuItem _mnuBoundaryShow;
        private MenuItem _mnuModelNormalHide;
        private MenuItem _mnuModelNormalShow;
        private MenuItem _mnuWireframe;
        private MenuItem _mnuSolid;
        private MenuItem _mnuOrthographic;
        private MenuItem _mnuPerspective;

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

            txtOctreeBuildInfo = new TextBox();
            horizontalBox.Widgets.Add(txtOctreeBuildInfo);

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

            _menuFile.Items.Add(CreateBuildDirectToDisk());

            var _mnuOpenOctree = new MenuItem();
            _mnuOpenOctree.Id = "_menuOpenOctree";
            _mnuOpenOctree.Text = "Open octree...";
            _mnuOpenOctree.Selected += (s, a) =>
            {
                var ofd = new FileDialog(FileDialogMode.OpenFile);
                ofd.Folder = Directory.GetCurrentDirectory() + "\\SampleModels\\";
                ofd.Filter = "*.oct";
                ofd.ShowModal();
                ofd.Closed += (s1, a1) =>
                {
                    if (ofd.FilePath != "" && ofd.Result)
                    {
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            game.octree.startTime = DateTime.Now;
                            game.bModel = new BModel(game.GraphicsDevice);
                            game.octree.OpenOctree(ofd.FilePath);
                            game.camera.rotationh = 0f;
                            game.camera.rotationv = 0f;
                            game.camera.distance = game.octree.bb.Max.Length() * 3;
                            game.axis.size = game.octree.bb.Max.Length();
                            game.boundary.bb = game.octree.bb;
                            game.octree.currentOperation = "";
                            game.octree.endTime = DateTime.Now;
                        }).Start();
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
                ofd.FilePath = Directory.GetCurrentDirectory() + "\\SampleModels\\";
                ofd.Filter = "*.oct";
                ofd.AutoAddFilterExtension = true;
                ofd.ShowModal();

                ofd.Closed += (s1, a1) =>
                {
                    if (ofd.FilePath != "" && ofd.Result)
                    {
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            game.octree.startTime = DateTime.Now;
                            game.octree.SaveOctree(ofd.FilePath);
                            game.octree.currentOperation = "";
                            game.octree.endTime = DateTime.Now;
                        }).Start();
                    }
                };

            };
            _menuFile.Items.Add(_mnuSaveOctree);

            _menuFile.Items.Add(CreateBuildFromOperation());

            var _menuEdit = new MenuItem();
            _menuEdit.Id = "_menuEdit";
            _menuEdit.Text = "Edit";

            var _mnuReverse = new MenuItem();
            _mnuReverse.Id = "_mnuReverse";
            _mnuReverse.Text = "Reverse              -";
            _mnuReverse.Selected += (s, a) =>
            {
                ClickReverse();
            };
            _menuEdit.Items.Add(_mnuReverse);

            var _mnuOptimize = new MenuItem();
            _mnuOptimize.Id = "_mnuOptimize";
            _mnuOptimize.Text = "Optimize";
            _mnuOptimize.Selected += (s, a) =>
            {
                ClickOptimize();
            };
            _menuEdit.Items.Add(_mnuOptimize);

            _menuEdit.Items.Add(CreateFill());

            var _menuOptions = new MenuItem();
            _menuOptions.Id = "_menuOptions";
            _menuOptions.Text = "Options";

            _menuOptions.Items.Add(CreateAxisOptions());

            var _mnuModelColor = new MenuItem();
            _mnuModelColor.Id = "_mnuOctreeColor";
            _mnuModelColor.Text = "Model color...";
            _mnuModelColor.Selected += (s, a) =>
            {
                ColorPickerDialog dialog = new ColorPickerDialog
                {
                    Color = Color.Yellow
                };

                dialog.Closed += (s2, a2) =>
                {
                    if (!dialog.Result)
                    {
                        // "Cancel" or Escape
                        return;
                    }

                    game.setModelColor(System.Drawing.Color.FromArgb(dialog.Color.R, dialog.Color.G, dialog.Color.B));
                };

                dialog.ShowModal();
            };
            _menuOptions.Items.Add(_mnuModelColor);

            var _mnuOctreeColor = new MenuItem();
            _mnuOctreeColor.Id = "_mnuOctreeColor";
            _mnuOctreeColor.Text = "Octree color...";
            _mnuOctreeColor.Selected += (s, a) =>
            {
                ColorPickerDialog dialog = new ColorPickerDialog
                {
                    Color = Color.Yellow
                };

                dialog.Closed += (s2, a2) =>
                {
                    if (!dialog.Result)
                    {
                        // "Cancel" or Escape
                        return;
                    }

                    game.setOctreeColor(System.Drawing.Color.FromArgb(dialog.Color.R,dialog.Color.G, dialog.Color.B));
                };

                dialog.ShowModal();
            };
            _menuOptions.Items.Add(_mnuOctreeColor);

            _menuView = new MenuItem();
            _menuView.Id = "_menuView";
            _menuView.Text = "View";

            _menuView.Items.Add(CreateModelInfo());

            var _menuSep1 = new MenuSeparator();
            _menuView.Items.Add(_menuSep1);

            _mnuModelHide = new MenuItem();
            _mnuModelHide.Id = "_mnuModelHide";
            _mnuModelHide.Text = "Hide model                      E";
            _mnuModelHide.Selected += (s, a) =>
            {
                ClickModelHide();
            };
            _menuView.Items.Add(_mnuModelHide);

            _mnuModelShow = new MenuItem();
            _mnuModelShow.Id = "_mnuModelShow";
            _mnuModelShow.Text = "Show model                     D";
            _mnuModelShow.Selected += (s, a) =>
            {
                ClickModelShow();
            };

            _mnuOctreeHide = new MenuItem();
            _mnuOctreeHide.Id = "_mnuOctreeHide";
            _mnuOctreeHide.Text = "Hide octree                      R";
            _mnuOctreeHide.Selected += (s, a) =>
            {
                ClickOctreeHide();
            };
            _menuView.Items.Add(_mnuOctreeHide);

            _mnuOctreeShow = new MenuItem();
            _mnuOctreeShow.Id = "_mnuOctreeShow";
            _mnuOctreeShow.Text = "Show octree                     F";
            _mnuOctreeShow.Selected += (s, a) =>
            {
                ClickOctreeShow();
            };

            _mnuAxisHide = new MenuItem();
            _mnuAxisHide.Id = "_mnuAxisHide";
            _mnuAxisHide.Text = "Hide axis                          T";
            _mnuAxisHide.Selected += (s, a) =>
            {
                ClickAxisHide();
            };
            _menuView.Items.Add(_mnuAxisHide);

            _mnuAxisShow = new MenuItem();
            _mnuAxisShow.Id = "_mnuAxisShow";
            _mnuAxisShow.Text = "Show axis                        G";
            _mnuAxisShow.Selected += (s, a) =>
            {
                ClickAxisShow();
            };

            _mnuBoundaryHide = new MenuItem();
            _mnuBoundaryHide.Id = "_menuBoundaryHide";
            _mnuBoundaryHide.Text = "Hide boundary                U";
            _mnuBoundaryHide.Selected += (s, a) =>
            {
                ClickBoundaryHide();
            };
            _menuView.Items.Add(_mnuBoundaryHide);

            _mnuBoundaryShow = new MenuItem();
            _mnuBoundaryShow.Id = "_menuBoundaryShow";
            _mnuBoundaryShow.Text = "Show boundary                J";
            _mnuBoundaryShow.Selected += (s, a) =>
            {
                ClickBoundaryShow();
            };

            _mnuModelNormalHide = new MenuItem();
            _mnuModelNormalHide.Id = "_mnuModelNormalHide";
            _mnuModelNormalHide.Text = "Hide model normals       B";
            _mnuModelNormalHide.Selected += (s, a) =>
            {
                ClickModelNormalHide();
            };

            _mnuModelNormalShow = new MenuItem();
            _mnuModelNormalShow.Id = "_mnuModelNormalShow";
            _mnuModelNormalShow.Text = "Show model normals     N";
            _mnuModelNormalShow.Selected += (s, a) =>
            {
                ClickModelNormalShow();
            };
            _menuView.Items.Add(_mnuModelNormalShow);

            _mnuWireframe = new MenuItem();
            _mnuWireframe.Id = "_mnuWireframe";
            _mnuWireframe.Text = "Wireframe                       W";
            _mnuWireframe.Selected += (s, a) =>
            {
                ClickWireframe();
            };
            _menuView.Items.Add(_mnuWireframe);

            _mnuSolid = new MenuItem();
            _mnuSolid.Id = "_mnuSolid";
            _mnuSolid.Text = "Solid                                  S";
            _mnuSolid.Selected += (s, a) =>
            {
                ClickSolid();
            };

            var _menuSep2 = new MenuSeparator();
            _menuView.Items.Add(_menuSep2);

            _mnuOrthographic = new MenuItem();
            _mnuOrthographic.Id = "_mnuOrthographic";
            _mnuOrthographic.Text = "Orthographic                   O";
            _mnuOrthographic.Selected += (s, a) =>
            {
                ClickOrthographic();
            };
            _menuView.Items.Add(_mnuOrthographic);

            _mnuPerspective = new MenuItem();
            _mnuPerspective.Id = "_menuBoundaryShow";
            _mnuPerspective.Text = "Perspective                       P";
            _mnuPerspective.Selected += (s, a) =>
            {
                ClickPerspective();
            };

            var _mnuYZ = new MenuItem();
            _mnuYZ.Id = "_mnuYZ";
            _mnuYZ.Text = "YZ                                      1";
                          
            _mnuYZ.Selected += (s, a) =>
            {
                ClickYZ();
            };
            _menuView.Items.Add(_mnuYZ);

            var _mnuXY = new MenuItem();
            _mnuXY.Id = "_mnuXY";
            _mnuXY.Text = "XY                                      2";
            _mnuXY.Selected += (s, a) =>
            {
                ClickXY();
            };
            _menuView.Items.Add(_mnuXY);

            var _mnuYX = new MenuItem();
            _mnuYX.Id = "_mnuYX";
            _mnuYX.Text = "YX                                      3";
            _mnuYX.Selected += (s, a) =>
            {
                ClickYX();
            };
            _menuView.Items.Add(_mnuYX);

            var _mnuXZ = new MenuItem();
            _mnuXZ.Id = "_mnuXZ";
            _mnuXZ.Text = "XZ                                      4";
            _mnuXZ.Selected += (s, a) =>
            {
                ClickXZ();
            };
            _menuView.Items.Add(_mnuXZ);

            var _mnuXYZ = new MenuItem();
            _mnuXYZ.Id = "_mnuXYZ";
            _mnuXYZ.Text = "XYZ                                    5";
            _mnuXYZ.Selected += (s, a) =>
            {
                ClickXYZ();
            };
            _menuView.Items.Add(_mnuXYZ);

            var _menuSep3 = new MenuSeparator();
            _menuView.Items.Add(_menuSep3);

            var _menuFullScreenOn = new MenuItem();
            var _menuFullScreenOff = new MenuItem();

            _menuFullScreenOn.Id = "_menuFullScreenOn";
            _menuFullScreenOn.Text = "Fullscreen on";
            _menuFullScreenOn.Selected += (s, a) =>
            {
                int pos = _menuView.Items.IndexOf(_menuFullScreenOn);
                _menuView.Items.Remove(_menuFullScreenOn);
                _menuView.Items.Insert(pos, _menuFullScreenOff);
                game._graphics.PreferredBackBufferWidth = game.GraphicsDevice.DisplayMode.Width;
                game._graphics.PreferredBackBufferHeight = game.GraphicsDevice.DisplayMode.Height;
                game._graphics.IsFullScreen = true;
                game._graphics.ApplyChanges();
            };
            _menuView.Items.Add(_menuFullScreenOn);

            _menuFullScreenOff.Id = "_menuFullScreenOff";
            _menuFullScreenOff.Text = "Fullscreen off";
            _menuFullScreenOff.Selected += (s, a) =>
            {
                int pos = _menuView.Items.IndexOf(_menuFullScreenOff);
                _menuView.Items.Remove(_menuFullScreenOff);
                _menuView.Items.Insert(pos, _menuFullScreenOn);
                game._graphics.PreferredBackBufferWidth = game.originalWidth;
                game._graphics.PreferredBackBufferHeight = game.originalHeight;
                game._graphics.IsFullScreen = false;
                game._graphics.ApplyChanges();
            };

            var _mnuStatusBarHide = new MenuItem();
            var _mnuStatusBarShow = new MenuItem();

            _mnuStatusBarHide.Id = "_mnuStatusBarHide";
            _mnuStatusBarHide.Text = "Hide status bar";
            _mnuStatusBarHide.Selected += (s, a) =>
            {
                int pos = _menuView.Items.IndexOf(_mnuStatusBarHide);
                _menuView.Items.Remove(_mnuStatusBarHide);
                _menuView.Items.Insert(pos, _mnuStatusBarShow);
                horizontalBox.Visible = false;
            };
            _menuView.Items.Add(_mnuStatusBarHide);

            _mnuStatusBarShow.Id = "_mnuStatusBarShow";
            _mnuStatusBarShow.Text = "Show status bar";
            _mnuStatusBarShow.Selected += (s, a) =>
            {
                int pos = _menuView.Items.IndexOf(_mnuStatusBarShow);
                _menuView.Items.Remove(_mnuStatusBarShow);
                _menuView.Items.Insert(pos, _mnuStatusBarHide);
                horizontalBox.Visible = true;
            };

            var verticalMenu1 = new HorizontalMenu();
            verticalMenu1.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
            verticalMenu1.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Top;
            verticalMenu1.Items.Add(_menuFile);
            verticalMenu1.Items.Add(_menuEdit);
            verticalMenu1.Items.Add(_menuOptions);
            verticalMenu1.Items.Add(_menuView);

            Desktop.Widgets.Add(verticalMenu1);
            Desktop.Widgets.Add(horizontalBox);
            
        }

        public void ClickOptimize()
        {
            game.octree.Optimize();
        }

        public void ClickReverse()
        {
            Octree B = game.octree.Reverse();
            game.octree = B;
        }

        public void ClickModelHide()
        {
            int pos = _menuView.Items.IndexOf(_mnuModelHide);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuModelHide);
                _menuView.Items.Insert(pos, _mnuModelShow);
                game.showModel = false;
            }
        }

        public void ClickModelShow()
        {
            int pos = _menuView.Items.IndexOf(_mnuModelShow);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuModelShow);
                _menuView.Items.Insert(pos, _mnuModelHide);
                game.showModel = true;
            }
        }

        public void ClickOctreeHide()
        {
            int pos = _menuView.Items.IndexOf(_mnuOctreeHide);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuOctreeHide);
                _menuView.Items.Insert(pos, _mnuOctreeShow);
                game.showOctree = false;
            }
        }

        public void ClickOctreeShow()
        {
            int pos = _menuView.Items.IndexOf(_mnuOctreeShow);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuOctreeShow);
                _menuView.Items.Insert(pos, _mnuOctreeHide);
                game.showOctree = true;
            }
        }

        public void ClickAxisHide()
        {
            int pos = _menuView.Items.IndexOf(_mnuAxisHide);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuAxisHide);
                _menuView.Items.Insert(pos, _mnuAxisShow);
                game.showAxis = false;
            }
        }

        public void ClickAxisShow()
        {
            int pos = _menuView.Items.IndexOf(_mnuAxisShow);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuAxisShow);
                _menuView.Items.Insert(pos, _mnuAxisHide);
                game.showAxis = true;
            }
        }

        public void ClickBoundaryHide()
        {
            int pos = _menuView.Items.IndexOf(_mnuBoundaryHide);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuBoundaryHide);
                _menuView.Items.Insert(pos, _mnuBoundaryShow);
                game.showBoundary = false;
            }
        }

        public void ClickBoundaryShow()
        {
            int pos = _menuView.Items.IndexOf(_mnuBoundaryShow);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuBoundaryShow);
                _menuView.Items.Insert(pos, _mnuBoundaryHide);
                game.showBoundary = true;
            }
        }

        public void ClickModelNormalHide()
        {
            int pos = _menuView.Items.IndexOf(_mnuModelNormalHide);
            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuModelNormalHide);
                _menuView.Items.Insert(pos, _mnuModelNormalShow);
                game.showModelNormals = false;
            }
        }

        public void ClickModelNormalShow()
        {
            int pos = _menuView.Items.IndexOf(_mnuModelNormalShow);

            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuModelNormalShow);
                _menuView.Items.Insert(pos, _mnuModelNormalHide);
                game.showModelNormals = true;
            }
        }

        public void ClickWireframe()
        {
            int pos = _menuView.Items.IndexOf(_mnuWireframe);

            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuWireframe);
                _menuView.Items.Insert(pos, _mnuSolid);
                game.wireframe = true;
            }
        }

        public void ClickSolid()
        {
            int pos = _menuView.Items.IndexOf(_mnuSolid);

            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuSolid);
                _menuView.Items.Insert(pos, _mnuWireframe);
                game.wireframe = false;
            }
        }

        public void ClickPerspective()
        {
            int pos = _menuView.Items.IndexOf(_mnuPerspective);

            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuPerspective);
                _menuView.Items.Insert(pos, _mnuOrthographic);
                game.projection = "P";
            }
        }

        public void ClickOrthographic()
        {
            int pos = _menuView.Items.IndexOf(_mnuOrthographic);

            if (pos >= 0)
            {
                _menuView.Items.Remove(_mnuOrthographic);
                _menuView.Items.Insert(pos, _mnuPerspective);
                game.projection = "O";
                game.CalculateOrtographicMatrix();
            }
        }

        public void ClickYZ()
        {
            game.camera.rotationh = 0;
            game.camera.rotationv = 0;
        }

        public void ClickXY()
        {
            game.camera.rotationh = 270;
            game.camera.rotationv = 90;
        }

        public void ClickYX()
        {
            game.camera.rotationh = 0;
            game.camera.rotationv = 270;
        }

        public void ClickXZ()
        {
            game.camera.rotationh = 270;
            game.camera.rotationv = 0;
        }

        public void ClickXYZ()
        {
            game.camera.rotationh = 45;
            game.camera.rotationv = 45;
        }

        public MenuItem CreateBuildDirectToDisk()
        {
            var _mnuBuildDirectToDisk = new MenuItem();
            _mnuBuildDirectToDisk.Id = "_mnuBuildWhileSavingToDisk";
            _mnuBuildDirectToDisk.Text = "Build direct to disk...";
            _mnuBuildDirectToDisk.Selected += (s, a) =>
            {
                var _window = new Window();
                _window.Title = "Build direct to disk";
                _window.Width = 350;

                var bttFileOctree = new TextButton();
                var lblFileOctree = new Label();
                var textFileOctree = new TextBox();

                var grid = new Grid
                {
                    ShowGridLines = false,
                    ColumnSpacing = 8,
                    RowSpacing = 8,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                _window.Content = grid;

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

                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());

                var lblSize = new Label();
                lblSize.Text = "Depth";
                lblSize.GridRow = 0;
                lblSize.GridColumn = 0;
                grid.Widgets.Add(lblSize);

                var depth = new SpinButton();
                depth.HorizontalAlignment = HorizontalAlignment.Stretch;
                depth.Maximum = 15;
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

                var lblMeshFile = new Label();
                lblMeshFile.Text = "Mesh file";
                lblMeshFile.GridRow = 1;
                lblMeshFile.GridColumn = 0;
                grid.Widgets.Add(lblMeshFile);

                var textMeshFile = new TextBox();
                textMeshFile.Text = "";
                textMeshFile.GridRow = 1;
                textMeshFile.GridColumn = 1;
                textMeshFile.Width = 240;
                grid.Widgets.Add(textMeshFile);

                var bttMeshFile = new TextButton();
                bttMeshFile.GridRow = 1;
                bttMeshFile.GridColumn = 2;
                bttMeshFile.Text = "...";
                bttMeshFile.Click += (s1, a1) =>
                {
                    var ofd = new FileDialog(FileDialogMode.OpenFile);
                    ofd.Filter = "*.*";
                    ofd.ShowModal();
                    ofd.FilePath = Directory.GetCurrentDirectory() + "\\SampleModels\\";
                    ofd.Closed += (s2, a2) =>
                    {
                        if (ofd.FilePath != "" && ofd.Result)
                        {
                            textMeshFile.Text = ofd.FilePath;
                        }
                    };
                };
                grid.Widgets.Add(bttMeshFile);

                lblFileOctree.Text = "Octree";
                lblFileOctree.GridRow = 2;
                lblFileOctree.GridColumn = 0;
                grid.Widgets.Add(lblFileOctree);

                textFileOctree.Text = "";
                textFileOctree.GridRow = 2;
                textFileOctree.GridColumn = 1;
                textFileOctree.Width = 240;
                grid.Widgets.Add(textFileOctree);

                bttFileOctree.GridRow = 2;
                bttFileOctree.GridColumn = 2;
                bttFileOctree.Text = "...";
                bttFileOctree.Click += (s1, a1) =>
                {
                    var ofd = new FileDialog(FileDialogMode.SaveFile);
                    ofd.Filter = "*.oct";
                    ofd.ShowModal();
                    ofd.FilePath = Directory.GetCurrentDirectory() + "\\SampleModels\\";
                    ofd.Closed += (s2, a2) =>
                    {
                        if (ofd.FilePath != "" && ofd.Result)
                        {
                            textFileOctree.Text = ofd.FilePath;
                        }
                    };
                };
                grid.Widgets.Add(bttFileOctree);

                var bttBuild = new TextButton();
                bttBuild.GridRow = 3;
                bttBuild.GridColumn = 1;
                bttBuild.Text = "Build";
                bttBuild.Width = 100;
                bttBuild.Click += (s1, a1) =>
                {
                    if (textMeshFile.Text == "")
                    {
                        showMessage("No mesh file selected", "Alert");
                        return;
                    }

                    if (textFileOctree.Text == "")
                    {
                        showMessage("No octree file selected", "Alert");
                        return;
                    }

                    _window.Close();

                    ClickBoundaryHide();
                    ClickModelHide();
                    ClickOctreeHide();

                    String result = game.bModel.Load(textMeshFile.Text);

                    if (result == "")
                    {
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;

                            game.camera.rotationh = 0f;
                            game.camera.rotationv = 0f;
                            game.camera.distance = game.bModel.bb.Max.Length() * 3;
                            game.axis.size = game.bModel.bb.Max.Length();
                            game.boundary.bb = game.bModel.bb;
                            if (game.bModel.tex != null)
                            {
                                game.textureModel = game.bModel.tex;
                            }
                            else
                            {
                                game.textureModel = game.textureModelDefault;
                            }

                            game.octree = new Octree(game, (short)depth.Value, true, false, "No fill");

                            game.octree.startTime = DateTime.Now;
                            game.octree.fileSaveOctree = new System.IO.StreamWriter(textFileOctree.Text, false);
                            game.octree.fileSaveOctree.AutoFlush = true;
                            game.octree.fileSaveOctree.WriteLine(game.bModel.bb.Max.X + "#" + game.bModel.bb.Max.Y + "#" + game.bModel.bb.Max.Z);
                            game.octree.fileSaveOctree.WriteLine(game.bModel.bb.Min.X + "#" + game.bModel.bb.Min.Y + "#" + game.bModel.bb.Min.Z);
                            game.octree.Build(game.bModel);
                            game.octree.fileSaveOctree.Close();
                            game.octree.fileSaveOctree = null;
                            game.octree.currentOperation = "";
                            game.octree.endTime = DateTime.Now;

                        }).Start();
                    }

                };
                grid.Widgets.Add(bttBuild);

                _window.ShowModal();
            };

            return _mnuBuildDirectToDisk;
        }

        public MenuItem CreateBuildFromOperation()
        {
            var _mnuBuildFromOperation = new MenuItem();
            _mnuBuildFromOperation.Id = "_mnuBuildFromOperation";
            _mnuBuildFromOperation.Text = "Build from operation...";
            _mnuBuildFromOperation.Selected += (s, a) =>
            {
                var _window = new Window();
                _window.Title = "Build from operation";
                _window.Width = 550;

                var bttFileOctreeB = new TextButton();
                var lblFileOctreeB = new Label();
                var textFileOctreeB = new TextBox();

                var grid = new Grid
                {
                    ShowGridLines = false,
                    ColumnSpacing = 8,
                    RowSpacing = 8,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                _window.Content = grid;

                grid.ColumnsProportions.Add(new Proportion
                {
                    Type = Myra.Graphics2D.UI.ProportionType.Auto,
                });

                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());

                var lblOperation = new Label();
                lblOperation.Text = "Operation";
                lblOperation.GridRow = 0;
                lblOperation.GridColumn = 0;
                grid.Widgets.Add(lblOperation);

                var comboOperation = new ComboBox
                {
                    GridColumn = 1,
                    GridRow = 0
                };

                comboOperation.Items.Add(new ListItem("Union"));
                comboOperation.Items.Add(new ListItem("Intersection"));
                comboOperation.Items.Add(new ListItem("Substract"));
                comboOperation.Items.Add(new ListItem("Reverse"));
                comboOperation.Items[0].IsSelected = true;
                comboOperation.Width = 100;
                comboOperation.SelectedIndexChanged += (s1, a1) =>
                {
                    if (comboOperation.SelectedItem.Text == "Reverse")
                    {
                        lblFileOctreeB.Visible = false;
                        textFileOctreeB.Visible = false;
                        bttFileOctreeB.Visible = false;
                        textFileOctreeB.Text = "";
                    }
                    else
                    {
                        bttFileOctreeB.Visible = true;
                        lblFileOctreeB.Visible = true;
                        textFileOctreeB.Visible = true;
                        bttFileOctreeB.Visible = true;
                    }
                };
                grid.Widgets.Add(comboOperation);

                var lblFile = new Label();
                lblFile.Text = "Octree A";
                lblFile.GridRow = 1;
                lblFile.GridColumn = 0;
                grid.Widgets.Add(lblFile);

                var textFileOctreeA = new TextBox();
                textFileOctreeA.Text = "";
                textFileOctreeA.GridRow = 1;
                textFileOctreeA.GridColumn = 1;
                textFileOctreeA.Width = 300;
                grid.Widgets.Add(textFileOctreeA);

                var bttFileOctreeA = new TextButton();
                bttFileOctreeA.GridRow = 1;
                bttFileOctreeA.GridColumn = 2;
                bttFileOctreeA.Text = "...";
                bttFileOctreeA.Click += (s1, a1) =>
                {
                    var ofd = new FileDialog(FileDialogMode.OpenFile);
                    ofd.Filter = "*.oct";
                    ofd.ShowModal();
                    ofd.FilePath = Directory.GetCurrentDirectory() + "\\SampleModels\\";
                    ofd.Closed += (s2, a2) =>
                    {
                        if (ofd.FilePath != "" && ofd.Result)
                        {
                            textFileOctreeA.Text = ofd.FilePath;
                        }
                    };
                };
                grid.Widgets.Add(bttFileOctreeA);

                lblFileOctreeB.Text = "Octree B";
                lblFileOctreeB.GridRow = 2;
                lblFileOctreeB.GridColumn = 0;
                grid.Widgets.Add(lblFileOctreeB);

                textFileOctreeB.Text = "";
                textFileOctreeB.GridRow = 2;
                textFileOctreeB.GridColumn = 1;
                textFileOctreeB.Width = 300;
                grid.Widgets.Add(textFileOctreeB);

                bttFileOctreeB.GridRow = 2;
                bttFileOctreeB.GridColumn = 2;
                bttFileOctreeB.Text = "...";
                bttFileOctreeB.Click += (s1, a1) =>
                {
                    var ofd = new FileDialog(FileDialogMode.OpenFile);
                    ofd.Filter = "*.oct";
                    ofd.ShowModal();
                    ofd.FilePath = Directory.GetCurrentDirectory() + "\\SampleModels\\";
                    ofd.Closed += (s2, a2) =>
                    {
                        if (ofd.FilePath != "" && ofd.Result)
                        {
                            textFileOctreeB.Text = ofd.FilePath;
                        }
                    };
                };
                grid.Widgets.Add(bttFileOctreeB);

                var bttBuild = new TextButton();
                bttBuild.GridRow = 3;
                bttBuild.GridColumn = 1;
                bttBuild.Text = "Build";
                bttBuild.Width = 100;
                bttBuild.Click += (s1, a1) =>
                {
                    if (textFileOctreeA.Text == "")
                    {
                        showMessage("No octree A selected for operation " + comboOperation.SelectedItem.Text ,"Alert");
                        return;
                    }

                    if (comboOperation.SelectedItem.Text != "Reverse" && textFileOctreeB.Text == "")
                    {
                        showMessage("No octree B selected for operation " + comboOperation.SelectedItem.Text, "Alert");
                        return;
                    }

                    Octree A = new Octree(game);
                    A.OpenOctree(textFileOctreeA.Text);
                    Octree B = new Octree(game);

                    switch (comboOperation.SelectedItem.Text)
                    {
                        case "Union":
                            B.OpenOctree(textFileOctreeB.Text);
                            game.octree = A.Union(B);
                            break;
                        case "Intersection":
                            B.OpenOctree(textFileOctreeB.Text);
                            game.octree = A.Intersection(B);
                            break;
                        case "Substract":
                            B.OpenOctree(textFileOctreeB.Text);
                            game.octree = A.Substract(B);
                            break;
                        case "Reverse":
                            game.octree = A.Reverse();
                            break;
                    }

                    game.camera.rotationh = 0f;
                    game.camera.rotationv = 0f;
                    game.camera.distance = game.octree.bb.Max.Length() * 3;
                    game.axis.size = game.octree.bb.Max.Length();
                    game.boundary.bb = game.octree.bb;

                    _window.Close();
                };
                grid.Widgets.Add(bttBuild);

                _window.ShowModal();
            };

            return _mnuBuildFromOperation;
        }

        private void showMessage(string text,string title)
        {
            var _window = new Window();
            _window.Title = title;

            var grid = new Grid
            {
                ShowGridLines = false,
                ColumnSpacing = 8,
                RowSpacing = 8,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            _window.Content = grid;

            grid.ColumnsProportions.Add(new Proportion
            {
                Type = Myra.Graphics2D.UI.ProportionType.Auto
            });

            grid.RowsProportions.Add(new Proportion());
            grid.RowsProportions.Add(new Proportion());

            var label1 = new Label
            {
                Text = text
            };
            label1.GridColumn = 0;
            label1.GridRow = 0;
            grid.Widgets.Add(label1);

            var bttClose = new TextButton();
            bttClose.Text = "Close";
            bttClose.Width = 100;
            bttClose.GridColumn = 0;
            bttClose.GridRow = 1;
            bttClose.HorizontalAlignment = HorizontalAlignment.Center;
            bttClose.Click += (s1, a1) =>
            {
                _window.Close();
            };
            grid.Widgets.Add(bttClose);

            _window.Content = grid;

            _window.ShowModal();
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

                // Add widgets
                var lblSize = new Label();
                lblSize.Text = "Depth";
                lblSize.GridRow = 0;
                lblSize.GridColumn = 0;
                grid.Widgets.Add(lblSize);

                var depth = new SpinButton();
                depth.HorizontalAlignment = HorizontalAlignment.Stretch;
                depth.Maximum = 15;
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
                lblOctantTextureCoordinates.Text = "Calculate color";
                lblOctantTextureCoordinates.GridRow = 1;
                lblOctantTextureCoordinates.GridColumn = 0;
                grid.Widgets.Add(lblOctantTextureCoordinates);

                var chkCalculateColor = new CheckBox();
                chkCalculateColor.GridRow = 1;
                chkCalculateColor.GridColumn = 1;
                chkCalculateColor.IsPressed = game.optimizeOctree;
                chkCalculateColor.Click += (s1, a1) =>
                {
                    game.calculatecolor = chkCalculateColor.IsPressed;
                };
                grid.Widgets.Add(chkCalculateColor);

                var lblFillObject = new Label();
                lblFillObject.Text = "Fill object direction";
                lblFillObject.GridRow = 3;
                lblFillObject.GridColumn = 0;
                grid.Widgets.Add(lblFillObject);

                var comboFillDirection = new ComboBox
                {
                    GridColumn = 1,
                    GridRow = 3
                };

                comboFillDirection.Items.Add(new ListItem("No fill"));
                comboFillDirection.Items.Add(new ListItem("Z+"));
                comboFillDirection.Items.Add(new ListItem("Z-"));
                comboFillDirection.Items.Add(new ListItem("X+"));
                comboFillDirection.Items.Add(new ListItem("X-"));
                comboFillDirection.Items.Add(new ListItem("Y+"));
                comboFillDirection.Items.Add(new ListItem("Y-"));
                for (int i = 0; i < comboFillDirection.Items.Count; i++)
                {
                    if (comboFillDirection.Items[i].Text == game.fillDirection)
                    {
                        comboFillDirection.Items[i].IsSelected = true;
                    }
                }
                comboFillDirection.SelectedIndexChanged += (s1, a1) =>
                {
                    game.fillDirection = comboFillDirection.SelectedItem.Text;
                };
                comboFillDirection.Width = 80;
                grid.Widgets.Add(comboFillDirection);

                var lblOptimizeOctree = new Label();
                lblOptimizeOctree.Text = "Optimize octree";
                lblOptimizeOctree.GridRow = 4;
                lblOptimizeOctree.GridColumn = 0;
                grid.Widgets.Add(lblOptimizeOctree);

                var chkOptimizeOctree = new CheckBox();
                chkOptimizeOctree.GridRow = 4;
                chkOptimizeOctree.GridColumn = 1;
                chkOptimizeOctree.IsPressed = game.optimizeOctree;
                chkOptimizeOctree.Click += (s1, a1) =>
                {
                    game.optimizeOctree = chkOptimizeOctree.IsPressed;
                };
                grid.Widgets.Add(chkOptimizeOctree);

                var lblFile = new Label();
                lblFile.Text = "File";
                lblFile.GridRow = 5;
                lblFile.GridColumn = 0;
                grid.Widgets.Add(lblFile);

                var textFile = new TextBox();
                textFile.Text = "";
                textFile.GridRow = 5;
                textFile.GridColumn = 1;
                textFile.Width = 300;
                grid.Widgets.Add(textFile);

                var bttFile = new TextButton();
                bttFile.GridRow = 5;
                bttFile.GridColumn = 2;
                bttFile.Text = "...";
                bttFile.Click += (s1, a1) =>
                {
                    var ofd = new FileDialog(FileDialogMode.OpenFile);
                    ofd.ShowModal();
                    ofd.FilePath = Directory.GetCurrentDirectory() + "\\SampleModels\\";
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
                bttBuild.GridRow = 6;
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
                        {
                            game.textureModel = game.bModel.tex;
                        }
                        else
                        {
                            game.textureModel = game.textureModelDefault;
                        }

                        game.octree = new Octree(game, game.octreeDepth, game.calculatecolor, game.optimizeOctantFaces, game.fillDirection);

                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            game.octree.startTime = DateTime.Now;
                            game.octree.Build(game.bModel);
                            if (game.fillDirection != "No fill")
                            {
                                game.octree.Fill();
                            }
                            if (game.optimizeOctree)
                            {
                                game.octree.Optimize();
                            }
                            game.octree.currentOperation = "";
                            game.octree.endTime = DateTime.Now;
                        }).Start();
                    };
                };

                grid.Widgets.Add(bttBuild);

                _window.ShowModal();
            };

            return _mnuBuildFromModel;
        }

        public MenuItem CreateFill()
        {
            var _mnuFill = new MenuItem();
            _mnuFill.Id = "_mnuFill";
            _mnuFill.Text = "Fill...";
            _mnuFill.Selected += (s, a) =>
            {
                var _window = new Window();
                _window.Title = "Fill";
                _window.Width = 300;

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
                

                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());
                
                var lblFillObject = new Label();
                lblFillObject.Text = "Fill object direction";
                lblFillObject.GridRow = 0;
                lblFillObject.GridColumn = 0;
                grid.Widgets.Add(lblFillObject);

                var comboFillDirection = new ComboBox
                {
                    GridColumn = 1,
                    GridRow = 0
                };

                comboFillDirection.Items.Add(new ListItem("Z+"));
                comboFillDirection.Items.Add(new ListItem("Z-"));
                comboFillDirection.Items.Add(new ListItem("X+"));
                comboFillDirection.Items.Add(new ListItem("X-"));
                comboFillDirection.Items.Add(new ListItem("Y+"));
                comboFillDirection.Items.Add(new ListItem("Y-"));
                comboFillDirection.Items[0].IsSelected = true;
                comboFillDirection.SelectedIndexChanged += (s1, a1) =>
                {
                    game.fillDirection = comboFillDirection.SelectedItem.Text;
                };
                comboFillDirection.Width = 80;
                grid.Widgets.Add(comboFillDirection);

                var bttFill = new TextButton();
                bttFill.GridRow = 2;
                bttFill.GridColumn = 1;
                bttFill.Text = "Fill";
                bttFill.Width = 100;
                bttFill.Click += (s1, a1) =>
                {
                    _window.Close();

                    game.octree.startTime = DateTime.Now;
                    game.octree.Fill();
                    game.octree.currentOperation = "";
                    game.octree.endTime = DateTime.Now;
                };
                
                grid.Widgets.Add(bttFill);

                _window.ShowModal();
            };

            return _mnuFill;
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
                grid.RowsProportions.Add(new Proportion());
                grid.RowsProportions.Add(new Proportion());

                // Add widgets
                game.octree.CalculateProperties();

                var lblArea = new Label();
                lblArea.Text = string.Format("Area: {0}", game.octree.area.ToString("000.00"));
                lblArea.GridRow = 0;
                lblArea.GridColumn = 0;
                grid.Widgets.Add(lblArea);

                var lblVolume = new Label();
                lblVolume.Text = string.Format("Volume: {0}", game.octree.volume.ToString("000.00"));
                lblVolume.GridRow = 1;
                lblVolume.GridColumn = 0;
                grid.Widgets.Add(lblVolume);

                var lblFilledOctants = new Label();
                lblFilledOctants.Text = string.Format("Filled octants: {0}", game.octree.octantsFilled);
                lblFilledOctants.GridRow = 2;
                lblFilledOctants.GridColumn = 0;
                grid.Widgets.Add(lblFilledOctants);

                var lblVertices = new Label();
                lblVertices.Text = string.Format("Vertices: {0}", game.octree.verticesNumber);
                lblVertices.GridRow = 3;
                lblVertices.GridColumn = 0;
                grid.Widgets.Add(lblVertices);

                var lblFaces = new Label();
                lblFaces.Text = string.Format("Faces: {0}", game.octree.verticesNumber / 3);
                lblFaces.GridRow = 4;
                lblFaces.GridColumn = 0;
                grid.Widgets.Add(lblFaces);

                var lblSize = new Label();
                lblSize.Text = string.Format("Size: <{0},{1},{2}>", (game.octree.bb.Max.X - game.octree.bb.Min.X).ToString("00.00").Replace(",", "."), (game.octree.bb.Max.Y - game.octree.bb.Min.Y).ToString("00.00").Replace(",", "."), (game.octree.bb.Max.Z - game.octree.bb.Min.Z).ToString("00.00").Replace(",", "."));
                lblSize.GridRow = 5;
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