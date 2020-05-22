# OctGL
CSharp Monogame Octree Graphics Library

This project is for [octree](https://en.wikipedia.org/wiki/Octree) spatial representation.

## Build from triangle mesh model ##
### Demo ###
![Demo](./OctGL/Resources/demo.gif)

### Fill object and optimize octree ##
![Sample wireframe](./OctGL/Resources/samplepiece2wf.jpg)

### Show normals ##
![Sample model normals](./OctGL/Resources/samplespherenormals.jpg)

## Operations ##
### Union ###

<table style="padding:10px">
  <tr>
    <td>
      <img src="./OctGL/Resources/cylinder.png"  alt="1" width = 279px height = 279px >
   </td>
   <td><img src="./OctGL/Resources/pyramid.png" alt="2" width = 279px height = 279pxx></td>
   <td><img src="./OctGL/Resources/cylinder_pyramid_union.png" alt="3" width = 279px height = 279px></td>
  </tr>
</table>

### Intersection ###

<table style="padding:10px">
  <tr>
    <td>
      <img src="./OctGL/Resources/cylinder.png"  alt="1" width = 279px height = 279px >
   </td>
   <td><img src="./OctGL/Resources/pyramid.png" alt="2" width = 279px height = 279pxx></td>
   <td><img src="./OctGL/Resources/cylinder_pyramid_intersection.png" alt="3" width = 279px height = 279px></td>
  </tr>
</table>

### Substract ###

<table style="padding:10px">
  <tr>
    <td>
      <img src="./OctGL/Resources/cylinder.png"  alt="1" width = 279px height = 279px >
   </td>
   <td><img src="./OctGL/Resources/piece2.png" alt="2" width = 279px height = 279pxx></td>
   <td><img src="./OctGL/Resources/cylinder_piece2_substract.png" alt="3" width = 279px height = 279px></td>
  </tr>
</table>

### Reverse ###

<table style="padding:10px">
  <tr>
    <td>
      <img src="./OctGL/Resources/sphere.png"  alt="1" width = 279px height = 279px >
   </td>
   <td><img src="./OctGL/Resources/sphere_reverse.png" alt="2" width = 279px height = 279pxx></td>
  </tr>
</table>

Compiled with [MonoGame 3.7.1](https://community.monogame.net/t/monogame-3-7-1-release/11173)  
UI created with [Myra](https://github.com/rds1983/Myra)  
Load 3D file models with [AssimpNET](https://github.com/assimp/assimp-net)  
Sample dragon model from [TurboSquid](https://www.turbosquid.com/es/FullPreview/Index.cfm/ID/1129559)  
AABB triangle intersection based on [StackOverflow](https://stackoverflow.com/questions/17458562/efficient-aabb-triangle-intersection-in-c-sharp)  
