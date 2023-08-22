using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Forms.Controls;

namespace Aqua
{
    /// <summary>
    /// basic Free look camera
    /// controlled from the outside
    /// </summary>
    public class Camera
    {

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        protected Matrix world = Matrix.Identity;

        /// <summary>
        /// 
        /// </summary>
        protected Matrix view;

        /// <summary>
        /// 
        /// </summary>
        protected Matrix projection;

        /// <summary>
        /// 
        /// </summary>
        protected Vector3 position;

        /// <summary>
        /// 
        /// </summary>
        protected Vector3 target;

        /// <summary>
        /// 
        /// </summary>
        protected float farPlane = 5000f;

        /// <summary>
        /// 
        /// </summary>
        protected float nearPlane = 100f;

        /// <summary>
        /// 
        /// </summary>
        protected float yaw = 0f;
        /// <summary>
        /// 
        /// </summary>
        protected float pitch = 0f;

        /// <summary>
        /// 
        /// </summary>
        protected Ray mouseRay;

        /// <summary>
        /// 
        /// </summary>
        protected GraphicsDevice device;

        private BoundingFrustum _bf;

        private float YawDegrees
        {
            get
            {
                float deg = MathHelper.ToDegrees(this.yaw);
                if (deg == 180) return 0;
                else if (deg < 0) return deg + 180;
                else return 180 - deg;
            }
        }

