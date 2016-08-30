# Procedural landscape generator
Generate your own low poly landscape using Unity.
![Captura](http://i.imgur.com/igNqc9t.png?1)

## How it works
A cellular automata algorithm is used to define the coastline. The heightmap is just the result from processing a noise map through a normalized kernel convolution. The colours are added to the mesh vertices (don't forget to use an appropriate shader to show the colours correctly!)

## Setting things up
1. You will need Unity installed to generate the maps
2. Create an empty GameObject
3. A Mesh Renderer is needed to show the landscape. Add it to the GameObject
5. Add CreateLandscapeMesh.cs to the GameObject
6. Add a Shader to the GameObject. I'm using a Standar(VertexColor) due to how colours were added
7. A plane mesh (with height 1) with a FX/Water shader was added to make the water effect

