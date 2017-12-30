using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace CH06___Graphics_3D
{
    // simple helper to encapsulate tedious mesh tasks
    public class MyMesh : My3DObject
    {
        // parent model for this mesh
        protected Model m_Model = null;
        public Model Model
        {
            get { return m_Model; }
            set { m_Model = value; }
        }

        // name of this mesh, within the model
        protected string m_MeshName = "";
        public string MeshName
        {
            get { return m_MeshName; }
            set { m_MeshName = value; }
        }

        // the active shader effect for this mesh
        protected Effect m_Effect = null;
        public Effect Effect
        {
            get { return m_Effect; }
            set { m_Effect = value; }
        }

        // draw this mesh to the screen
        public void Draw(GraphicsDevice device, MyCamera camera, float aspect)
        {
            // copy parent transforms
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            // get a reference to the mesh to save some typing
            ModelMesh mesh = Model.Meshes[MeshName];

            // make sure all our mesh parts are using the current effect
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                part.Effect = Effect;
            }

            // rotate and transform mesh
            float x = MathHelper.ToRadians(Rotation.X);
            float y = MathHelper.ToRadians(Rotation.Y);
            float z = MathHelper.ToRadians(Rotation.Z);

            // matrix to translate this mesh from model
            // space to world space
            Matrix world =
                transforms[mesh.ParentBone.Index] *
                Matrix.CreateRotationY(y) *
                Matrix.CreateRotationX(x) *
                Matrix.CreateRotationZ(z) *
                Matrix.CreateTranslation(Location);

            // set effect parameters
            if (Effect is BasicEffect)
            {
                // effect is a basic (or default XNA) effect
                BasicEffect basic = (BasicEffect)Effect;
                basic.Projection = camera.ProjectionMatrix;
                basic.View = camera.ViewMatrix;
                basic.World = world;
            }
            else
            {
                // effect is one of our custom effects
                Effect.Parameters["WorldTransform"]
                    .SetValue(world);
                Effect.Parameters["WorldViewProjection"]
                    .SetValue(
                        world *
                        camera.ViewMatrix *
                        camera.ProjectionMatrix);
                Effect.Parameters["CameraPosition"]
                    .SetValue(camera.Location);
            }

            //if(rsWireFrame == null)
            //{
            //    rsWireFrame = new RasterizerState();
            //    rsWireFrame.FillMode = FillMode.WireFrame;  
            //}
            if(rsSolid == null)
            {
                rsSolid = new RasterizerState();
                rsSolid.FillMode = FillMode.Solid;
            }

            // reset fill mode; otherwise basic effect will get 
            // confused whenever our custom "wireframe" effect runs
            //device.RenderState.FillMode = FillMode.Solid;
            device.RasterizerState = rsSolid;

            //draw the mesh using the effect options, set above
            mesh.Draw();
        }
        
        //protected RasterizerState rsWireFrame = null;  
        protected RasterizerState rsSolid = null;  
    }
}
