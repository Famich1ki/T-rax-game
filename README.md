# T-rax-game
A MonoGame remake of Google's Chrome Dino game! 🦖 

This is my first attempt of programming a 2D game and also the first time using MonoGame with C#. 
Everything feels fresh and interesting. I can not wait anymore to explore this brand-new world! XD

## Update Timeline

### 2025.02.25

#### Updating
Prototype finished. Basic functions, including movements, speed accelaration, score tracking and barrier generation has been fulfilled.

#### NEED TO DO
- Collision detection accuracy improvement
    - The hit boxes of barrier as well as T-rax were too large to provide accurate boundary for collision detection.
- The UI designation of game start and game over.
    - Design a simple UI for game start frame and game over frame.
- score tracking process need to be improved.
    - The score tracking now is somehow unsatisfied.
- Revising variable names.  
    - Some variable names are confused which need to be renamed.
- (If necessary)Reconstruction methods.
    - Some Parameters of methods are unnecessary or unstructured.

### 2025.03.04

#### updating 
Last version the collision detection method is simply AABB detection. Enhancing the accuracy with pixel-level detection after AABB detection.

#### NEED TO DO
- The UI designation of game start and game over.
    - Design a simple UI for game start frame and game over frame.
- score tracking process need to be improved.
    - The score tracking now is somehow unsatisfied.
- Revising variable names.  
    - Some variable names are confused which need to be renamed.
- (If necessary)Reconstruction methods.
    - Some Parameters of methods are unnecessary or unstructured.
- Game over frame.

### 2025.03.06

#### updating
Fixed some collision detection issues on pixel-level detection. Now the pixel-level detection can be correctly performed.  
Adding several debug methods.  
Fixed some score recording issues. Now score can be correctly displayed.  
Adjusting the ground height on the screen.
Adding game over frame.  

#### NEED TO DO
- The UI designation of game start and game over.
    - Design a simple UI for game start frame and game over frame.
- Sometimes birds and cactuses are generated simultaneously， which makes it impossible to dodge.
    - Restruct the obstacle generating process.

### 2025.03.07

#### updating 
Adjusting obstacle generating problems.   
Game Start and Game Over UI design. 
