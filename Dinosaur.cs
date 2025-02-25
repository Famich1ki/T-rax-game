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
        public Texture2D Runnings { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        private float elapsedTime; // buzzer
        private float frameTime;   // frame rate
        private float zoom; // zoom rate
        private float originalPos; // original position of dinosaur (before game start)
        public bool IsOnTheGround {get; private set;}
        private float widthPos; // actual X position in game
        private float heightPos; // actual Y position in game
        private float acceleration; // a
        private float airDuration; // t (from ground to top, s)
        private float jumpHeight; // h
        private float jumpingSpeed; // v
        private Texture2D Crouchs;
        public bool IsCrouching {get; private set;}

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
 
        public void Run(SpriteBatch spriteBatch)
        {
            int width = Runnings.Width / 5;
            int height = Runnings.Height / Rows;
            int row = currentFrame / Columns;
            int column = 2 + ((currentFrame % Columns) == 0 ? 0 : 1);
 
            Rectangle sourceRectangle = new Rectangle(width * column + 1, height * row, width, height);
            Vector2 destination = new Vector2(widthPos, originalPos);
 
            spriteBatch.Draw(Runnings, destination, sourceRectangle, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
            
        }

        public void Jump(SpriteBatch spriteBatch) 
        {
            Rectangle sourceRectangle = new Rectangle(1, 0, Runnings.Width / 5, Runnings.Height);
            Vector2 destination = new Vector2(widthPos, heightPos);

            spriteBatch.Draw(Runnings, destination, sourceRectangle, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
        }

        public void Crouch(SpriteBatch spriteBatch) {
            int width = Crouchs.Width / 2;
            int height = Crouchs.Height;
            // int row = 0;
            int column = currentFrame % 2;

            Rectangle sourceRectangle = new Rectangle(width * column, 0, width, height);
            Vector2 destination = new Vector2(widthPos, originalPos);
            spriteBatch.Draw(Crouchs, destination, sourceRectangle, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
        }
    }
}