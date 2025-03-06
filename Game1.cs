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
    private List<Bird.Bird.Entity> birds;       // position of birds currently displayed on screen
    private Vector2 crouch;                     // width and height of crouching dinosaur (width, height)
    private Vector2 run;                        // width and height of standing dinosaur (width, height)
    private Vector2 birdSize;                   // width and height of bird (width, height)
    private bool isGameOver;                    // flag to determine whether the game is over
    private SpriteFont font;                    // instance of font
    private int score;                          // score recorder 
    private Color[] dinosaurColor;              // Color matrix of dinosaur
    private float originalSpeed;                // initial game speed
    private float scoreSum;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        groundSpeed = 500f;
        isGameOver = true;
        score = 0;
        originalSpeed = groundSpeed;
        scoreSum = 0f;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        // set resolution（例如 1920x1080）
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        dinosaurInitX = _graphics.PreferredBackBufferWidth * 1 / 20;
        groundHeight = _graphics.PreferredBackBufferHeight * 3 / 4;
        dinosaurPos = new Vector2(dinosaurInitX, groundHeight);
        cactusBaseHeight = groundHeight * 1.05f;
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

            scoreSum += (float) (groundSpeed / 100 * gameTime.ElapsedGameTime.TotalSeconds); 
            if(scoreSum >= 1) {
                score ++;
                scoreSum = 0f;
            }
        }
        if(!isGameOver && isColliding()) {
            System.Console.WriteLine("Colliding!"); 
            isGameOver = true;
        }

        groundSpeed = originalSpeed * (1 + (score / 100) * 0.5f);
        base.Update(gameTime);
        // System.Console.WriteLine(birds == null);
        // System.Console.WriteLine(cactuses == null);
        // if(birds != null) {
        //     System.Console.WriteLine(birds.Count);
        // } else {
        //     System.Console.WriteLine("Null");
        // }
        // if(cactuses != null) {
        //     System.Console.WriteLine(cactuses.Count);
        // } else {
        //     System.Console.WriteLine("Null");
        // }
        
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
        if(!isGameOver) {
            if(dinosaur.IsOnTheGround) {
                if(dinosaur.IsCrouching) {
                    dinosaurColor = dinosaur.Crouch(_spriteBatch);
                } else {
                    dinosaurColor = dinosaur.Run(_spriteBatch);
                }
            } else {
                dinosaurColor = dinosaur.Jump(_spriteBatch);
            }
        } else {
            if(isColliding()) {
                dinosaur.GameOver(_spriteBatch);
            } else {
                dinosaur.Init(_spriteBatch);
            }
        }
        debugDrawing();
        _spriteBatch.DrawString(font, "SCORE :" + score, new Vector2(_graphics.PreferredBackBufferWidth * 0.80f, _graphics.PreferredBackBufferHeight * 0.1f), Color.Blue);
        // drawBirdRectangle(birds, _spriteBatch, Color.Red);
        // drawDinosaurRectangle(_spriteBatch, Color.Blue);
        // drawCactusRectangle(cactuses, _spriteBatch, Color.Yellow);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private bool isColliding() {
        if(birds == null || cactuses == null) {
            return false;
        }
        return isCollidingWithBirds() || isCollidingWithCactuses();
    }

    private bool isCollidingWithCactuses() {
        float[] dinoPos = getDinosaurPos();
        foreach(Cactus.Cactus.Entity entity in cactuses) {
            float[] obstacle = new float[4];
            Texture2D cactus = entity.cactus;
            float width = entity.pos.X;
            float zoom = entity.zoom;
            obstacle[0] = width;                                // left
            obstacle[1] = width + cactus.Width * zoom;          // right
            obstacle[3] = cactusBaseHeight;                     // bottom
            obstacle[2] = obstacle[3] - cactus.Height * zoom;   // top
            if(isOverlappingX(dinoPos[0], dinoPos[1], obstacle[0], obstacle[1]) && isOverlappingY(dinoPos[3], dinoPos[2], obstacle[3], obstacle[2])) {
                System.Console.WriteLine("colliding with cactus");
                return isPixelCollision(dinoPos, obstacle, dinosaurColor, entity.color);
            }
        }
        return false;
    }

    private bool isCollidingWithBirds() {
        float[] dinoPos = getDinosaurPos();
        foreach(Bird.Bird.Entity entity in birds) {
            float[] obstacle = new float[4];
            Vector2 birdPos = entity.pos;
            obstacle[0] = birdPos.X;                        // left
            obstacle[1] = birdPos.X + birdSize.X * 1.5f;    // right
            obstacle[2] = birdPos.Y;                        // top
            obstacle[3] = birdPos.Y + birdSize.Y * 1.5f;    // bottom
            if(isOverlappingX(dinoPos[0], dinoPos[1], obstacle[0], obstacle[1]) && isOverlappingY(dinoPos[3], dinoPos[2], obstacle[3], obstacle[2])) {
                System.Console.WriteLine("colliding with bird");
                return isPixelCollision(dinoPos, obstacle, dinosaurColor, entity.color);
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
        groundSpeed = originalSpeed;

        birds = new List<Bird.Bird.Entity>();
    }

    private void drawBirdRectangle(List<Bird.Bird.Entity> birds, SpriteBatch spriteBatch, Color color) 
    {
        if(birds == null) {
            return;
        }
        Texture2D pixel;
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([color]);
        foreach (Bird.Bird.Entity bird in birds) {
            Vector2 birdPos = bird.pos;
            int X = (int) birdPos.X, Y = (int) birdPos.Y, width = (int) (birdSize.X * 1.5f), height = (int) (birdSize.Y * 1.5f);
            spriteBatch.Draw(pixel, new Rectangle(X, Y, width, 1), color); // top
            spriteBatch.Draw(pixel, new Rectangle(X, Y + height, width, 1), color); // bottom
            spriteBatch.Draw(pixel, new Rectangle(X, Y, 1, height), color); // left
            spriteBatch.Draw(pixel, new Rectangle(X + width, Y, 1, height), color); // right
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
        if(cactuses == null) {
            return;
        }
        Texture2D pixel;
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([color]);
        foreach(Cactus.Cactus.Entity cactus in cactuses) {
            int X = (int) cactus.pos.X, Y = (int) cactusBaseHeight;
            float zoom = cactus.zoom;
            int width = (int) (cactus.cactus.Width * zoom), height = (int) (cactus.cactus.Height * zoom);
            spriteBatch.Draw(pixel, new Rectangle(X, Y - height, width, 1), color); // top
            spriteBatch.Draw(pixel, new Rectangle(X, Y, width, 1), color); // bottom
            spriteBatch.Draw(pixel, new Rectangle(X, Y - height, 1, height), color); // left
            spriteBatch.Draw(pixel, new Rectangle(X + width, Y - height, 1, height), color); // right
        }
    }

    private bool isPixelCollision(float[] dinosaur, float[] obstacle, Color[] colorDinosaur, Color[] colorObstacle) {
        int dWidth = (int) (dinosaur[1] - dinosaur[0]);
        int dHeight = (int) (dinosaur[3] - dinosaur[2]);
        int oWidth = (int) (obstacle[1] - obstacle[0]);
        int oHeight = (int) (obstacle[3] - obstacle[2]);
        Rectangle d = new Rectangle((int) dinosaur[0], (int) dinosaur[2], dWidth, dHeight);
        Rectangle o = new Rectangle((int) obstacle[0], (int) obstacle[2], oWidth, oHeight);
        
        Rectangle intersect = Rectangle.Intersect(d, o);

        for(int y = intersect.Top; y < intersect.Bottom; y ++) {
            for(int x = intersect.Left; x < intersect.Right; x ++) {
                // System.Console.WriteLine(x + " " + y);
                int pixelD = (x - d.X) + (y - d.Y) * dWidth;
                int pixelO = (x - o.X) + (y - o.Y) * oWidth;

                if(pixelD >= 0 && pixelD < colorDinosaur.Length && pixelO >= 0 && pixelO < colorObstacle.Length) {
                    // System.Console.WriteLine("in");
                    Color colorD = colorDinosaur[pixelD];
                    Color colorO = colorObstacle[pixelO];

                    if(colorD.A > 0 && colorO.A > 0) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void debugDrawing() {
        if(cactuses == null || birds == null) {
            return;
        }
        foreach(Cactus.Cactus.Entity entity in cactuses) {
            int width = (int) (entity.cactus.Width * entity.zoom);
            int height = (int) (entity.cactus.Height * entity.zoom);
            Texture2D textureFromColor = new Texture2D(GraphicsDevice, width, height);
            textureFromColor.SetData(entity.color);
            _spriteBatch.Draw(textureFromColor, entity.pos, null, Color.Yellow, 0f, new Vector2(0, height), 1, SpriteEffects.None, 0f);
        }

        foreach(Bird.Bird.Entity entity in birds) {
            int width = (int) (entity.bird.Width * 1.5f);
            int height = (int) (entity.bird.Height * 1.5f);
            Texture2D textureFromColor = new Texture2D(GraphicsDevice, width, height);
            // System.Console.WriteLine($"entity.bird: width - {width} - height - {height}");
            // System.Console.WriteLine($"entity.color: {entity.color.Length}");
            textureFromColor.SetData(entity.color);
            _spriteBatch.Draw(textureFromColor, entity.pos, null, Color.Red, 0f, new Vector2(0, 0), 1, SpriteEffects.None, 0f);
        }
        
    }
}
   