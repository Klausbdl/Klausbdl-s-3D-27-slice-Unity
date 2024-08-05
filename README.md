# Klausbdl-s-3D-27-slice-Unity
A script that can slice 3D models into 27 parts, stretching the middle and keeping the corners.

![showcase gif 1](https://github.com/Klausbdl/Klausbdl-s-3D-27-slice-Unity/blob/main/27%20slice.gif)
![showcase gif 2](https://github.com/Klausbdl/Klausbdl-s-3D-27-slice-Unity/blob/main/27%20slice%202.gif)

# Usage
1. Create an empty object and add the TiledMesh script. It should add all the components needed (a mesh filter, renderer and an optional mesh collider.
2. Add a game object to the Original Model variable for the script to load its mesh. It will probably give you an error about tell you to use sharedMesh, but you can ignore it.
3. Use the "scale" variable to edit the mesh. The Transform's transformations will work along it with no issue.

Please fork this and make something better.
