using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aqua
{
    public struct VertexPositionNormalColorTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;
        public Vector2 TextureCoordinate;

        #region IVertexType Members

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
                new VertexElement[]
                {
                    new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                    new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                });

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
        #endregion

        public VertexPositionNormalColorTexture(Vector3 position, Vector3 normal, Color color, Vector2 textureCordinate)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
            this.TextureCoordinate = textureCordinate;
        }

    }
}
