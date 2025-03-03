using ground;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using dinosaur;
using Cactus;
using System.Collections.Generic;
using System;

namespace T_rax;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Ground ground;                      // instance of Ground
    private int groundHeight;                   // basic ground height in game
    private Dinosaur dinosaur;                  // instance of Dinosaur
    private float groundSpeed;                  // initial game speed
    private Cactus.Cactus cactus;               // instance of cactus
    private float cactusBaseHeight;             // Y position of all cactus
    private Bird.Bird bird;                     // instance of Bird
    private static float dinosaurInitX;         // initial X position of dinosaur  
    private Vector2 dinosaurPos;                // initial dinosaur position (X, Y)
    private List<Cactus.Cactus.Entity> cactuses;// cactuses currently displayed on screen
    private List<Vector2> birds;                // position of birds currently displayed on screen
    private Vector2 crouch;                     // width and height of crouching dinosaur (width, height)
    private Vector2 run;                        // width and height of standing dinosaur (width, height)
    private Vector2 birdSize;                   // width and height of bird (width, height)
    private bool isGameOver;                    // flag to determine whether the game is over
    private SpriteFont font;                    // instance of font
    private int score;                          // score recorder 
    private float duration;                     // time duration
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        groundSpeed = 500f;
        dinosaurInitX = _graphics.PreferredBackBufferWidth * 1 / 20;
        dinosaurPos = new Vector2(dinosaurInitX, _graphics.PreferredBackBufferHeight * 3 / 4);
        cactusBaseHeight = _graphics.PreferredBackBufferHeight * 3 / 4 + 14;
        groundHeight = _graphics.PreferredBackBufferHeight * 3 / 4;
        isGameOver = true;
        score = 0;
        duration = 0;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        // 设置分辨率（例如 1920x1080）
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges(); // 应用更改

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // TODO: use this.Content to load your game content here
        Texture2D runningAtlases = Content.Load<Texture2D>("RunningAtlases");
        Texture2D crouchAtlasess = Content.Load<Texture2D>("CrouchAtlases");
        crouch = new Vector2(crouchAtlasess.Width / 2, crouchAtlasess.Height);
        run = new Vector2(runningAtlases.Width / 5, runningAtlases.Height);
        dinosaur = new Dinosaur(runningAtlases, crouchAtlasess, 1, 2, dinosaurInitX, groundHeight, _graphics);

        ground = new Ground(Content.Load<Texture2D>("ground"), Content.Load<Texture2D>("ground"), groundHeight, groundSpeed);

        List<Texture2D> cactuses = new List<Texture2D>();
        cactuses.Add(Content.Load<Texture2D>("lc1"));
        cactuses.Add(Content.Load<Texture2D>("lc3"));
        // cactuses.Add(Content.Load<Texture2D>("lc5"));
        cactuses.Add(Content.Load<Texture2D>("xx1"));
        cactuses.Add(Content.Load<Texture2D>("xx3"));
        cactuses.Add(Content.Load<Texture2D>("xx6"));
        cactus = new Cactus.Cactus(cactuses, cactusBaseHeight, groundSpeed);

        Texture2D bird1 = Content.Load<Texture2D>("bird1");
        Texture2D bird2 = Content.Load<Texture2D>("bird2");
        birdSize = new Vector2(bird1.Width, bird1.Height);
        bird = new Bird.Bird(bird1, bird2, groundHeight, groundHeight - runningAtlases.Height * 2, groundSpeed);

        font = Content.Load<SpriteFont>("score");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        if(Keyboard.GetState().IsKeyDown(Keys.R)) {
            isGameOver = false;
            initGame();
        }
        if(!isGameOver) {
            ground.Update(gameTime);
            dinosaurPos.Y = dinosaur.Update(gameTime);
            cactuses = cactus.Update(gameTime, _graphics);
            birds = bird.Update(gameTime, _graphics);
        }
        if(!isGameOver && isColliding()) {
            System.Console.WriteLine("Colliding!"); 
            isGameOver = true;
        }

        score += (int) (groundSpeed * gameTime.ElapsedGameTime.TotalSeconds); 
        duration += (float) gameTime.ElapsedGameTime.TotalSeconds;
        groundSpeed += (duration / 10f);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.Clear(Color.LightGray);
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        ground.Draw(_spriteBatch);
        bird.Draw(_spriteBatch);
        cactus.Draw(_spriteBatch);
        
        if(dinosaur.IsOnTheGround) {
            if(dinosaur.IsCrouching) {
                dinosaur.Crouch(_spriteBatch);
            } else {
                dinosaur.Run(_spriteBatch);
            }
        } else {
            dinosaur.Jump(_spriteBatch);
        }

        _spriteBatch.DrawString(font, "SCORE :" + score, new Vector2(_graphics.PreferredBackBufferWidth * 0.80f, _graphics.PreferredBackBufferHeight * 0.1f), Color.Blue);
        // drawBirdRectangle(birds, _spriteBatch, Color.Red);
        // drawDinosaurRectangle(_spriteBatch, Color.Blue);
        // drawCactusRectangle(cactuses, _spriteBatch, Color.Yellow);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private bool isColliding() {
        return isCollidingWithBirds() || isCollidingWithCactuses();
    }

    private bool isCollidingWithCactuses() {
        float[] dinoPos = getDinosaurPos();
        foreach(Cactus.Cactus.Entity entity in cactuses) {
            Texture2D cactus = entity.cactus;
            float width = entity.pos;
            float zoom = entity.zoom;
            float cactusLeft = width - cactus.Width * zoom;
            float cactusRight = width;
            float cactusBottom = cactusBaseHeight;
            float cactusTop = cactusBottom - cactus.Height * zoom; 
            if(isOverlappingX(dinoPos[0], dinoPos[1], cactusLeft, cactusRight) && isOverlappingY(dinoPos[3], dinoPos[2], cactusBottom, cactusTop)) {
                return true;
            }
        }
        return false;
    }

    private bool isCollidingWithBirds() {
        float[] dinoPos = getDinosaurPos();
        foreach(Vector2 birdPos in birds) {
            float birdLeft = birdPos.X - birdSize.X * 1.5f;
            float birdRight = birdPos.X;
            float birdTop = birdPos.Y - birdSize.Y * 1.5f;
            float birdBottom = birdPos.Y;
            if(isOverlappingX(dinoPos[0], dinoPos[1], birdLeft, birdRight) && isOverlappingY(dinoPos[3], dinoPos[2], birdBottom, birdTop)) {
                return true;
            }
        }
        return false;
    }

    private bool isOverlappingX(float leftA, float rightA, float leftB, float rightB) {
        return rightB > leftA && rightA > leftB;
    }

    private bool isOverlappingY(float bottomA, float topA, float bottomB, float topB) {
        return bottomB > topA && bottomA > topB;
    }

    private float[] getDinosaurPos() {
        // 0 left, 1 right, 2 top, 3 bottom
        float[] pos = new float[4];
        pos[0] = dinosaurPos.X;
        pos[1] = pos[0] + (dinosaur.IsCrouching ? crouch.X : run.X) * 2.0f;
        pos[2] = dinosaurPos.Y;
        pos[3] = pos[2] + (dinosaur.IsCrouching ? crouch.Y : run.Y) * 2.0f;
        return pos;
    }

    private void initGame() 
    {
        Texture2D runningAtlases = Content.Load<Texture2D>("RunningAtlases");
        Texture2D crouchAtlasess = Content.Load<Texture2D>("CrouchAtlases");
        crouch = new Vector2(crouchAtlasess.Width / 2, crouchAtlasess.Height);
        run = new Vector2(runningAtlases.Width / 5, runningAtlases.Height);
        dinosaur = new Dinosaur(runningAtlases, crouchAtlasess, 1, 2, dinosaurInitX, groundHeight, _graphics);

        ground = new Ground(Content.Load<Texture2D>("ground"), Content.Load<Texture2D>("ground"), groundHeight, groundSpeed);

        List<Texture2D> cactuses = new List<Texture2D>();
        cactuses.Add(Content.Load<Texture2D>("lc1"));
        cactuses.Add(Content.Load<Texture2D>("lc3"));
        // cactuses.Add(Content.Load<Texture2D>("lc5"));
        cactuses.Add(Content.Load<Texture2D>("xx1"));
        cactuses.Add(Content.Load<Texture2D>("xx3"));
        cactuses.Add(Content.Load<Texture2D>("xx6"));
        cactus = new Cactus.Cactus(cactuses, cactusBaseHeight, groundSpeed);

        Texture2D bird1 = Content.Load<Texture2D>("bird1");
        Texture2D bird2 = Content.Load<Texture2D>("bird2");
        birdSize = new Vector2(bird1.Width, bird1.Height);
        bird = new Bird.Bird(bird1, bird2, groundHeight, groundHeight - runningAtlases.Height * 2, groundSpeed);

        score = 0;
        
        duration = 0;
    }

    private void drawBirdRectangle(List<Vector2> birds, SpriteBatch spriteBatch, Color color) 
    {
        Texture2D pixel;
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([color]);
        foreach (Vector2 birdPos in birds) {
            int X = (int) birdPos.X, Y = (int) birdPos.Y, width = (int) (birdSize.X * 1.5f), height = (int) (birdSize.Y * 1.5f);
            spriteBatch.Draw(pixel, new Rectangle(X - width, Y - height, width, 1), color); // top
            spriteBatch.Draw(pixel, new Rectangle(X - width, Y, width, 1), color); // bottom
            spriteBatch.Draw(pixel, new Rectangle(X - width, Y - height, 1, height), color); // left
            spriteBatch.Draw(pixel, new Rectangle(X, Y - height, 1, height), color); // right
        }
    }

    private void drawDinosaurRectangle(SpriteBatch spriteBatch, Color color) {
        Texture2D pixel;
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([color]);
        float[] pos = getDinosaurPos();
        int X = (int) pos[0], Y = (int) pos[2];
        int width = (int) (pos[1] - pos[0]), height = (int) (pos[3] - pos[2]);
        spriteBatch.Draw(pixel, new Rectangle(X, Y, width, 1), color); // top
        spriteBatch.Draw(pixel, new Rectangle(X, Y + height, width, 1), color); // bottom
        spriteBatch.Draw(pixel, new Rectangle(X, Y, 1, height), color); // left
        spriteBatch.Draw(pixel, new Rectangle(X + width, Y, 1, height), color); // right
    }

    private void drawCactusRectangle(List<Cactus.Cactus.Entity> cactuses, SpriteBatch spriteBatch, Color color) {
        Texture2D pixel;
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([color]);
        foreach(Cactus.Cactus.Entity cactus in cactuses) {
            int X = (int) cactus.pos, Y = (int) cactusBaseHeight;
            float zoom = cactus.zoom;
            int width = (int) (cactus.cactus.Width * zoom), height = (int) (cactus.cactus.Height * zoom);
            spriteBatch.Draw(pixel, new Rectangle(X - width, Y - height, width, 1), color); // top
            spriteBatch.Draw(pixel, new Rectangle(X - width, Y, width, 1), color); // bottom
            spriteBatch.Draw(pixel, new Rectangle(X - width, Y - height, 1, height), color); // left
            spriteBatch.Draw(pixel, new Rectangle(X, Y - height, 1, height), color); // right
        }
    }
}
