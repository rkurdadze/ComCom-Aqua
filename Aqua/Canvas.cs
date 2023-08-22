using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Forms.Controls;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.Components;
using System.Diagnostics;
using System.Linq;

namespace Aqua
{
    #region event delegates
    public delegate void CubesChangedEventHandler(object sender, StringValueChangedEventArgs e);
    #endregion

    public class Canvas : UpdateWindow
    {

        #region events
        public event CubesChangedEventHandler CubesValueChanged;
        protected virtual void OnCubesChanged(StringValueChangedEventArgs e)
        {
            if (CubesValueChanged != null)
                CubesValueChanged?.Invoke(this, e);
        }
        #endregion

        #region Private Fields
        Camera camera;
        private System.ComponentModel.BackgroundWorker backgroundWorker;


        GraphicsDevice graphics;
        SpriteBatch spriteBatch;
        BasicEffect effect;
        List<Shape> cubes = new List<Shape>();
        List<Shape> VisibleCubes = new List<Shape>();


        // Input previous states
        MouseState prevMouseState;
        Vector2 mousePos = Vector2.Zero;
        Vector3 tempMousePosition = Vector3.Zero;
        private int previousScrollValue;
        #endregion

        #region Public Fields
        public Animation Logo;
        public bool EditMode = false;
        public int LastFrame = 0;
        #endregion



