## Introduction

ChronoTower is a mixed-reality puzzle game where players help a small robot named Lil Charlie repair a broken clock tower by solving spatial and time-based puzzles.

The experience takes place on a tabletop Mixed reality environment, where players interact with a miniature clock tower using hand gestures such as grabbing, poking, and rotating objects. Combining elements of perspective puzzles, time manipulation, and rotating environments, players guide Lil Charlie through multiple floors of the tower until reaching the top to repair the broken mechanism.

**The problem:** After discussing with the team and talking to some friends, we realised that many people crave intellectual puzzles, but many traditional puzzle games are usually experienced on flat screens and can limit the player's physical interaction with the environment.

**The proposed solution:** ChronoTower is valuable because it not only allows players to enjoy solving puzzles using their intellect, it also incorporates physical interactions by leveraging Mixed Reality technology, as it allows players to manipulate the environment directly with their hands, rotate structures, control time, and interact with objects in a natural and intuitive way. This combination of cognitive problem-solving and embodied interaction creates a more engaging and immersive puzzle experience.

## Design Process

**Brainstorming**
 
<table>

<tr>
<td align="center" width="40%">
<img src="https://github.com/user-attachments/assets/010b7147-761a-4378-a347-1c7fc85b88a2" width="300">
</td>

<td width="60%">

The project began with a group brainstorming session where we explored ideas for a mixed reality experience. Since many of us enjoy puzzle games, we decided to create an MR puzzle game focused on perspective puzzles, time control, and rotating structures. During this stage we also defined the core character, Lil Charlie, a small rusty robot created to repair clock towers.

</td>
</tr>


<tr>
<td align="center">
<img src="https://github.com/user-attachments/assets/34427b18-1c2a-48ec-9c18-4fc7b5e6fce0" width="300">
</td>

<td>

After defining the core concept, we created a moodboard to establish the visual direction of the project. We decided to adopt a <b>steampunk-inspired aesthetic</b>, influenced by mechanical clockwork systems, gears, and old machinery often associated with historical clock towers. This step helped the team align on a consistent visual identity for the environment and characters while ensuring that the overall atmosphere felt cohesive and believable.

</td>
</tr>


<tr>
<td align="center">
<img src="https://github.com/user-attachments/assets/c4004a10-51e2-4e6b-9cd8-034e9acf08c1" width="300">
</td>

<td>

To communicate the concept more clearly within the team, we created an early visual draft of the game idea. This sketch illustrated the basic layout of the tower, the player’s perspective from outside the structure, and how players could interact with different puzzle elements. Creating this visualisation was an important step in aligning our understanding of the project, as some verbal references or inspirations were not familiar to all group members.

</td>
</tr>

</table>


**User Persona**
<img width="1008" height="563" alt="Screenshot 2026-03-09 at 2 21 24 AM" src="https://github.com/user-attachments/assets/aa55c007-dd83-4702-abea-73affcec3d43" />

**User Journey**

1. Discovery
The player approaches the setup and sees a table where a miniature clock tower appears in mixed reality. If presented in a demo setting, a short briefing is given to introduce the experience and explain the goal of helping Lil Charlie repair the tower. Curious about the interactive environment, the player begins exploring the tower from different angles.

2. Introduction
A short introduction explains that the clock tower is broken and that the player must help Lil Charlie repair it. The player learns basic interactions such as grabbing objects and moving Lil Charlie.

3. Exploration
The player starts interacting with the tower by rotating bridges, moving platforms, and experimenting with the environment. Walking around the table allows them to view puzzles from different perspectives.

4. Puzzle Solving
As the player progresses through the tower floors, they encounter more complex puzzles. They must use mechanics such as time manipulation, rotating structures, and environmental clues to guide Lil Charlie forward.

5. Completion
After solving the final puzzle code, Lil Charlie will replace the broken gear and restore the clock tower.
Outcome
The tower begins working again, giving the player a sense of accomplishment for successfully solving the puzzles and repairing the tower.



**Wireframes and Prototypes**

Assets: 
<br>

<table>
<tr>
<td align="center">

<b>V2</b><br>
<img src="https://github.com/user-attachments/assets/aee33c26-b87e-4058-a576-de3dcc0d5661" width="350">

</td>
<td align="center">

<b>V3</b><br>
<img src="https://github.com/user-attachments/assets/062ba130-eb46-4b8f-b93c-cdf5d44caca0" width="350">

</td>
</tr>

<tr>
<td align="center">

<b>V4</b><br>
<img src="https://github.com/user-attachments/assets/a85b879b-55c8-4799-98a4-c9ab9d460ab5" width="350">

</td>
<td align="center">

<b>V5</b><br>
<img src="https://github.com/user-attachments/assets/69a300ae-f573-4986-94a4-ab598cdf876d" width="350">

</td>
</tr>
</table>

<b>Wireframes</b>

<table>
<tr>
<td align="center">
<b>V3</b><br>
<img src="https://github.com/user-attachments/assets/0abd3387-10a5-45e3-8484-297199bdc2fa" width="350">
</td>

