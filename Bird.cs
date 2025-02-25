
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bird;
class Bird {
    private Texture2D bird1;    
    private Texture2D bird2;
    private float maxHeight;
    private float minHeight;
    private float speed;
    private Random random;
    private int currentFrame;
    private float frameTime;
    private int totalFrame;
    private float elapsedTime;
    private float zoom; 
    private List<Vector2> displayed;
    private bool isGenerated;
    private float spawnAfter;
    private float duration;
    private float totalDuration;

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
        displayed = new List<Vector2>();
        isGenerated = false;
        spawnAfter = GetNextSpawnAfter();
        duration = 0f;
        this.totalDuration = 0f;
    }

    public List<Vector2> Update(GameTime gameTime, GraphicsDeviceManager _graphics) 
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
            displayed.Add(new Vector2(_graphics.PreferredBackBufferWidth, GetNextHeight()));
        }
        for(int i = 0; i < displayed.Count; i ++) {
            Vector2 cur = displayed[i];
            cur.X -= (float)System.Math.Floor(speed * timeUnit);
            displayed[i] = cur;
        }
        if(displayed.Count > 0 && displayed[0].X <= - bird1.Width) {
            displayed.RemoveAt(0);
        }
        return displayed;
    }   

    public void Draw(SpriteBatch spriteBatch) 
    {
        foreach(Vector2 pos in displayed) {
            Texture2D curBird = currentFrame == 0 ? bird1 : bird2;
            Vector2 destination = pos;
            spriteBatch.Draw(curBird, destination, null, Color.White, 0f, new Vector2(curBird.Width, curBird.Height), zoom, SpriteEffects.None, 0f);
        }
    }

    private float GetNextSpawnAfter() 
    {
        return (float) random.NextSingle() * 2 + 3; // 3 <= NextSpawn < 5
    }

    private float GetNextHeight() 
    {
        return minHeight + (float) random.NextSingle() * (maxHeight - minHeight);
    }
}