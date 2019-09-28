# Procuderally generated 2d world in Unity
 
The application attempts to simulate a forest environment. It generates a 400x400 (by default - easily modified) tilemap and semi-randomly places trees, lakes and lake objects in it. 

Contains simple sprites (my horrendous pixel art experiments) and some other basic functionality such as;
- Player movement & rotation
- Animal spawning
  - Spawn at certain distances from the player to create realistic environment where you'll occasionally run into wild animals. Numbers need tweaking, but the logic is there.
  - Weighed spawning - some animals have a higher chance of spawning than others. Easily modified.
- Animal logic
  - Wander (randomly walks around the map, a few tiles at a time)
  - Aggressive (chase player)
  - Escape (Hastily runs away from the player)
- Chopping trees
  - Colliding with trees will chop them down (replace the tree sprite with a trunk sprite) and respawn the tree (undo sprite change) after X seconds


![procedurally_generated_map](https://github.com/teSill/Procedurally-generated-2d-world/blob/master/Pics/map.png?raw=true)
![random_view](https://github.com/teSill/Procedurally-generated-2d-world/blob/master/Pics/random_view.png?raw=true)