<td align="center">
<b>V5</b><br>
<img src="https://github.com/user-attachments/assets/5d267d46-676a-4289-bb15-6b43d5255607" width="350">
</td>
</tr>
</table>

**User Research and Testing**

In the beginning of our project, we talked to many people and found out that many people enjoy puzzle-based games, particularly those that are intellectually stimulating but not overly difficult. Based on this insight, the goal was to design puzzles that challenge players while remaining approachable and intuitive.

After creating our workable prototype, we conducted informal user testing sessions to evaluate the gameplay experience and interaction design of ChronoTower. 

During testing, participants were tasked to play the game, and interacted with the mixed reality tower using hand gestures to move bridges, rotate bridges, and manipulate puzzle elements. After completing the experience, participants were asked to provide feedback on the clarity of the interactions and overall gameplay.

The feedback indicated that players generally enjoyed the puzzle mechanics and found the experience engaging. However, several participants mentioned that more guidance was needed at the beginning of the experience, as some interactions were not immediately obvious. In particular, players sometimes struggled to identify which objects were interactive.
Based on this feedback, we improved the design by introducing clearer visual signifiers such as colored handles. We also recognised the importance of providing clearer instructions or a short briefing at the start of the experience to help players understand the core mechanics before attempting the puzzles.

These testing sessions helped refine the interaction design and ensured that the puzzles remained engaging while still being accessible to new players.


## System description

### Features & Functionalities

ChronoTower includes several interactive features designed to showcase the potential of mixed reality for puzzle gameplay.

**Immersive Tabletop Mixed Reality Environment**
The experience detects a real-world table and generates a virtual clock tower on top of it. Players can walk around the tower and observe it from multiple angles, using 6DoF movement to better understand the spatial layout of the puzzles.

**Spatial Puzzle Mechanics**
The game includes several environment-based puzzle mechanics that require players to manipulate structures within the tower.

**Draggable Bridges, Stairs, and Platforms (Hand-Based Interaction)**
Inspired by architectural puzzle games, bridges, stairs, and platforms can be dragged and repositioned by the player to create connections between different parts of the tower.

**Time Manipulation**
One of the core mechanics of ChronoTower is the ability to control the flow of time. Players can move time forwards or backwards, which affects environmental elements such as vines. Moving time forward may cause vines to grow and create new paths, while reversing time can shrink them and reveal alternative routes.

**Puzzle Locks**
The final door at the top of the tower is protected by a lock mechanism that requires players to solve a symbol-based code. Players must observe clues and determine the correct combination in order to unlock the door and fix the tower.

**Watch the demo video or try the live version.**

Link: 

## Digital Implementation

The project adopts a low-poly paper-toy inspired visual style, with bases on a steampunk aesthetic for the visual story telling. This choice supports both the playful narrative tone and the technical constraints of standalone mixed reality hardware, allowing the tower and its mechanisms to remain readable and performant while maintaining a handcrafted toy-like aesthetic.

The development workflow combines several tools:
Blender 5.0.1: Creation of all 3D models and environment assets
Adobe Illustrator: 2D graphic assets and visual textures
Unity 6: Implementation of gameplay system, mixed reality interaction, and scene assembly
Together these tools support a pipeline where stylized assets are modeled externally and assembled into an interactive MR puzzle environment inside Unity.

The project's implementation relies on the Meta SDK. Specifically, the interaction implementation depends on the Meta XR Interaction SDK. PathAlignTransformer implements an ITransformer interface to control the trajectory of dragged objects and performs transform based on interpolation of the distance between set points. The mixed reality implementation relies on the Meta MR Utility Kit, utilizing its built-in Passthrough and Occlusion functions and calling the Scene API to get the position of real table.

## Installation

Requirements:
- Meta Quest 3, Meta Quest 3s or Meta Quest pro.
- Allow the use of spatial data.
- Allow the use of [experimental features](https://developers.meta.com/horizon/documentation/native/android/mobile-experimental-features).

To get the game:
- Download the released .apk from [github](https://github.com/curefate/ChronoTower).
- Clone source code from repository and build apk youself:
    1. `git clone https://github.com/curefate/ChronoTower`
    2. Open project in Unity 6000.3.0f1+.
    3. Switch platform to Android.
    4. Build.
- > Download from Meta Store TODO

## Usage
To use ChronoTower and interact with its features, follow the guidelines below:

- To move around, use your finger to push Lil Charlie in the desired direction.
- To drag a platform, pinch the handles and pull.
- To drag the bridge, pinch the handle and pull upwards.
- To forward or reverse time, look at the watch on your wrist and use your finger of your opposite hand to rotate the clock clockwise or counterclockwise.
- To enter a password, poke the buttons on the password panel.
- To see inside the tower from a different direction, walk around the tower to change your perspective.
  
  
## References
Music:


## Contributors

Fernando Valcazara: fernandovalcazara@gmail.com
Li Zijie: curefate@outlook.com
Tan Ju Wei Audrie: audiwei123@gmail.com
