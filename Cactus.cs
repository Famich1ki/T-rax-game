using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Cactus;

class Cactus {

    public class Entity {

        public float pos;
        public Texture2D cactus;
        public float zoom;

        public Entity(float pos, Texture2D cactus, float zoom) {
            this.pos = pos;
            this.cactus = cactus;
            this.zoom = zoom;
        }
    }
    private List<Texture2D> cactuses;
    private int size;
    private float originalPosHeihgt;
    private float speed;
    private List<Entity> displayed;
    private Random random;
    private float spawnAfter;
    private float duration;
    private bool isGenerated;
    private float totalDuration;
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
            cactus.pos -= (float)System.Math.Floor(timeUnit * speed);
        }
        if(displayed.Count > 0 && displayed[0].pos <= - displayed[0].cactus.Width) {
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
            Entity newCactus = new Entity(curPos, next, zoom);
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
            Vector2 destination = new Vector2(entity.pos, originalPosHeihgt);
            float zoom = entity.zoom;
            spriteBatch.Draw(cactus, destination, null, Color.White, 0f, new Vector2(cactus.Width, cactus.Height), zoom, SpriteEffects.None, 0f);
        }
    }
}