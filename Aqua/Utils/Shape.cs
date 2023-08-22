using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aqua
{
    

    public class Shape
    {
        private VertexPositionNormalColorTexture[] _vertices;
        public VertexPositionNormalColorTexture[] Vertices
        {
            get { return _vertices; }
        }

        public float Width
        {
            get { return topRight.X-topLeft.X; }
        }

        private float vHeight = Properties.Settings.Default.vHeight;
        private Color col;
        public Color Col
        {
            get { return col; }
        }


        private BoundingBox _boundingBox;
        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
        }

        public Vector2 topLeft
        {
            get { return new Vector2(_vertices[0].Position.X, _vertices[0].Position.Y); }
        }

        public Vector2 topRight
        {
            get { return new Vector2(_vertices[2].Position.X, _vertices[2].Position.Y); }
        }

        public Vector2 bottomLeft
        {
            get { return new Vector2(_vertices[3].Position.X, _vertices[3].Position.Y); }
        }

        public Vector2 bottomRight
        {
            get { return new Vector2(_vertices[4].Position.X, _vertices[4].Position.Y); }
        }


        private bool _mouseOver;
        public bool mouseOver
        {
            get { return _mouseOver; }
            set
            {
                _mouseOver = value;
                if (_mouseOver)
                {
                    for (int i = 0; i < _vertices.Length; i++)
                        _vertices[i].Color = new Color(Color.Red, 1f);
                }
                else
                {
                    for (int i = 0; i < _vertices.Length; i++)
                        _vertices[i].Color = col;
                }
            }
        }

        public Shape(float x, float y, Color c)
        {
            col = c;
            

            VertexPositionNormalColorTexture[] vertices = new VertexPositionNormalColorTexture[36];
            Vector2 Texcoords = new Vector2(0f, 0f);

            Vector3[] face = new Vector3[6];
            //TopLeft
            face[0] = new Vector3(-vHeight, vHeight, 0.0f);
            //BottomLeft
            face[1] = new Vector3(-vHeight, -vHeight, 0.0f);
            //TopRight
            face[2] = new Vector3(vHeight, vHeight, 0.0f);
            //BottomLeft
            face[3] = new Vector3(-vHeight, -vHeight, 0.0f);
            //BottomRight
            face[4] = new Vector3(vHeight, -vHeight, 0.0f);
            //TopRight
            face[5] = new Vector3(vHeight, vHeight, 0.0f);


            

            //front face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i] = new VertexPositionNormalColorTexture(face[i] + Vector3.UnitZ, Vector3.UnitZ, col, Texcoords);
                vertices[i + 3] = new VertexPositionNormalColorTexture(face[i + 3] + Vector3.UnitZ, Vector3.UnitZ, col, Texcoords);
            }

            //back face

            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 6] = new VertexPositionNormalColorTexture(face[2 - i] - Vector3.UnitZ, -Vector3.UnitZ, col, Texcoords);
                vertices[i + 6 + 3] = new VertexPositionNormalColorTexture(face[5 - i] - Vector3.UnitZ, -Vector3.UnitZ, col, Texcoords);
            }

            //left face
            Matrix RotY90 = Matrix.CreateRotationY(-(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 12] = new VertexPositionNormalColorTexture(Vector3.Transform(face[i], RotY90) - Vector3.UnitX, -Vector3.UnitX, col, Texcoords);
                vertices[i + 12 + 3] = new VertexPositionNormalColorTexture(Vector3.Transform(face[i + 3], RotY90) - Vector3.UnitX, -Vector3.UnitX, col, Texcoords);
            }

            //Right face

            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 18] = new VertexPositionNormalColorTexture(Vector3.Transform(face[2 - i], RotY90) + Vector3.UnitX, Vector3.UnitX, col, Texcoords);
                vertices[i + 18 + 3] = new VertexPositionNormalColorTexture(Vector3.Transform(face[5 - i], RotY90) + Vector3.UnitX, Vector3.UnitX, col, Texcoords);

            }

            //Top face

            Matrix RotX90 = Matrix.CreateRotationX(-(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 24] = new VertexPositionNormalColorTexture(Vector3.Transform(face[i], RotX90) + Vector3.UnitY, Vector3.UnitY, col, Texcoords);
                vertices[i + 24 + 3] = new VertexPositionNormalColorTexture(Vector3.Transform(face[i + 3], RotX90) + Vector3.UnitY, Vector3.UnitY, col, Texcoords);

            }

            //Bottom face

            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 30] = new VertexPositionNormalColorTexture(Vector3.Transform(face[2 - i], RotX90) - Vector3.UnitY, -Vector3.UnitY, col, Texcoords);
                vertices[i + 30 + 3] = new VertexPositionNormalColorTexture(Vector3.Transform(face[5 - i], RotX90) - Vector3.UnitY, -Vector3.UnitY, col, Texcoords);
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position = Vector3.Transform(new Vector3(vertices[i].Position.X, vertices[i].Position.Y, vertices[i].Position.Z), Matrix.CreateTranslation(x, y, 0));
            }

            
            _vertices = vertices;
            mouseOver = false;
            CreateBoundingBox();
        }

        public Shape(float x, float y, Color c, float width) : this(x,y,c)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                //_vertices[i].Position = Vector3.Transform(new Vector3(_vertices[i].Position.X, _vertices[i].Position.Y, _vertices[i].Position.Z), Matrix.CreateScale(width, 1, 1));
                _vertices[i].Position = Vector3.Transform(new Vector3(_vertices[i].Position.X, _vertices[i].Position.Y, _vertices[i].Position.Z), Matrix.CreateScale(width, 1, 1));
                _vertices[i].Position = Vector3.Transform(new Vector3(_vertices[i].Position.X, _vertices[i].Position.Y, _vertices[i].Position.Z), Matrix.CreateTranslation(-width*x+x, 0, 0));
            }

            CreateBoundingBox();

            //System.Diagnostics.Debug.WriteLine(string.Format("x:{0}, y:{1}, color:{2}, width:{3}", x, y, c, width));
        }

        public bool CheckRayIntersection(Ray ray)
        {
            if (ray.Intersects(_boundingBox) != null) return true; 
            else return false;
        }

        public void MoveY(float val)
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i].Position = Vector3.Transform(new Vector3(_vertices[i].Position.X, _vertices[i].Position.Y, _vertices[i].Position.Z), Matrix.CreateTranslation(0, val, 0));
            }

            CreateBoundingBox();
        }

        public void SetY(float val)
        {
            float minY = _vertices.Min(x => x.Position.Y);
            float maxY = _vertices.Max(x => x.Position.Y);
            float height = maxY - minY;

            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i].Position = Vector3.Transform(new Vector3(_vertices[i].Position.X, _vertices[i].Position.Y == minY ? -height / 2 : height / 2, _vertices[i].Position.Z), Matrix.Identity);
                _vertices[i].Position = Vector3.Transform(new Vector3(_vertices[i].Position.X, _vertices[i].Position.Y, _vertices[i].Position.Z), Matrix.CreateTranslation(0, val, 0));
            }

            CreateBoundingBox();
        }

        private void CreateBoundingBox()
        {
            Vector3[] vertexs = new Vector3[_vertices.Length];
            for (int index = 0; index < vertexs.Length; index++)
            {
                vertexs[index] = _vertices[index].Position;
            }
            _boundingBox = BoundingBox.CreateFromPoints(vertexs);
        }


    }
}
