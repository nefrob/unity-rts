# unity-rts

This project represents testing and development for simple RTS game mechanics using Unity Game Engine and C#.

Author: Robert Neff (rneff@alumni.stanford.edu)  
Property of Curious Cardinals.

#### RTS concepts to implement:  

- Object selection, box selection  
- Camera movement  
    - Pan, rotate, zoom, drag  
- Pathing  
    - Movement, object tracking, cancelling tracking, following unit
- Vision, "fog of war"  
    - Hidden, revealed but no vision, see  
- Terrain  
    - Level building  
    - Generation (if random try tile approach like AOE2)  
- Actions  
    - Right click indicator  
    - Shift queueing?  
    - Attack, gather, build, etc.
- Resources  
    - Gathering, interacting, destruction, using  
- Attacking  
    - Single target, next target  
    - "Stances" (ex. attack enemy if can see)  
    - Ranged?  
- AI  
    - Response, build orders, exploring, etc.  
- Building  
- Grouping  
    - Formations, commands, patrolling, spacing, micro  
- Multiplayers  
    - PVP, PVE, offline and P2P/server-based  
- Menus  
- UI  
    - Develop as project needs new components, integrate with UI hanlder / Input handler of some kind
    - Minimpap (clicking jumps to location, fog of war active)  
    - [UI cameras](https://answers.unity.com/questions/878667/world-space-canvas-on-top-of-everything.html)  
- Animation  
    - Units, particle effects on abilities  
 
#### Links 

[Flow chart](https://drive.google.com/file/d/1ahTbVrirH2d-aui5a-4yt30Q8P8YACB0/view?usp=sharing)  

#### Requirements:

[NavMeshComponents](https://github.com/Unity-Technologies/NavMeshComponents)