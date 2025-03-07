using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
 /*
    This is a class for drawing ground in the game.
    The core idea is using two texture of the ground and the second one is placed right after the first one.
    When the first one running out of the window, it is repositioned to the second one and become the second one, creating a seamless scrolling effect.
 */
namespace ground
{
    public class Ground
    {
        private Texture2D ground1;      // instance of ground
        private Texture2D ground2;      // another instance of ground
        private float ground1PosWidth;  // The real X position of the first ground, updating during game.
        private float ground2PosWidth;  // The real X position of the second ground, also updating during game.
        private int groundPosHeight;    // The real Y position of ground.
        private float groundSpeed;      // speed of ground.
        private float groundWidth;      // static width of ground texture.
        private float groundHeight;     // static height of ground texture.
        private float zoom;             // zoom rate (the original texture is a little bit too thin so using zoom to enhance performance)
 
        public Ground(Texture2D ground1, Texture2D ground2, int height, float speed)
        {
            this.ground1 = ground1; 
            this.ground2 = ground2;  
            zoom = 2.0f;
            groundWidth = ground1.Width * zoom;
            groundHeight = ground1.Height * zoom;
            ground1PosWidth = 0;
            ground2PosWidth = groundWidth;
            groundPosHeight = height;
            groundSpeed = speed;
            
        }
 
        public void Update(GameTime gameTime, float groundSpeed)
        {
            float updatedSpeed = groundSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds; 
            // This adjustment is to eliminate loss of precision effect. If we dont do this, you will find a slight flashing gap when two texture repositioning.
            ground1PosWidth -= (float)System.Math.Floor(updatedSpeed); 
            ground2PosWidth -= (float)System.Math.Floor(updatedSpeed);

            if(ground1PosWidth <= - groundWidth) {
                ground1PosWidth = ground2PosWidth + groundWidth;
            }
            if(ground2PosWidth <= - groundWidth) {
                ground2PosWidth = ground1PosWidth + groundWidth;
            }
        }
 
        public void Draw(SpriteBatch spriteBatch)
        {
            // without zoom
            // spriteBatch.Draw(ground1, new Vector2(ground1PosWidth, groundPosHeight), Color.White); 
            // spriteBatch.Draw(ground2, new Vector2(ground2PosWidth, groundPosHeight), Color.White);
            // zoom
            spriteBatch.Draw(ground1, new Vector2(ground1PosWidth, groundPosHeight), null, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
            spriteBatch.Draw(ground2, new Vector2(ground2PosWidth, groundPosHeight), null, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
        }
    }
}