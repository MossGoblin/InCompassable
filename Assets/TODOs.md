# TODOs
- Figure out rotation of massives
- Shake out obsolete grids

# PATH FINDING WAYS

## Global map characteristics

- **Biomes** – even though the obstacles that the map is made out of are only a handful of types (tree, pillar, stone block, etc.), the map itself will be split into 3-4 uneven sections, each of one of perhaps 3 different biomes. It is uncertain if this will have any functional meaning, but the main goal is to provide visual marker to the players – if both of them are in a greenish forest, they know they may be close, as it is more likely than not that there is only one such forest in the area. Additionally – if both players are on the border of such biome, they may easily find each other, following the border of the biome.

- **Gradient** – a simple coloring gradient of the surface features, following a random direction throughout the map. A simple linear gradient from one basic color to another will not work (tested and can be seen in the screenshot). Nevertheless some sort of gradient could and probably will come in handy – for example, regardless of biome, some sort of fireflies can be found around a type of stones that are spread more or less regularly throughout the area. The color of those fireflies is yellow on the far east, red in the center and green on the far right. In practice this is a variation on the biome idea, meant to be an overlapping layer.

- **General features** – one of the first ideas was to generate a river that splits the area in twain and that can be used as an orientation tool once found. For now that idea is mothballed, because it will probably the hardest to implement. I’ll try it out at the end. The problem comes from the fact that the river has to have width, maintained on a grid with very large cells. This means quite the work to make the banks of the river not be utterly pixelated. In addition – it has to be crossable. Shallow parts are easy and will be employed, but the real treasure would be a bridge. But a bridge should be a pain in the ass, both in terms of placement and may be in terms of actual traversement (given the fact that I’m not using rigid body movement and would like to avoid any changes in hight position and so on and so forth).

- **Radial beacon** – a version of the gradient, but with radial symmetry around the center of the map. Something that can suggest either in which direction the center is or how far it is or both.

## Intermittent map features

So far there is only one type of those:
- **Die Schmetterlinge**! On random, but regular basis a swarm of some sort of unknown invisible light emitting creatures flies through the map on a line that passes through both the players. The swarm should be large enough to make sure the player has not moved out of their path before they reach him. When this was tested it looked promising. Knowing that the schmetterling swarm travels in a straight line, seeing it’s path through the maps of both players and seeing roughly how long it takes for the swarm to travel the distance between both players really helps get an idea of their relative orientation. Before the idea of these ghost lights was to implement this by letting a herd stampede through the map – no impact on the players (apart from disrupting their vision and movement perhaps), but again letting them get their relative bearings. I also thought about doing this with a swarm of flying creatures, passing between the player cameras and the map. Same thing, really.
A variation of the swarm is a combination between the migration and the River. May be a migration of passive creatures, moving through the map in a slow single line. This would create a temporary ‘River’ that can be used for navigation.

## Personal tools

There will be a large assortment of personal tools that can help the players navigate. The Flare will be always available – a shared skill, based on cooldown. The rest will be found throughout the area and will have use limited in different ways. The players will have very restricted inventory – probably one item only. Some navigation tools will be single use and will leave the inventory right after. Others, however, will be on a cooldown and keeping them will be a compromise the players have to make.

- **Flare** – A shared skill – any one of the players can activate it. A signal flare is launched from the player who activated it and flies on a ballistic (or similar) trajectory over the map to the other player. The direction of the flare while on screen and the flight time should provide guidance.
This Just In! – idea – if the players have, say, two item slots, one of them starts with the flare in one of the slots (the flare player is either selected in the beginning by the players or at random). The flare is an actual object that travels from inventory to inventory, taking up slot while not used. This means that in order to be used, the receiving player has to have an open slot. Good!

- **Personal totem** – an item that will most likely have some passive effect while in inventory (reduce some cooldown?), but can be used for navigation by being dropped on the map – it will always be visible on the compass (see the compass bellow). Yup, now that I think about it, it can be used as a spawning point if the player dies – he respawns on the place of the totem or in the original spawn point, if the totem was in the inventory.

- **Activated Breadcrumbs** – some sort of object that, when activated, makes the player leave a trail while moving. The trail will fade in time, but slowly enough to be relevant. Depending on the needed difficulty, the trails of the two players may be of different or of the same color.

- **Triggered Breadcrumbs** – some tile on the terrain may also make the player leave a trail. As the position of the tiles is not within the control of the player, those trails can be left visible longer or even permanently. Those can be of the same color or of color, depending on the biome or a color tied to the Radial Beacon.

- **MAP** – a real map! Not sure what the impact of this could be, but so far it sounds good – when this object is found it does not do anything. If used, it displays a  stylized map of the whole area, containing all major features and biomes. It does not show the players or their drops or any collectibles or mobs. However, if the players already have some grasp of the terrain, they may use the map to get a sense of the whole picture. Using the object while viewing the map hides it. To nerf the map three things can be done (only of those, all would be too much) – one option is to make the map work on a cooldown. It can be kept indefinitely, but used only occasionally. The second option is no cooldown, but if it is dropped, it disappears. So the only drawback is that it uses up a slot. The third option is to allow the map to be used for a limited time – it can be brought up and folded at will, but has a lifetime, which is counted from the moment of it’s first use. Afterwards it disappears from the inventory. Aaand… now I think I have an even better fourth option – the map is persistent (dropping it does not destroy it), but viewing it uses it up. The total time it’s brought up is counted down, after which it is destroyed. This sound like the best approach.

- **Laser Pointer** – an item with no cooldown or number of uses. The player fires a visible beam of light in the direction he’s looking and it can be seen in the screen of the other player if there is a line of sight. Given the fact that the map will prevent line of sight in most cases, this item will not be very useful. Therefore it does not need nerfing. The only thing that has to be prevented is keeping it active at all times, which can easily be done by allowing rotation while using it, but no movement.

## Compass

Of the ‘quest compass’ type – icons of points of interest, hovering on the border of each minimap. Two types of points of interest will be tracked on each compass:

- **Items** – starting with the totem. When dropped it will always be tracked by it’s owner. The regular items will probably also be on the compass, but only within line of sight.

- **Area features** – some of the area features will also be tracked, if within line of sight – obstacles that are also distinguishable from the rest when viewed on the camera – special boulders, high trees, glowing… stuff and such. The tracking distance of the objects will be limited – there will be a visible distance. Most of the items and regular features will be tracked only within sight range. Most of the features that can be tracked however will not be regular for the purpose of tracking – most likely they will be ‘high’ and will be tracked from further away. The simplest example would be a spire at the center of the map, that can be seen from a third of the map diameter away (however the diameter is defined). Certainly there will also be similar tall structures throughout the map (a few) so that the player will see at least one most of the time. Those will not be unique and will have the same icons, so the players would not know which if the features they see.
  
# BIOMES

## Biome types
- Dark Forest
- Stone
- Bog
- Enchanted Forest
- Ruins

## Type of objects
- squares 1×1
  - [1 1]
  - [1 1]
- angles
  - [1 1] [1 1] [1 0] [0 1]
  - [0 1] [1 0] [1 1] [1 1]
- small diagonals
  - [1 0]
  - [0 1]
- Exes
  - [1 0 1]
  - [0 1 0]
  - [1 0 1]
- Pluses
  - [0 1 0]
  - [1 1 1]
  - [0 1 0]