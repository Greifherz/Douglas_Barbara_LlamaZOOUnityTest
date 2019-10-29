<h1>Douglas Barbará LlamaZooUnityTest</h1>

This is a Map generation tool, created in Unity. 
It allows you to:
  - Generate Completely random maps
  - Smooth them out so they look like a map
  - Identify rooms
  - Ensure connectivity between the rooms
  - Clear rooms that are too small for your liking
  - Create a mesh with the result

In order to generate maps using it no need to hit Playmode, just access the window from Window/MapTool in the Unity Options at the top.

The window itself is very self-explanatory but its functions will be described in detail here.

As soon as you open the window, there will be 2 fields, one for the map width and one for the map height. 
These values must go between 0 and 256, values less than 0 are impossible and values greater than 256 are heavy, however than can be changed if you really need it. My computer fries at the Rio's summer if it get any higher than 150 while clearing islands. (it survives since 2012 though)

Once you've set width and height you may press Generate Map, it will generate internally the map and it will show a little checkbox if you want to preview. 

If you want to preview it allows you to set the colors of what would be walls and what would be empties. It is previewed in Gizmo, so no need to worry about clutter on your scene.

Once you see what a map that has been just generated looks like, press smooth so it actually looks like a map. You can press it as many times as you would want it to. It is a tool after all, many uses.

You might have noticed more buttons appeared other than smooth. Find rooms and Ensure connectivity. Find rooms will find what could be called rooms in the map, areas of the same emptyness surrounded by walls. Once rooms were found in the preview it allows you to look at them and forget about the walls. The gizmo is weird so it might need you to hover your mouse over the scene window for it to update.

Also when you've found rooms the Ensure Connectivity starts to work. It makes passageways between all rooms so logically it becomes just a single room. Since it does from 2 to 2 rooms at a time, it always finds the closest 2 rooms that would be connected, then once having them, looks for where they are closest from one another (although there is a decidability problem there, solved with a random).

Also there is room for you to add a wall prefab there. Once you've placed it, it shows a button to generate mesh. You can't generate meshes without the wall prefab so it won't allow you to.

Once you've hit generate mesh it will generate a combined mesh out of the map, save the mesh as a mesh in the outputs folder along with a prefab for the mesh. Both are located in the outputs folder. If you hit again it will overwrite so rename if you want to save something.

About using the tool thats to it.

<h2>Improvements</h2>
I made some notes about where and how to improve along the code. One I just noticed as I'm writing this was about the Extensions. I use a lot of extension methods, and since a lot of iteration would be made I made the iterations extensions that would recieve actions. The actions are there just to write it as lambda and make it more readable, along with extensions I've got two advantages: readability and code reuse.
Since the code in the extensions are reused, it would be best if instead of nested for loops some parallelism would be inserted there. We're in Unity, Burst would be great. But since Burst is a no-do in this (as it is experimental) it could also be improved with common c# threading. Parralel.For and such. It would be much more performatic if you want to generate maps fast.

Another improvement worth mentioning is how the mesh is created. It would be ideal if I calculated the vertices and triangles of it instead of instantiating a bunch of cubes and combining their mesh. It takes patience and time. Left it there as is but there is a cellular automata tutorial that already did the calculation the same way I would. It is concealed behind an interface, it could be easily changed. The project is more than just that mesh part.


<h2>Considerations</h2>
This tool was made thinking about a tool, not a script that runs on play. Hence why it is an editor window, it has no objects the sample scene. Most of it was made trying to avoid use of Mono classes for unit testing. There is interface use.

Not much room on that scope for many design patterns, it is simple. I do avoid using singletons.

Since I avoided using Mono classes you might notice the camera on the sample scene is called MinimapCamera. It can become one and Unity actually makes it easy. I didn't do it myself because it would take some minutes trying to find where I can instantiate a render texture to have it as output and place it on a quad/sprite in front of the camera. Also I do believe it is optional because you want to know if I know how to, I just explained it. I suggested one in Sword Legacy and game designer hated it.

Hope you like the project. It took a lot of effort making room for free time to make it real. Thanks for the motivation to make it, I enjoyed making it.

Douglas Barbará
