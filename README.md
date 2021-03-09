# unity-rts

This project represents testing and development for simple RTS game mechanics using Unity Game Engine and C#.

Author: Robert Neff (rneff@alumni.stanford.edu)  

#### RTS concepts to implement:  

- Object selection, box selection  
- Camera movement  
    - Pan, rotate, zoom, drag  
    - [Clamp to level edge](https://answers.unity.com/questions/1223377/how-to-stop-the-camera-when-the-player-has-reached.html)
- Pathing  
    - Movement, object tracking, cancelling tracking, following unit
- Vision, "fog of war"  
    - Hidden, revealed but no vision, see  
    - [Shader  / rendertexture](https://andrewhungblog.wordpress.com/2018/06/23/implementing-fog-of-war-in-unity/)
    - [Grid ref count](https://blog.gemserk.com/2018/08/27/implementing-fog-of-war-for-rts-games-in-unity-1-2/)
    - Disable mesh rendered when out of view (trigger check) -> wait for team semantics and actions to make this work
- Terrain  
    - Level building  
    - Generation (if random try tile approach like AOE2), [texturing](https://tech.innogames.com/terrain-shader-in-unity/)
    - [Clamp pos](https://forum.unity.com/threads/in-game-snap-to-grid.77029/)
    - Try tile map mesh of some kind for levels
- Actions  
    - Right click indicator  
    - Shift queueing?  
    - Attack, gather, build, etc.
- Resources  
    - Gathering, interacting, destruction, using  
- Attacking  
    - Single target, next target  
    - "Stances" (ex. attack enemy if can see)  
    - Ranged -> [projectile arcing](https://en.wikipedia.org/wiki/Projectile_motion), object tracking, wall angle avoidance, [formulas](http://hyperphysics.phy-astr.gsu.edu/hbase/traj.html)
    - [Tutorial?](https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D/)
- AI  
    - Response, build orders, exploring, etc.  
- Building  
- Grouping  
    - Formations, commands, patrolling, spacing, micro  
- Multiplayers  
    - Offline and P2P/server-based  
    - PVE  
    - Network = send enemy unit / building locations and status, update locations as needed  
    - [Lockstep updates](https://bitbucket.org/brimock/lockstep-sample/src/master/Assets/Code/), [article](http://clintonbrennan.com/2013/12/lockstep-implementation-in-unity3d/)  
    - [client server lockstep](https://medium.com/@evan_73063/rts-client-server-networking-36e8154ff740)  
    
- Menus  
- UI  
    - Develop as project needs new components, integrate with UI hanlder / Input handler of some kind
    - Minimpap (clicking jumps to location, fog of war active)  
    - [UI cameras](https://answers.unity.com/questions/878667/world-space-canvas-on-top-of-everything.html)  
- Animation  
    - Units, particle effects on abilities  
 
#### Links 

[Flow chart](https://drive.google.com/file/d/1ahTbVrirH2d-aui5a-4yt30Q8P8YACB0/view?usp=sharing)  

[XposeCraft](https://github.com/scscgit/XposeCraft)  

[RTS](https://github.com/DanielKM/unity-RTS)

[Courses?](https://courseupload.com/unity-multiplayer-intermediate-c-coding-and-networking1/)

#### Requirements:

- Assets:

[NavMeshComponents](https://github.com/Unity-Technologies/NavMeshComponents)

[Mirror](https://assetstore.unity.com/packages/tools/network/mirror-129321)

- Packages:

ProGrids (preview package)

ProBuilder

Cinemachine? Input System?

