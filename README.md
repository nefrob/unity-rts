# unity-rts

This project implements a basic RTS game using Unity game engine (2020.1.3f1) and C#. It is based on [this](https://www.udemy.com/course/unity-multiplayer/) Udemy course, with expanded mechanics.

The following gameplay features are present:  
- Camera movement (pan, rotate and zoom) with clamping to the edge of the map.  
- Object click and drag box selection.  
- `NavMesh` pathing for units with dynamically carved buildings. Units will follow targets within a certain range when commanded to attack.  
- Basic "fog-of-war" and player vision projected on the map and minimap. Vision meshes (on a specific layer) are rendered to `RenderTexture`s, combined with shaders to allow the map to be explored. Three states exist: unexplored, explored but out of player vision (building/unit sight lines), explored and in player vision (see [here](https://andrewhungblog.wordpress.com/2018/06/23/implementing-fog-of-war-in-unity/) for example). In classic games like *Age of Empires* a 2D vision count grid is used to map the visible and hidden world elements to the player. The above approach was easier to generate a similar result, and a render texture scaled to the map grid size would achieve a similar result. To expand on this feature enemy objects could have their `MeshRenderer`s toggled (ex. trigger with vision counter) when entering/exiting player vision range.  
- Actions (right click) for selected objects are supported. Namely, attack for military units and gather point setting for buildings.  
- Ranged and melee combat units. Note: the melee units essentially just fire a non-moving projectile. Projectiles compute the velocity needed to reach their target given an initial launch angle.  
- Basic buildings: base, resource generator and spawner buildings for ranged/melee units. Destroying a player's base ends the game.  
- Simple main menu to join/host a game.  
- Multiplayer via [Mirror](https://assetstore.unity.com/packages/tools/network/mirror-129321) allows for a simple multiplayer client-server model (not lockstep like old RTS game). Players can play together via *Steam* via [FizzySteamworks](https://github.com/Chykary/FizzySteamworks), i.e. peer-to-peer with one player acting as the host. To play, build the project and add the generate executable to steam. Have both player load to the main menu, with one selecting to host a game. The second player can then join their friends game session from the steam friends list page.  

#### Controls

- Camera move: `WASD` to pan (this can be toggled to mouse at edges of the screen if preferred), `QE` to rotate, mouse `scroll` to zoom.  
- Mouse `left` to select, hold to drag select. Mouse `right` to command (ex. move). Clicking on a selected building creates a new unit if you have enough resources.

#### Images

TODO:

#### Areas to build on this project:

- Level building or generation (perhaps using grid world like *Age of Empires*).
- Collectible resources on the map. A "Villager" type unit to gather resources would then also be necessary.  
- Proper melee units.  
- Attack stances (chase, stand ground, etc.) and formations.  
- Ranged unit projectile tracking for 100 percent accuracy on shots.   
- Enemy offline AI.  
- more units and building types.  
- Audio, art, particle effects, animation, etc.  

#### Issues:

- Currently hosting a game twice causes the end of the second game to crash. This is a legacy bug from the Udemy tutorial and doesn't have a readily obvious fix (although it likely stems from events not being properly deregistered). IIt wasn't my focus on this project so it's still there :/.

#### Requirements:

[NavMeshComponents](https://github.com/Unity-Technologies/NavMeshComponents)

[Mirror](https://assetstore.unity.com/packages/tools/network/mirror-129321)

[FizzySteamworks](https://github.com/Chykary/FizzySteamworks) and [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET).

[Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Installation.html)
