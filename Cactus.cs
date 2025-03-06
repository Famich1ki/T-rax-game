using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Cactus;

class Cactus {

    public class Entity {
        // entity of current displayed cactus
        public Vector2 pos;           // X position
        public Texture2D cactus;    // instance of cactus
        public Color[] color;
        public float zoom;
        public Entity(Vector2 pos, Texture2D cactus, Color[] color, float zoom) {
            this.pos = pos;
            this.cactus = cactus;
            this.color = color;
            this.zoom = zoom;
        }
    }
    private List<Texture2D> cactuses;   // all types of cactus
    private int size;                   // number of type of cactus
    private float originalPosHeihgt;    // initial Y position of cactus
    private float speed;                // speed of cactus (as same as groundSpeed)
    private List<Entity> displayed;     // cactuses currently displayed on the screen
    private Random random;              // instance of Random
    private float spawnAfter;           // 
    private float duration;             // 
    private bool isGenerated;           // 
    private float totalDuration;        // 
    
    public Cactus(List<Texture2D> cactuses, float originalPosHeihgt, float speed) 
    {
        this.cactuses = cactuses;
        size = cactuses.Count;
        this.originalPosHeihgt = originalPosHeihgt;
        this.speed = speed;
        displayed = new List<Entity>();
        random = new Random();
        spawnAfter = GetNextSpawnAfter();
        duration = 0;
        isGenerated = false;
        totalDuration = 0f;
    }

    public List<Entity> Update(GameTime gameTime, GraphicsDeviceManager _graphics) 
    {
        float timeUnit = (float) gameTime.ElapsedGameTime.TotalSeconds;
        totalDuration += timeUnit;
        if(isGenerated) {
            spawnAfter = GetNextSpawnAfter();
            isGenerated = false;
        }
        duration += timeUnit;
        if(duration >= spawnAfter && (totalDuration % 3) >= 1) {
            isGenerated = true;
            duration = 0;
            GetNextCactuses(_graphics);
        }

        foreach(Entity cactus in displayed) {
            cactus.pos.X -= (float)System.Math.Floor(timeUnit * speed);
        }
        if(displayed.Count > 0 && displayed[0].pos.X <= - displayed[0].cactus.Width) {
            displayed.RemoveAt(0);
        }
        return displayed;
    }

    private float GetNextSpawnAfter() 
    {
        return (float) random.NextSingle() * 2 + 1; // 1 <= NextSpawn < 3
    }

    private void GetNextCactuses(GraphicsDeviceManager _graphics) 
    {
        int num = random.Next(1, 4); // {1, 2, 3}
        List<int> selected = new List<int>();

        for(int i = 0; i < num; i ++) {
            int nextCactus = random.Next(0, size);
            selected.Add(nextCactus);
        }
        float prePos = -1;
        float curPos = 0;
        foreach(int nextCactus in selected) {
            if(prePos == -1) {
                curPos = _graphics.PreferredBackBufferWidth;
            } else {
                curPos = prePos;
            }
            float zoom = getZoom();
            Texture2D next = cactuses[nextCactus];
            Color[] origin = new Color[next.Width * next.Height];
            next.GetData(origin);
            int newWidth = (int) (next.Width * zoom);
            int newHeight = (int) (next.Height * zoom);
            Color[] zoomedColor = getZoomedColor(origin, next.Width, next.Height, newWidth, newHeight);
            Entity newCactus = new Entity(new Vector2(curPos, originalPosHeihgt), next, zoomedColor, zoom);
            displayed.Add(newCactus);
            prePos = curPos + next.Width * zoom;
        }
    }

    private float getZoom() 
    {
        return random.NextSingle() + 1;
    }

    public void Draw(SpriteBatch spriteBatch) 
    {
        foreach(Entity entity in displayed) {
            Texture2D cactus = entity.cactus;
            Vector2 destination = entity.pos;
            float zoom = entity.zoom;
            spriteBatch.Draw(cactus, destination, null, Color.White, 0f, new Vector2(0, cactus.Height), zoom, SpriteEffects.None, 0f);
        }
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