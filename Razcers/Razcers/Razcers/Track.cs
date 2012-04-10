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
    public class TrackWeather
    {
        public Vector3 lightDir;
        public Color lightColor;
        public Color lightAmbSky;
        public Color lightAmbGround;
        public float fogDistance;
        public Color fogColor;

        public TrackWeather():
            this(new Vector3(0, 0, -1), Color.Yellow, Color.CornflowerBlue,
            Color.SaddleBrown, 10000, new Color(0, 0, 0, 0))
        {}

        public TrackWeather(Vector3 lightDir, Color lightColor, 
            Color lightAmbSky, Color lightAmbGround, float fogDistance, 
            Color fogColor)
        {
            this.lightDir = lightDir;
            this.lightAmbSky = lightAmbSky;
            this.lightAmbGround = lightAmbGround;
            this.fogDistance = fogDistance;
            this.fogColor = fogColor;
        }
    }


    public class Track
    {
        //heightmap;
        //minimap;
        //landscape geometry;
        //decorations;
        //landscape textures;
        //collectable positions;
        //start positions;
        //restart positions;
        //waypoint regions;
        //billboard locations;
        //checkCollision(position, velocity);
        //getRegion(position);
        //draw(graphicsdevice, camera)
        //skybox

        Effect effect;
        VertexDeclaration vertexDeclaration;
        VertexPositionNormalTexture[] indexedVertices;
        VertexBuffer vertexBuffer;
        short[] indices;
        IndexBuffer indexBuffer;


        Texture2D heightMap;

        public int totalRegions;
        public bool contentSmallLoaded = false;
        public bool contentLargeLoaded = false;

        public TrackWeather weather;

        public Track()
        {
            weather = new TrackWeather();
        }

        /// <summary>
        /// Loads initial, 'preview' content, such as the minimap
        /// </summary>
        /// <param name="content"></param>
        public void LoadContentSmall(ContentManager content)
        {
            //load small items(minimap, other?)
            contentSmallLoaded = true;
        }

        /// <summary>
        /// Loads larger content, eg: textures, geometry buffers, effects
        /// </summary>
        /// <param name="Content">Game instance's content manager</param>
        /// <param name="graphicsDevice">Game instance's graphicsDevice</param>
        public void LoadContentLarge(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            heightMap = Content.Load<Texture2D>("perlin-noise");

            effect = Content.Load<Effect>("Effect2");

            effect.Parameters["Terrain"].SetValue(Content.Load<Texture2D>("leopard"));

            effect.Parameters["dirLight1"].SetValue(weather.lightDir);
            effect.Parameters["dirColor1"].SetValue(weather.lightColor.ToVector3());
            effect.Parameters["dirIntensity1"].SetValue(0.3f);
            effect.Parameters["ambIntensity"].SetValue(0.3f);
            effect.Parameters["ambLight"].SetValue(weather.lightAmbSky.ToVector3());

            initLandscape(graphicsDevice, 256, 256, 1, -32, 32);

            contentLargeLoaded = true;
        }

        private void initLandscape(GraphicsDevice graphicsDevice, int x, int y, float gridScale, float minHeight, float maxHeight)
        {

            Color[] heights = new Color[x * y];
            Vector3[] positions = new Vector3[x * y];

            heightMap.GetData(0, new Rectangle(100, 100, x, y), heights, 0, x * y);

            float xOff = (float)x / 2;
            float yOff = (float)y / 2;

            indices = new short[(x - 1) * (y - 1) * 6];

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if ((j < (x - 1)) && (i < (y - 1)))
                    {
                        indices[6 * (i * (x - 1) + j) + 0] = (short)(x * (i + 0) + (j + 0));
                        indices[6 * (i * (x - 1) + j) + 1] = (short)(x * (i + 1) + (j + 0));
                        indices[6 * (i * (x - 1) + j) + 2] = (short)(x * (i + 0) + (j + 1));
                        indices[6 * (i * (x - 1) + j) + 3] = (short)(x * (i + 0) + (j + 1));
                        indices[6 * (i * (x - 1) + j) + 4] = (short)(x * (i + 1) + (j + 0));
                        indices[6 * (i * (x - 1) + j) + 5] = (short)(x * (i + 1) + (j + 1));
                    }
                    float height = (maxHeight - minHeight) * ((float)heights[y * i + j].G / 255) + minHeight;
                    positions[y * i + j] = new Vector3((i - yOff) * gridScale, height, (j - xOff) * gridScale);
 
                }
            }
            
            vertexDeclaration = new VertexDeclaration(new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Vector2,VertexElementUsage.TextureCoordinate, 0)
                }
            );

            indexedVertices = new VertexPositionNormalTexture[x * y];

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    Vector3 norm = calcNormal(x, y, positions, i, j);
                    
                    indexedVertices[y * i + j].Position = positions[y * i + j];
                    indexedVertices[y * i + j].Normal = norm;
                    indexedVertices[y * i + j].TextureCoordinate = new Vector2(i / (float)x, j / (float)y);

                }

            }

            vertexBuffer = new VertexBuffer(
                graphicsDevice,
                vertexDeclaration,
                indexedVertices.Length,
                BufferUsage.None
                );
            //vertexBuffer.SetData<VertexPositionColor>(vertices);

            indexBuffer = new IndexBuffer(graphicsDevice,
                IndexElementSize.SixteenBits,
                indices.Length,
                BufferUsage.None
                );

            indexBuffer.SetData<short>(indices);


        }

        private Vector3 calcNormal(int x, int y, Vector3[] positions, int i, int j)
        {

            Vector3 xVec1 = new Vector3();
            Vector3 xVec2 = new Vector3();
            int xDiv = 1;

            Vector3 yVec1 = new Vector3();
            Vector3 yVec2 = new Vector3();
            int yDiv = 1;


            indexedVertices[y * i + j] = new VertexPositionNormalTexture();

            //calc x vector
            xDiv = 2; //default
            if (j == 0) //on left edge
            {
                xVec2 = positions[y * i + j]; //so change point and averager
                xDiv = 1;
            }
            else //not on left edge
            {
                xVec2 = positions[y * i + (j - 1)]; //so use offset point
            }

            if (j == (x - 1))// on right edge
            {
                xVec1 = positions[y * i + j];  //so change point and averager
                xDiv = 1;
            }
            else //not on right edge
            {
                xVec1 = positions[y * i + (j + 1)]; //so use offset point
            }
            //Vector3.Normalize(((xVec1 - xVec2) / xDiv));


            //calc y vector
            yDiv = 2; //default
            if (i == 0) //on top edge
            {
                yVec1 = positions[y * i + j]; //so change point and averager
                yDiv = 1;
            }
            else //not on top edge
            {
                yVec1 = positions[y * (i - 1) + j]; //so use offset point
            }

            if (i == (y - 1))// on bottom edge
            {
                yVec2 = positions[y * i + j];  //so change point and averager
                yDiv = 1;
            }
            else //not on bottom edge
            {
                yVec2 = positions[y * (i + 1) + j]; //so use offset point
            }

            //calc cross
            Vector3 norm = Vector3.Cross(
                Vector3.Normalize((xVec1 - xVec2) / xDiv),
                Vector3.Normalize((yVec1 - yVec2) / yDiv));
            norm = (norm + new Vector3(1, 1, 1)) / 2;
            return norm;
        }



        /// <summary>
        /// Detect an imminent collision from a position and a velocity
        /// </summary>
        /// <param name="position">map position</param>
        /// <param name="velocity">velocity </param>
        /// <returns>true if collision will occur within the next frame;</returns>
        public bool CheckCollision(Vector3 position, Vector3 velocity)
        {
            //determine if position and velocity will result in a collision
            //determine collisions with decorator objects
            return false;
        }

        /// <summary>
        /// Gets a position's numerical progression through areas of the track.
        /// This allows a race manager to track a player's advancement along
        /// the racetrack, detect lap completion, etc.
        /// </summary>
        /// <param name="position">The position for which to determine the region</param>
        /// <returns>region number, from zero to (totalRegions - 1)</returns>
        public int GetRegion(Vector3 position)
        {
            return 0;
        }


        public void Draw(GraphicsDevice graphicsDevice, ChaseCamera camera)
        {
            //draw skybox, then flush depth buffer or some such
            //draw landscape geometry
            //draw oranment models
            //draw lens flare?


            //Matrix world = Matrix.CreateWorld(-player.position, Vector3.UnitZ, Vector3.UnitY);
            Matrix world = Matrix.Identity;
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(camera.view);
            effect.Parameters["Projection"].SetValue(camera.projection);

            effect.CurrentTechnique = effect.Techniques["Technique1"];

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    indexedVertices,
                    0,   // vertex buffer offset to add to each element of the index buffer
                    indexedVertices.Length,  // number of vertices to draw
                    indices,
                    0,   // first index element to read
                    indices.Length / 3   // number of primitives to draw
                );
            }
        }


    }
}