        protected override void Initialize()
        {
            base.Initialize();

         
            graphics = Editor.graphics;
            Editor.ShowCursorPosition = true;

            camera = new Camera(new Vector3(0, 5, 40), Vector3.Zero, 1f, 1000f, this);

            Logo = new Animation(Editor.Content.Load<Texture2D>("Logo_Sheet"), 10, 10, 0.5f, true, true);
            Editor.BackgroundColor = new Color(20, 19, 40);
            previousScrollValue = Mouse.GetState().ScrollWheelValue;


            effect = new BasicEffect(graphics);
            effect.LightingEnabled = true;
            effect.AmbientLightColor = Color.White.ToVector3();
            effect.VertexColorEnabled = true;
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();//Vector3.One;
            effect.DirectionalLight0.Direction = new Vector3(2f, -1.5f, -1f);
            effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();



            effect.World = camera.World;
            effect.Projection = camera.Projection;
            effect.View = camera.View;

            //Loading Content
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics);

            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5/*r.Next(100, 200)*/; j++)
                {
                    Color col = new Color(r.Next(10, 100), r.Next(10, 100), r.Next(10, 100));
                    cubes.Add(new Shape((float)i * 4f, -(float)j* 4f, col, r.Next(2, 5)));
                }
            }

            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;  //Tell the user how the process went
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        protected override void Update(GameTime gameTime)
        {

            MouseState mouseState = Mouse.GetState();
            #region Cam FreeLook

            // Detect right mouse button up/down 
            Viewport activeViewport = Editor.graphics.Viewport;

            //pitch Yaw
            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Pressed)
            {
                Vector3 msWorld = camera.pointToWorld(activeViewport, mouseState.X, mouseState.Y);
                camera.AddYaw(msWorld.X - tempMousePosition.X);
                camera.AddPitch(-msWorld.Y + tempMousePosition.Y);
                tempMousePosition = camera.pointToWorld(activeViewport, mouseState.X, mouseState.Y);
            }
            else if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
            {
                tempMousePosition = camera.pointToWorld(activeViewport, mouseState.X, mouseState.Y);
            }


            //move in 2D
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector3 msWorld = camera.pointToWorld(activeViewport, mouseState.X, mouseState.Y);
                //camera.Move2D(msWorld-tempMousePosition);
                Vector3 temp = tempMousePosition - msWorld;
                camera.MoveAlongX(temp.X);
                camera.MoveAlongY(temp.Y);
                tempMousePosition = camera.pointToWorld(activeViewport, mouseState.X, mouseState.Y);


            }
            else if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                tempMousePosition = camera.pointToWorld(activeViewport, mouseState.X, mouseState.Y);
            }


            // Detect Scrolling
            if (mouseState.ScrollWheelValue < previousScrollValue)
            {
                camera.MoveForward(5f);
                previousScrollValue = mouseState.ScrollWheelValue;
                //foreach (Shape s in cubes) s.UpdateScale(camera.ScaleFactor);
            }
            else if (mouseState.ScrollWheelValue > previousScrollValue)
            {
                camera.MoveBackward(5f);
                previousScrollValue = mouseState.ScrollWheelValue;
                //foreach (Shape s in cubes) s.UpdateScale(camera.ScaleFactor);
            }

            camera.UpdateMouseRay(new Vector2(mouseState.X, mouseState.Y), graphics.Viewport);
            foreach (Shape s in VisibleCubes)
            {
                if (s.CheckRayIntersection(camera.MouseRay))
                {
                    s.mouseOver = true;
                    Debug.WriteLine(string.Format("topleft:{0}, topr:{1}, bottoml:{2}, bottomr:{3}, width:{4}", s.topLeft, s.topRight, s.bottomLeft, s.bottomRight, s.Width));
                }
                else s.mouseOver = false;
            }


            camera.Update();
            effect.Projection = camera.Projection;
            effect.View = camera.View;
            #endregion

            prevMouseState = mouseState;


            if (!backgroundWorker.IsBusy) backgroundWorker.RunWorkerAsync();
            base.Update(gameTime);
        }

        protected override void Draw()
        {
            base.Draw();

            //GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            rs.FillMode = FillMode.Solid;
            graphics.RasterizerState = rs;
            graphics.DepthStencilState = DepthStencilState.Default;

           

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                for (int i = 0; i < VisibleCubes.Count; i++)
                {
                    graphics.DrawUserPrimitives<VertexPositionNormalColorTexture>(PrimitiveType.TriangleList, VisibleCubes[i].Vertices, 0, 12);
                }
            }


            /*
            Editor.spriteBatch.Begin();

            Editor.spriteBatch.Draw(Logo.Texture, new Rectangle(
                       Editor.graphics.Viewport.Width / 2,
                       Editor.graphics.Viewport.Height / 2,
                       Logo.PartSizeX,
                       Logo.PartSizeY),
                       (EditMode ? Logo.GetCurrentFrame() : Logo.DoAnimation()),
                       Logo.GetDrawingColor, 
                       0f, 
                       Logo.GetOrigin, 
                       SpriteEffects.None, 0f);

            Editor.spriteBatch.DrawString(Editor.Font, "WelcomeMessage", new Vector2(
                (Editor.graphics.Viewport.Width / 2) - (Editor.Font.MeasureString("WelcomeMessage").X / 2),
                (Editor.graphics.Viewport.Height / 2) - (Editor.FontHeight / 2) + Logo.GetOrigin.Y + 10),
                Color.White);

            Editor.spriteBatch.End();*/

            Editor.DrawDisplay();
        }



        #region Background Shape Sort
        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            VisibleCubes = SortCubes(cubes.Where(x => camera.GetFrustum.Intersects(x.BoundingBox)).ToList());
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //lblStatus.Text = "Process was cancelled";
            }
            else if (e.Error != null)
            {
                //lblStatus.Text = "There was an error running the process. The thread aborted";
            }
            else
            {
                //lblStatus.Text = "Process was completed";
                OnCubesChanged(new StringValueChangedEventArgs(string.Format("{0} from {1}", VisibleCubes.Count, cubes.Count)));
            }
        }
        #endregion



        protected List<Shape> SortCubes(List<Shape> lstVisible)
        {
            return lstVisible;
            List<Shape> lst = lstVisible;//.OrderBy(x => x.topLeft.X).OrderBy(x => x.Width).ToList();
            
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].SetY(0);
                for (int j = 0; j < i; j++)
                {
                    float step = -4f;
                    Shape s = new Shape(lst[i].topRight.X - lst[i].Width / 2, step * j, lst[i].Col, lst[i].Width);
                    if (!overlappingXY(s, lst[i]))
                    {
                        lst[i].SetY(step * j);
                        break;
                    }                  
                }
            }

            return lst;
        }

        protected bool overlappingXY(Shape s1, Shape s2)
        {
            if (s1.topLeft.Y == s2.topLeft.Y)
            {
                if (s1.topLeft.X.IsWithin(s2.topLeft.X, s2.topRight.X) || s1.topRight.X.IsWithin(s2.topLeft.X, s2.topRight.X))
                {
                    Debug.WriteLine(string.Format("true s1:{0}, s2:{1}", s1.BoundingBox, s2.BoundingBox));
                    return true;
                }
                if (s1.topLeft.X == s2.topLeft.X && s1.topRight.X == s2.topRight.X)
                {
                    Debug.WriteLine(string.Format("true s1:{0}, s2:{1}", s1.BoundingBox, s2.BoundingBox));
                    return true;
                }
                if (s1.topLeft.X < s2.topLeft.X && s1.topRight.X > s2.topRight.X)
                {
                    Debug.WriteLine(string.Format("true s1:{0}, s2:{1}", s1.BoundingBox, s2.BoundingBox));
                    return true;
                }
                return false;
            }
            else return false;
            
        }

        



    }




    public class StringValueChangedEventArgs : EventArgs
    {
        public string NewValue { get; set; }

        public StringValueChangedEventArgs(string newValue)
        {
            NewValue = newValue;
        }
    }


}
