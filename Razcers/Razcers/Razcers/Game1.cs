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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ChaseCamera camera;
        InputState input;

        DebugInfoWriter debug;
        int index1;
        int index2;
        int index3;
        int index4;

        Player player;

        Model ship;

        BasicEffect basicEffect;
        Random random;
        VertexPositionColor[] vertices;
        VertexPositionColor[] edges;
        VertexPositionColor[] landscape;

        public Track track;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            camera = new ChaseCamera(
                Vector3.Zero, 
                Vector3.UnitZ, 
                Vector3.UnitY, 
                10, 
                GraphicsDevice.DisplayMode.AspectRatio);

            random = new Random();
            input = new InputState(this);
            debug = new DebugInfoWriter(this);
            index1 = debug.AddText("camera info");
            index2 = debug.AddText("camera info");
            index3 = debug.AddText("camera info");
            index4 = debug.AddText("camera info");

            track = new Track();

            Components.Add(input);
            Components.Add(debug);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            basicEffect = new BasicEffect(GraphicsDevice);
            
            basicEffect.VertexColorEnabled = true;

            track.LoadContentSmall(Content);
            track.LoadContentLarge(Content, GraphicsDevice);

            ship = Content.Load<Model>("BubbleShip");

            player = new Player(this, ship, input, camera);
            Components.Add(player);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            camera.Update(gameTime);

            debug.UpdateTextAtIndex(index1, "pla for: " + player.direction.ToString());
            debug.UpdateTextAtIndex(index2, "pla pos: " + player.position.ToString());
            debug.UpdateTextAtIndex(index3, "pla upp: " + player.top.ToString());

          //  debug.UpdateTextAtIndex(index4, "play speed: " + player.speed.ToString());

            basicEffect.View = camera.view;
            basicEffect.Projection = camera.projection;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            track.Draw(GraphicsDevice, player.camera);

            base.Draw(gameTime);
        }

    }
}
