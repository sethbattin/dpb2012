using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Razcers
{
    public class ChaseCamera
    {
        public Matrix view;
        public Matrix projection;
        public Matrix world;

        public Vector3 position;
        private Vector3 positionCurrent;
        public Vector3 forward;
        private Vector3 forwardCurrent;
        public Vector3 up;
        private Vector3 upCurrent;

        public float distance;
        public float aspectRatio;
        private const float xeno = 0.06f;

        public ChaseCamera(Vector3 position, Vector3 forward, Vector3 up, float distance, float aspectRatio)
        {
            this.position = position;
            positionCurrent = position;
            this.forward = forward;
            forwardCurrent = forward;
            this.up = up;
            upCurrent = up;
            this.distance = distance;
            this.aspectRatio = aspectRatio;
        }

        public void Update(GameTime gameTime)
        {
            ChaseVectors(gameTime);
            MakeMatrices();
        }

        private void ChaseVectors(GameTime gameTime)
        {
            positionCurrent = Vector3.Lerp(positionCurrent, position, xeno);
            forwardCurrent = Vector3.Lerp(forwardCurrent, forward, xeno);
            upCurrent = Vector3.Lerp(upCurrent, up, xeno);
        }

        private void MakeMatrices()
        {
            world = Matrix.CreateWorld(positionCurrent - (distance * forwardCurrent), forwardCurrent, upCurrent);
            view = Matrix.CreateLookAt(positionCurrent - (distance * forwardCurrent), positionCurrent + 10 * (distance * forwardCurrent), upCurrent);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 100000);
        }
    }
}
