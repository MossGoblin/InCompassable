# TODOs
- Figure out rotation of massives - will there be any?
- **Consolidate** patterns and massives - DONE ??
- Shake out obsolete grids - DONE
- Split visibility into tiers:
  - (keep in an enum)
  - alvays visible
  - visible from large distance (absolute value)
  - visible from small distance (absolute value)
  - never visible
  - map relative visibility (example: 1/4 width)


# BIOMES

## Biome types
- Bog
- Dark Forest
- Enchanted Forest
- Stone Forest
- Ruins

## Type of objects
- squares 1×1
  - [1 1]
  - [1 1]
- angles
  - [1 1] [1 1] [1 0] [0 1]
  - [0 1] [1 0] [1 1] [1 1]
- arcs
  - [1 0]
  - [0 1]
- exes
  - [1 0 1]
  - [0 1 0]
  - [1 0 1]
- crosses
  - [0 1 0]
  - [1 1 1]
  - [0 1 0]

# Libraries
## Elements Library
- keeps a list (enum) of all possible elements
- keeps a link to the biome library
- request elements from the biome library by biome index and element index

## Biome Library
- keeps a list of biomes
- requests elements from a biome with specific index by element index

## Biome
- keeps a list of possible prefabs for each of the possible elements, as they are defined by the Elements Library