
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bird;
class Bird {

    public class Entity {
        public Vector2 pos;
        public Color[] color;
        public Texture2D bird;
        public Entity(Vector2 pos, Color[] color, Texture2D bird) {
            this.pos = pos;
            this.color = color;
            this.bird = bird;
        }
    }
    private Texture2D bird1;        // instance of bird
    private Texture2D bird2;        // another instance of bird
    private Color[] color1;
    private Color[] color2;
    private float maxHeight;        // upper limitation of bird
    private float minHeight;        // lower limitation of bird
    private float speed;            // speed of bird. (as same as groundSpeed)
    private Random random;          // instance of Random
    private int currentFrame;       // current frame of bird animation
    private float frameTime;        // duration of each frame
    private int totalFrame;         // number of frame in a loop
    private float elapsedTime;      // timer
    private float zoom;             // zoom rate
    private List<Entity> displayed;// birds currently displayed on the screen   
    private bool isGenerated;       // 
    private float spawnAfter;       // 
    private float duration;         //
    private float totalDuration;    // 

    public Bird(Texture2D bird1, Texture2D bird2, float maxHeight, float minHeight, float speed) 
    {
        this.bird1 = bird1;
        this.bird2 = bird2;
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        this.speed = speed;
        random = new Random();
        currentFrame = 0;
        frameTime = 0.2f;
        totalFrame = 2;
        elapsedTime = 0f;
        zoom = 1.5f;
        displayed = new List<Entity>();
        isGenerated = false;
        spawnAfter = GetNextSpawnAfter();
        duration = 0f;
        this.totalDuration = 0f;

        this.color1 = new Color[this.bird1.Width * this.bird1.Height];
        this.bird1.GetData(this.color1);
        this.color2 = new Color[this.bird2.Width * this.bird2.Height];
        this.bird2.GetData(this.color2);
        this.color1 = getZoomedColor(this.color1, bird1.Width, bird1.Height, (int) (bird1.Width * zoom), (int) (bird1.Height * zoom));
        this.color2 = getZoomedColor(this.color2, bird2.Width, bird2.Height, (int) (bird2.Width * zoom), (int) (bird2.Height * zoom));
    }

    public List<Entity> Update(GameTime gameTime, GraphicsDeviceManager _graphics) 
    {
        float timeUnit = (float) gameTime.ElapsedGameTime.TotalSeconds;
        totalDuration += timeUnit;
        if(isGenerated) {
            spawnAfter = GetNextSpawnAfter();
            isGenerated = false;
        }
        elapsedTime += timeUnit;
        duration += timeUnit;
        if(elapsedTime >= frameTime) {
            elapsedTime = 0;
            currentFrame ++;
        }

        if(currentFrame == totalFrame) {
            currentFrame = 0;
        }

        if(duration >= spawnAfter && (totalDuration % 3) < 1) {
            isGenerated = true;
            duration = 0f;
            Color[] color = currentFrame == 0 ? color1 : color2;
            Texture2D bird = currentFrame == 0 ? bird1 : bird2;
            // System.Console.WriteLine($"bird: width - {bird.Width} - height - {bird.Height}");
            // System.Console.WriteLine($"color: {color.Length}");
            displayed.Add(new Entity(new Vector2(_graphics.PreferredBackBufferWidth, GetNextHeight()), color, bird));
        }
        for(int i = 0; i < displayed.Count; i ++) {
            Vector2 cur = displayed[i].pos;
            cur.X -= (float)System.Math.Floor(speed * timeUnit);
            displayed[i].pos = cur;
        }
        if(displayed.Count > 0 && displayed[0].pos.X <= - bird1.Width) {
            displayed.RemoveAt(0);
        }
        return displayed;
    }   

    public void Draw(SpriteBatch spriteBatch) 
    {
        foreach(Entity entity in displayed) {
            Texture2D curBird = currentFrame == 0 ? bird1 : bird2;
            Vector2 destination = entity.pos;
            entity.color = currentFrame == 0 ? color1 : color2;
            entity.bird = curBird;
            spriteBatch.Draw(curBird, destination, null, Color.White, 0f, new Vector2(0, 0), zoom, SpriteEffects.None, 0f);
        }
    }

    private float GetNextSpawnAfter() 
    {
        return (float) random.NextSingle() * 2 + 3; // 3 <= NextSpawn < 5
    }

    private float GetNextHeight() 
    {
        return minHeight - bird1.Height * zoom + (float) random.NextSingle() * (maxHeight - minHeight);
    }

    private Color[] getZoomedColor(Color[] original, int originWidth, int originHeight, int newWidth, int newHeight) {
        Color[] zoomedColor = new Color[newWidth * newHeight];

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int origX = (int)(x / (float)newWidth * originWidth);
                int origY = (int)(y / (float)newHeight * originHeight);

                origX = Math.Clamp(origX, 0, originWidth - 1);
                origY = Math.Clamp(origY, 0, originHeight - 1);

                int origIndex = origY * originWidth + origX;
                int newIndex = y * newWidth + x;
                zoomedColor[newIndex] = original[origIndex];
            }
        }

        return zoomedColor;
    }
}