        private float PitchDegrees
        {
            get
            {
                float deg = MathHelper.ToDegrees(this.pitch);
                return deg;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        /// <param name="game"></param>
        public Camera(Vector3 position, Vector3 target, float near, float far, UpdateWindow game)
        {
            if (position == target) target.Z += 10f;

            this.position = position;
            this.target = target;

            this.device = game.GraphicsDevice;
            this.nearPlane = near;
            this.farPlane = far;

            // If the camera's looking straight down it has to be fixed

            CalculateYawPitch();

            while (Math.Abs(pitch) >= MathHelper.ToRadians(80))
            {
                this.position.Z += 10;
                CalculateYawPitch();
            }

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, nearPlane, farPlane);
            view = Matrix.CreateLookAt(this.position, target, this.Up);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void CalculateYawPitch()
        {
            Vector3 dir = target - position;
            dir.Normalize();
            Vector3 m = dir; m.Y = position.Y;

            yaw = (float)Math.Atan2(dir.X, dir.Z);

            float len = (new Vector2(m.X, m.Z)).Length();
            pitch = (float)Math.Atan2(dir.Y, len);
        }

        #endregion

        #region Ray calculation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public Ray GetMouseRay(Vector2 mousePosition, Viewport viewport)
        {
            Vector3 near = new Vector3(mousePosition, 0);
            Vector3 far = new Vector3(mousePosition, 1);

            near = viewport.Unproject(near, projection, view, Matrix.Identity);
            far = viewport.Unproject(far, projection, view, Matrix.Identity);

            return new Ray(near, Vector3.Normalize(far - near));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="viewport"></param>
        public virtual void UpdateMouseRay(Vector2 mousePos, Viewport viewport)
        {
            mouseRay = this.GetMouseRay(mousePos, viewport);
        }

        #endregion

        #region Update

        /// <summary>
        /// 
        /// </summary>
        public virtual void Update()
        {
            view = Matrix.CreateLookAt(position, target, this.Up);

            _bf = new BoundingFrustum(View * Projection);

            // we could just put UpdateMouseRay here but that well calculate the ray even when we don't need it
        }

        #endregion

        #region Cam Movement

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public virtual void MoveForward(float amount)
        {
            Vector3 temp = position;
            if (temp.Z > 10)
            {
                position += amount * this.Direction;
                target += amount * this.Direction;
                this.Update();
            }
        }

        public virtual void MoveBackward(float amount)
        {
            Vector3 temp = position;
            if (temp.Z < 200)
            {
                position -= amount * this.Direction;
                target -= amount * this.Direction;
                this.Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public virtual void MoveAlongX(float amount)
        {
            amount = amount*position.Z;            
            position.X += amount;
            target.X += amount;
            this.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public virtual void MoveAlongY(float amount)
        {
            amount = amount * position.Z;
            position.Y += amount;
            target.Y += amount;
            this.Update();
        }

        



        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public virtual void Starfe(float amount)
        {
            /*Vector3 temp = amount * this.Right;
            temp *= Direction*10f;
            position += temp;
            target += temp;
            this.Update();*/

            
            position += amount * this.Right;
            target += amount * this.Right;
            this.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public virtual void AddYaw(float angle)
        {
            if (180 - Math.Abs(MathHelper.ToDegrees(yaw + angle)) > 40) return;
            yaw += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(this.Up, angle));

            target = position + Vector3.Distance(target, position) * dir;
            CalculateYawPitch();
            this.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public virtual void AddPitch(float angle)
        {
            if (Math.Abs(MathHelper.ToDegrees(angle + pitch)) > 40) return;
            pitch += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(this.Right, angle));

            target = position + Vector3.Distance(target, position) * dir;
            CalculateYawPitch();
            this.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public virtual void Levitate(float amount)
        {
            position.Y += amount;
            target.Y += amount;
            this.Update();
        }

        #endregion

        #region Properties
        public Matrix World
        {
            get
            {
                return world;
            }
        }


        
        /// <summary>
        /// 
        /// </summary>
        public BoundingFrustum GetFrustum
        {
            get
            {
                return _bf;
                //return new BoundingFrustum(View * Projection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix View
        {
            get
            {
                return view;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix Projection
        {
            get
            {
                return projection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                return Vector3.Normalize(target - position);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Up
        {
            get
            {
                return Vector3.Up;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(this.Direction, this.Up));
                //return Vector3.Normalize(Vector3.Cross(this.Up, this.Direction));
            }
        }
        /*
        public float AngleX
        {
            get
            {
                Vector3 first = Vector3.Right;
                first.Normalize();
                Vector3 second = this.Direction;
                second.Normalize();
                float dotproduct = Vector3.Dot(first, second);// Take arc cosine of our dot product to give us the angle
                float angle = (float)Math.Acos(dotproduct);
                float degrees = angle * 180 / MathHelper.Pi;   // Convert radians to degrees
                if (degrees == 90) return 0;
                else if (degrees < 90) return 90 - degrees;
                else return 90-degrees;
            }
        }

        public float AngleY
        {
            get
            {
                Vector3 first = Vector3.Down;
                first.Normalize();
                Vector3 second = this.Direction;
                second.Normalize();
                float dotproduct = Vector3.Dot(first, second);// Take arc cosine of our dot product to give us the angle
                float angle = (float)Math.Acos(dotproduct);
                float degrees = angle * 180 / MathHelper.Pi;   // Convert radians to degrees
                if (degrees == 90) return 0;
                else if (degrees < 90) return 90 - degrees;
                else return 90 - degrees;
            }
        }
        */
        

        /// <summary>
        /// 
        /// </summary>
        public Ray MouseRay
        {
            get
            {
                return mouseRay;
            }
        }

        public float ScaleFactor
        {
            get
            {
                return position.Z * Properties.Settings.Default.vHeight / 40f;
            }
        }

        #endregion

        public Vector3 pointToWorld(Viewport vp, float x, float y)
        {
            //  Unproject the screen space mouse coordinate into model space 
            //  coordinates. Because the world space matrix is identity, this 
            //  gives the coordinates in world space.

            //  Note the order of the parameters! Projection first.
            Vector3 pos1 = vp.Unproject(new Vector3(x, y, 0), this.Projection, this.View, this.World);
            Vector3 pos2 = vp.Unproject(new Vector3(x, y, 1), this.Projection, this.View, this.World);
            Vector3 dir = Vector3.Normalize(pos2 - pos1);
            //System.Diagnostics.Debug.WriteLine(dir);
            return dir;
        }
    }
}
