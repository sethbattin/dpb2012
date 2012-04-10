using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Razcers
{
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Model model;
        private InputState input;
        public ChaseCamera camera;
        private BasicEffect effect;

        public InputState.InputMode inputMode;

        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public Vector3 top;
        public Vector3 left;

        public float agility;
        public float topspeed;
        public float acceleration;
        //public float 


        private PlayerIndex playerIndex = PlayerIndex.One;

        private float speedMax = 15;
        private float speedMin = 1;
        private int inverted = 1;  //or -1;


        public Player(Game game, Model model, InputState input, ChaseCamera camera)
            : base(game)
        {
            this.model = model;
            this.input = input;
            this.camera = camera;
            position = new Vector3(0, 20, 0);
            speed = 0;
            direction = Vector3.UnitZ;
            top = Vector3.UnitY;
            left = Vector3.Cross(top, direction);

            inputMode = InputState.InputMode.Advanced;
            
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        float roll;
        Matrix mroll;
        float pitch;
        Matrix mpitch;
        public override void Update(GameTime gameTime)
        {
            roll = input.GetRoll(playerIndex, inputMode) * inverted / 40f;
            mroll = Matrix.CreateFromAxisAngle( direction, roll);
            pitch = input.GetPitch(playerIndex, inputMode) * inverted / 40f;
            mpitch = Matrix.CreateFromAxisAngle( left, pitch);

            top = Vector3.Transform(top, mroll);
            left = Vector3.Transform(left, mroll);

            direction = Vector3.Transform(direction, mpitch);
            top = Vector3.Transform(top, mpitch);

            speed *= (float)Math.Cos(pitch );
            speed += input.GetThrust(playerIndex, inputMode) * 0.05f;
            if (speed != 0)
            {
                //velocity = Vector3.Dot(velocity, direction) * direction;

                speed = MathHelper.Clamp(speed, speedMin, speedMax);

                position += direction * speed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }

            if (input.IsNewButtonPress(Buttons.Y, playerIndex)) speed = 0;

            camera.position = position;
            camera.forward = direction;
            camera.up = top;


        }

        public override void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            ModelMesh mesh;

            //foreach (ModelMesh mesh in model.Meshes)
            //{

            mesh = model.Meshes[0];

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * 
                        Matrix.CreateScale(.01f) * 
                        Matrix.CreateWorld(position, direction, top);
                    //effect.World = transforms[mesh.ParentBone.Index] * camera.world;

                    // Use the matrices provided by the chase camera
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }
                mesh.Draw();
            //}

                mesh = model.Meshes[1];
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.World = 
                        transforms[mesh.ParentBone.Index] *
                        Matrix.CreateScale(new Vector3(0.01f, 0.01f, 0.01f)) *
                        Matrix.CreateRotationX(input.LeftAilervator(playerIndex, inputMode)) * 
                        Matrix.CreateWorld(position, direction, top);
                }

                mesh = model.Meshes[2];
                mesh.Draw();
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.World =
                        transforms[mesh.ParentBone.Index] *
                        Matrix.CreateScale(new Vector3(0.01f, 0.01f, 0.01f)) *
                        Matrix.CreateRotationX(input.RightAilervator(playerIndex, inputMode)) * 
                        Matrix.CreateWorld(position, direction, top);
                }

                mesh.Draw();

            base.Draw(gameTime);
        }
    }
}
