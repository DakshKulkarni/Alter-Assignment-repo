# Multiplayer Turn-Based Card Game – Assignment README
Unity version used: Unity 6000.0.41
This project is a 2-player online card game built in Unity using Mirror networking and JSON-based card data.  
Tested on:
- Android ↔ Android  
- PC ↔ Android  

Both players draw cards, select actions, reveal simultaneously, and abilities apply based on JSON rules.

---

# Networking Solution (Mirror)

This project uses **Mirror**, a lightweight networking framework for Unity.

Mirror provides:
- Host/Client architecture
- Automatic scene switching (Lobby → GameScene)
- NetworkIdentity + SyncVar support
- RPC messaging for JSON communication

Mirror is chosen because:
- It works on Android and PC
- No cloud server is required
- Works entirely over LAN/WiFi
- Simple Host/Join workflow suitable for assignments

---

# Networking Architecture

### 1. Host
A device taps **Host**.  
It becomes both **server + client**.  
All game logic runs only on the host (turn system, scoring, RNG, deck generation).

### 2. Client
Another device taps **Join**.  
Connects using the host’s **LAN IPv4 address**.

### 3. Communication
Server sends game updates as JSON via:
```
TargetRpc → RpcReceiveJsonMessage(json)
```

### 4. Scene Flow
```
Lobby Scene → Host/Join → GameScene (auto loaded for both)
```

---

# JSON Usage in the Game

The game is fully data-driven using a JSON card data file.

Example:
```json
{
  "cards": [
    {
      "id": 1,
      "name": "Beginner",
      "cost": 1,
      "power": 1,
      "ability": { "type": "None" }
    }
  ]
}
```

## 1. Data Loading
- `CardDatabase` loads the JSON file at game start.
- Data is converted into `CardDefinition` objects.

## 2. Random Deck Generation
- The host generates **two different random decks**.
- Cards are drawn from JSON.
- Decks are guaranteed to:
  - Not be identical
  - Include **at least one cost-1 card** at the beginning

## 3. Game Logic (AbilityResolver)
Abilities defined in JSON directly affect gameplay:

| Ability Type   | Meaning                    |
|----------------|----------------------------|
| GainPoints     | Adds score to player       |
| StealPoints    | Removes score from enemy   |
| DoublePower    | Multiplies power value     |
| None           | No effect                  |

No code change is needed to add new cards; updating the JSON is enough.

## 4. UI Rendering
Card UI automatically displays:
- Name  
- Power  
- Cost  
- Ability description  

UI is fully coming from JSON.

---

# Turn System

### Turn Rules
- Players start with **3 cards**.
- Every turn both players:
  - Draw 1 card from their deck  
  - Have a turn cost equal to turn number  
  - Select cards whose total cost ≤ turn number  
- Press **End Turn** to lock choice.

### Resolution Phase
- Both players' actions reveal simultaneously.
- Abilities apply.
- Scores update.
- Next turn starts automatically.

### Game End
After the final turn:
- Higher score wins
- Game shows "Player X Won"

---

# Scenes

### Lobby Scene
- Buttons: Host, Join, Quit
- Configures IP / NetworkManager
- Automatically moves to GameScene on connect

### GameScene
Contains:
- Player hand area
- Opponent hand hidden
- Board area to show played cards
- Turn number
- Timer countdown
- Score display
- Winner text

---

# Instructions To Run & Test

## IMPORTANT: Set Correct IP Address
There might be a scenario where you will have to set the IP address according to your device manually in the project itself and build again. I realise this is not feasible but I was already late to submit the assigment and had to submit the working game.
In `Lobby → NetworkManager`, set:

```
networkAddress = "YOUR_IPV4_ADDRESS"
```

Examples:
- Windows: 192.168.1.10
- Mac: 192.168.0.12
- Android hotspot: 172.20.10.1
- (For my case it was different)

---

# Testing on Two Android Devices

1. Connect both phones to:
   - Same WiFi  
   OR
   - One phone's hotspot  

2. Install APK on both devices.

3. Phone A:
   - Set its IPv4 in NetworkManager
   - Tap **Host**

4. Phone B:
   - Tap **Join**

5. GameScene loads automatically for both.

---

# Testing on PC + Android

1. PC (Unity Editor or Build) → Press **Host**  
2. Android → Press **Join**  
3. Use same WiFi network  
4. GameScene loads for both players

---

# Project Folder Structure

### Gameplay
```
GameController
GameState
AbilityResolver
CardDatabase
CardDefinition
GameEvents
NetworkPlayer
```

### UI
```
HandUI
BoardUI
CardView
GameHUD
```

### Networking
```
CardGameNetworkManager
LobbyUI
NetMessages (JSON wrappers)
```

### Scenes
```
Lobby
GameScene
```

---

# Additional Notes for Evaluators

1. Builds will **not** work unless you update the IP address.
2. Mirror networking requires both devices on the same LAN.
3. This project contains no cloud or internet dependencies.
4. JSON can be modified to add or rebalance cards.

---
