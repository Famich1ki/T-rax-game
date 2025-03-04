using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/*
    This is dinosaur class including every attrubute and behavior of dinosaur.
    There are three core behaviors which are init (before game start), running (simply running), crouch (when press key 's' or 'down') and jump (when press key space or 'w' or 'up')

*/
namespace dinosaur
{
    public class Dinosaur
    {
        public Texture2D Runnings { get; set; }         // instance of running dinosaur
        public int Rows { get; set; }                   // row of atlases
        public int Columns { get; set; }                // col of atlases
        private int currentFrame;                       // current frame of animation
        private int totalFrames;                        // total number of frames in a loop
        private float elapsedTime;                      // buzzer    
        private float frameTime;                        // frame rate
        private float zoom;                             // zoom rate
        private float originalPos;                      // original position of dinosaur (before game start)
        public bool IsOnTheGround {get; private set;}   // flag to determine whether the dinosaur is on the ground
        private float widthPos;                         // actual X position in game
        private float heightPos;                        // actual Y position in game
        private float acceleration;                     // a
        private float airDuration;                      // t (from ground to top, s)
        private float jumpHeight;                       // h
        private float jumpingSpeed;                     // v
        private Texture2D Crouchs;                      // instance of crouching dinosaur
        public bool IsCrouching {get; private set;}     // flag to determine whether dinosaur is crouching

        Color[][] runningColor;
        Color[][] crouchingColor;
        Color[] jumpingColor;

        public Dinosaur(Texture2D runnings, Texture2D crouchs,  int rows, int columns, float widthPos, int heightPos, GraphicsDeviceManager _graphics)
        {
            Runnings = runnings;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
            elapsedTime = 0f;
            frameTime = 0.08f;
            zoom = 2.0f;

            originalPos = (int)heightPos - Runnings.Height * zoom + 40;
            IsOnTheGround = true;
            this.widthPos = widthPos;
            this.heightPos = originalPos;
            airDuration = 0.3f;
            jumpHeight = _graphics.PreferredBackBufferHeight * 3 / 10;
            acceleration = 2 * jumpHeight / (airDuration * airDuration);
            jumpingSpeed = acceleration * airDuration;

            Crouchs = crouchs;
            IsCrouching = false;

            getColors();
        }
 
        public float Update(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();
            if(IsOnTheGround) {
                // running on the ground
                elapsedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
                if(elapsedTime >= frameTime) {
                    elapsedTime = 0;
                    currentFrame++;
                }
                
                if (currentFrame == totalFrames) {
                    currentFrame = 0;
                }

                if(kstate.IsKeyDown(Keys.S) || kstate.IsKeyDown(Keys.Down)) {
                    IsCrouching = true;
                } else {
                    IsCrouching = false;
                }
            } else {
                heightPos -= jumpingSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                jumpingSpeed -= acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
            // jumping in the air
            if (!IsOnTheGround && heightPos >= originalPos) {
                heightPos = originalPos;
                IsOnTheGround = true;
            }

            if((kstate.IsKeyDown(Keys.Space) || kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.Up)) && IsOnTheGround) {
                IsOnTheGround = false;
                jumpingSpeed = acceleration * airDuration;
            }

            return heightPos;
        }
        
        public void Init(SpriteBatch spriteBatch) {
            Rectangle sourceRectangle = new Rectangle(1, 0, Runnings.Width / 5, Runnings.Height);
            Vector2 destination = new Vector2(widthPos, originalPos);
            spriteBatch.Draw(Runnings, destination, sourceRectangle, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
        }
 
        public Color[] Run(SpriteBatch spriteBatch)
        {
            int width = Runnings.Width / 5;
            int height = Runnings.Height / Rows;
            int row = currentFrame / Columns;
            int column = 2 + ((currentFrame % Columns) == 0 ? 0 : 1);
 
            Rectangle sourceRectangle = new Rectangle(width * column + 1, height * row, width, height);
            Vector2 destination = new Vector2(widthPos, originalPos);
 
            spriteBatch.Draw(Runnings, destination, sourceRectangle, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
            if(currentFrame % Columns == 0) {
                return runningColor[0];
            }
            return runningColor[1]; 
        }

        public Color[] Jump(SpriteBatch spriteBatch) 
        {
            Rectangle sourceRectangle = new Rectangle(1, 0, Runnings.Width / 5, Runnings.Height);
            Vector2 destination = new Vector2(widthPos, heightPos);

            spriteBatch.Draw(Runnings, destination, sourceRectangle, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
            return jumpingColor;
        }

        public Color[] Crouch(SpriteBatch spriteBatch) {
            int width = Crouchs.Width / 2;
            int height = Crouchs.Height;
            // int row = 0;
            int column = currentFrame % 2;

            Rectangle sourceRectangle = new Rectangle(width * column, 0, width, height);
            Vector2 destination = new Vector2(widthPos, originalPos);
            spriteBatch.Draw(Crouchs, destination, sourceRectangle, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);

            if(currentFrame % 2 == 0) {
                return crouchingColor[0];
            }
            return crouchingColor[1];
        }

        private void getColors() {
            int runningLen = (this.Runnings.Width / 5) * this.Runnings.Height;
            int crouchingLen = (this.Crouchs.Width / 2) * this.Crouchs.Height;
            runningColor = new Color[2][];
            crouchingColor = new Color[2][];
            jumpingColor = new Color[runningLen];

            runningColor[0] = new Color[runningLen];
            runningColor[1] = new Color[runningLen];
            crouchingColor[0] = new Color[crouchingLen];
            crouchingColor[1] = new Color[crouchingLen];

            Rectangle running1 = new Rectangle((this.Runnings.Width / 5) * 2, 0, this.Runnings.Width / 5, this.Runnings.Height);
            Rectangle running2 = new Rectangle((this.Runnings.Width / 5) * 3, 0, this.Runnings.Width / 5, this.Runnings.Height);
            Rectangle crouching1 = new Rectangle(0, 0, this.Crouchs.Width / 2, this.Crouchs.Height);
            Rectangle crouching2 = new Rectangle(this.Crouchs.Width / 2, 0, this.Crouchs.Width / 2, this.Crouchs.Height);
            Rectangle jumping = new Rectangle(0, 0, this.Runnings.Width / 5, this.Runnings.Height);

            this.Runnings.GetData(0, running1, runningColor[0], 0, runningLen);
            this.Runnings.GetData(0, running2, runningColor[1], 0, runningLen);
            this.Crouchs.GetData(0, crouching1, crouchingColor[0], 0, crouchingLen);
            this.Crouchs.GetData(0, crouching2, crouchingColor[1], 0, crouchingLen);
            this.Runnings.GetData(0, jumping, jumpingColor, 0, runningLen);
        }
    }
}