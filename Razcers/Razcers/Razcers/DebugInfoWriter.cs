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
    public class DebugInfoWriter : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        RasterizerState rasterState;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        string fps;

        List<string> texts;


        public DebugInfoWriter(Game game) : base (game)
        {
            texts = new List<string>();
            Initialize();
        }

        
        public override void Initialize()
        {
 	        base.Initialize();
            rasterState = new RasterizerState();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteFont = Game.Content.Load<SpriteFont>("SpriteFont1");
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public int AddText(string textToAdd)
        {
            texts.Add(textToAdd);

            return (texts.Count() - 1);
        }

        public void UpdateTextAtIndex(int index, string text)
        {
            texts[index] = text;
        }

        public override void  Draw(GameTime gameTime)
        {
            int lines = 15;
            frameCounter++;

            fps = frameRate.ToString();

            rasterState = Game.GraphicsDevice.RasterizerState;
            DepthStencilState depthState = Game.GraphicsDevice.DepthStencilState;

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, "FPS: " + fps, new Vector2(42, 32), Color.Black);
            spriteBatch.DrawString(spriteFont, "FPS: " + fps, new Vector2(40, 30), Color.White);

            for (int i = 0; i < texts.Count(); i++)
            {
                spriteBatch.DrawString(spriteFont, texts[i], new Vector2(42, 32 + lines), Color.Black);
                spriteBatch.DrawString(spriteFont, texts[i], new Vector2(40, 30 + lines), Color.White);
                lines += 15;
            }
            spriteBatch.End();

            Game.GraphicsDevice.RasterizerState = rasterState;
            Game.GraphicsDevice.DepthStencilState = depthState;
            
        }
    }
}