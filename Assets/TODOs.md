# TODOs
- Figure out rotation of massives - will there be any?
- **Consolidate** patterns and massives
- Shake out obsolete grids - DONE


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
- keeps a list of possible elements for each of the possible elements, as defined by the Elements Library