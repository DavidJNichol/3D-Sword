using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;

namespace Vertices
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        VertexBuffer vertexBuffer;                              //Buffer for triangle
        VertexDeclaration basicEffectVertexDeclaration;         //https://docs.microsoft.com/en-us/windows/win32/direct3d9/mapping-fvf-codes-to-a-directx-9-declaration

        Matrix worldMatrix;                                     //Matrix to hold world
        Matrix viewMatrix;                                      //View is the view from the camera
        Matrix projectionMatrix;                                //Projection is the 2D flattened view with occlusion

        BasicEffect effect;                                //ShaderEffect used to draw on video card this is a simple mongame HLSL shader

        //World Transform variables
        #region World Tranfom variables
        float rotationX, orbitX;        //used to rotate and orbit objects
        float rotationY, orbitY;
        Vector3 worldTrans;             //translate is moving an object
        float worldScale;               //scales object in the work
        #endregion


        #region GameServices
        InputHandler input;
        GameConsole gameConsole;
        FPS fps;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Game components from MonogameLibrary.Util
            input = new InputHandler(this);
            gameConsole = new GameConsole(this);
            fps = new FPS(this);
            this.Components.Add(input);
            this.Components.Add(gameConsole);
            this.Components.Add(fps);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            //Setup intial values for World Transform objects
            rotationX = 0.0f;
            rotationY = 0.0f;
            orbitX = 0.0f;
            orbitY = 0.0f;
            worldTrans = Vector3.Zero;
            worldScale = 1.0f;

            // Set up the initial View Matrixes
            // camera
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, -4, 10), new Vector3(0,1,0), Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height,
                1.0f, 100.0f);

            gameConsole.GameConsoleWrite("Translate");
            gameConsole.GameConsoleWrite("w: y+ s:y- a:x- d:x+");
            gameConsole.GameConsoleWrite("Rotate");
            gameConsole.GameConsoleWrite("up down left right");

            gameConsole.GameConsoleWrite("+:scale up");
            gameConsole.GameConsoleWrite("-:scale down");
            gameConsole.GameConsoleWrite("r reset the triangle");

            gameConsole.GameConsoleWrite("3D Sword");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load any ResourceManagementMode.Automatic content
            this.InitializeEffect();
            this.SetUpVertices();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Elapsed time since last update
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //Identity
            worldMatrix = Matrix.Identity;
            //Manipulate world this changes the entire world matrix
            #region worldMatrix
            //Scale
            worldMatrix *= Matrix.CreateScale(worldScale);

            //Rotation
            worldMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(rotationX));
            worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(rotationY));


            //Translation
            worldMatrix *= Matrix.CreateTranslation(worldTrans);

            //Orbit
            worldMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(orbitX));
            worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(orbitY));

            #endregion

            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            #region Input

            //Scale
            if (input.KeyboardState.IsKeyDown(Keys.OemPlus))
            {
                worldScale += (0.001f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.OemMinus))
            {
                worldScale -= (0.001f * time);
            }

            // Rotation
            if (input.KeyboardState.IsKeyDown(Keys.Left))
            {
                rotationX += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Right))
            {
                rotationX -= (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Up))
            {
                rotationY += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Down))
            {
                rotationY -= (0.5f * time);
            }

            //Orbit
            if (input.KeyboardState.IsKeyDown(Keys.Q))
            {
                orbitX += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.E))
            {
                orbitX -= (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.T))
            {
                orbitY += (0.5f * time);
            }
            if (input.KeyboardState.IsKeyDown(Keys.G))
            {
                orbitY -= (0.5f * time);
            }



            //Translation
            if (input.KeyboardState.IsKeyDown(Keys.D))
            {
                worldTrans += new Vector3(0.01F * time, 0, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.A))
            {
                worldTrans -= new Vector3(0.01F * time, 0, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.W))
            {
                worldTrans += new Vector3(0, 0.01f * time, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.S))
            {
                worldTrans -= new Vector3(0, 0.01F * time, 0);
            }


            if (input.KeyboardState.HasReleasedKey(Keys.R))
            {
                //reset world matrix
                rotationX = 0.0f;
                rotationY = 0.0f;
                orbitX = 0.0f;
                orbitY = 0.0f;
                worldTrans = Vector3.Zero;
                worldScale = 1.0f;
            }


            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //graphics.GraphicsDevice.VertexDeclaration = basicEffectVertexDeclaration;
            //graphics.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            // This code would go between a device
            // BeginScene-EndScene block.
            //effect.Begin();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
                //pass.End();
            }
            //effect.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Setup Vertices fro drawing primitives
        /// </summary>
        private void SetUpVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[33];
            vertices[0] = new VertexPositionColor(new Vector3(0.5f, 4.5f, 0), Color.Pink);
            vertices[1] = new VertexPositionColor(new Vector3(-.5f, 0, 0), Color.Black);
            vertices[2] = new VertexPositionColor(new Vector3(.5f, 0, 0), Color.FloralWhite);
            /*
             * Try to add some more vertices
             * remember to change the 
            */
            vertices[3] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Gold);
            vertices[4] = new VertexPositionColor(new Vector3(0f, -1, 0), Color.Silver);
            vertices[5] = new VertexPositionColor(new Vector3(-2, -1f, 0), Color.Gold);

            vertices[6] = vertices[3];
            vertices[7] = vertices[4];
            vertices[8] = new VertexPositionColor(new Vector3(2, -1, 0), Color.Silver);

            vertices[9] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Gold);
            vertices[10] = new VertexPositionColor(new Vector3(-2f, 0, 0), Color.Gold);
            vertices[11] = new VertexPositionColor(new Vector3(-2, -1, 0), Color.Gold);

            vertices[12] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Gold);
            vertices[13] = new VertexPositionColor(new Vector3(2f, 0, 0), Color.Gold);
            vertices[14] = new VertexPositionColor(new Vector3(2, -1, 0), Color.Silver);

            vertices[15] = new VertexPositionColor(new Vector3(.5f, -1f, 0), Color.Black);
            vertices[16] = new VertexPositionColor(new Vector3(-.5f, -1f, 0), Color.Black);
            vertices[17] = new VertexPositionColor(new Vector3(-.15f, -2.5f, 0), Color.SaddleBrown);

            vertices[18] = vertices[15];
            vertices[19] = new VertexPositionColor(new Vector3(.15f, -2.5f, 0), Color.Black);
            vertices[20] = vertices[17];

            vertices[21] = new VertexPositionColor(new Vector3(.23f, -2.5f, 0), Color.Black);
            vertices[22] = new VertexPositionColor(new Vector3(-.23f, -2.5f, 0), Color.Silver);
            vertices[23] = new VertexPositionColor(new Vector3(.23f, -2.8f, 0), Color.Silver);

            vertices[24] = new VertexPositionColor(new Vector3(-.23f, -2.5f, 0), Color.Silver);
            vertices[25] = new VertexPositionColor(new Vector3(-.23f, -2.8f, 0), Color.Black);
            vertices[26] = new VertexPositionColor(new Vector3(.23f, -2.8f, 0), Color.Silver);

            vertices[27] = new VertexPositionColor(new Vector3(-.5f, 4.5f, 0), Color.Pink);
            vertices[28] = new VertexPositionColor(new Vector3(-.5f, 0, 0), Color.Black);
            vertices[29] = new VertexPositionColor(new Vector3(.5f, 4.5f, 0), Color.FloralWhite);

            vertices[30] = new VertexPositionColor(new Vector3(-.5f, 4.5f, 0), Color.Pink);
            vertices[31] = new VertexPositionColor(new Vector3(0, 7, 0), Color.Silver);
            vertices[32] = new VertexPositionColor(new Vector3(.5f, 4.5f, 0), Color.FloralWhite);

            //vertexBuffer = new VertexBuffer(this.graphics.GraphicsDevice, 
            //       VertexPositionColor.SizeInBytes * vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor),
                vertices.Length, BufferUsage.WriteOnly | BufferUsage.None);
            vertexBuffer.SetData(vertices);
        }

        private void InitializeEffect()
        {
            //basicEffectVertexDeclaration = new VertexDeclaration(
            //    graphics.GraphicsDevice, VertexPositionColor.VertexElements);

            basicEffectVertexDeclaration = new VertexDeclaration(
                VertexPositionTexture.VertexDeclaration.GetVertexElements());

            //basicEffect = new BasicEffect(graphics.GraphicsDevice, null);
            effect = new BasicEffect(GraphicsDevice);
            effect.Alpha = 1.0f;
            effect.VertexColorEnabled = true;
        }
    }
}
