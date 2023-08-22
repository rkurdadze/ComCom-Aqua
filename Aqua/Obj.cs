using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Forms.Components;

namespace Aqua
{
    /// <summary>
    /// Very basic class for manipulating objects in a 3d world
    /// </summary>
    public class DrawableObject
    {

        #region Fields

        protected Model model;
        protected Matrix[] modelTransforms;

        protected Vector3 position = Vector3.Zero;
        protected Matrix rotation = Matrix.Identity;
        protected float scale = 1f;

        #endregion

        #region Initialization

        public DrawableObject(Model model, Vector3 position, Matrix rotation, float scale)
        {
            this.model = model;

            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            modelTransforms = new Matrix[model.Bones.Count];
        }

        #endregion

        #region Draw

        public virtual void Draw(Camera camera)
        {

            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = camera.Projection;
                    effect.View = camera.View;
                    effect.World = modelTransforms[mesh.ParentBone.Index] * GetWorld();
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        #endregion

        #region Misc. Functions

        public bool CheckRayIntersection(Ray ray)
        {
            BoundingSphere boundingSphere;

            foreach (ModelMesh mesh in model.Meshes)
            {
                boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * GetWorld());
                if (ray.Intersects(boundingSphere) != null) return true;
            }
            return false;
        }

        public virtual Matrix GetWorld()
        {

            return Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
        }

        #endregion

        #region Properties

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }

        }
        public Matrix Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }

        }
        public float ModelScale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }

        }

        #endregion

    }
}
