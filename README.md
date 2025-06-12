# Ruinox Escape

A Unity 2D mobile game in which the astronaut (player) finds themself stranded on a deserted, dystopian, sci-fi planet [with a brutalist style] after an emergency crash landing. The player must discover and complete different stages (currently two have been implemented: a combat and endless runner level), collect items such as oxygen/dash orbs and coins, and recover/retrieve the broken spaceship parts in order to rebuild it and escape before the countdown timer and oxygen run out.

The runner involves a dash mechanism (with cooldown) that can be used to break through obstacles, while the combat has a simple hit/attack ability.
## API Reference

Sample:
#### Get all scores

```http
  GET /api/scores
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `api_key` | `string` | **Required**. The API key |

#### Get score

```http
  GET /api/scores/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `string` | **Required**. user ID to fetch relevant scores |

## Installation

1. Download or clone the repository to a local space
2. Check that Unity, npm, Node.js, sqlite, and all other dependencies / libraries are installed
```bash
  npm install RuinoxEscape
  cd RuinoxEscape
```
3. Ensure build settings and scenes are correctly placed in order
4. Run the game

Note: use Unity version 2022.3 for best / most compatibility.

## Third-party Plugins

- REST Client for Unity