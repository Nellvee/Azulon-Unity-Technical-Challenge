# Azulon-Unity-Technical-Challenge

### Overview:
Create a small Item Collection & Inventory System in Unity.

- Collection: the list of all available items in the game (shop, world, or database).
- Inventory: the items currently owned by the player.

The player must be able to:

- View a list of available items (with name, icon, description, and relevant attributes)
- Acquire items through a simple interaction
- View collected items in a personal inventory
- Experience at least one meaningful gameplay mechanic connected to item collection

This challenge focuses on structure, clarity, extensibility, and design thinking, not on feature quantity.

### Requirements

- Items must be easy to configure and extend.
    - New items or item behaviors must be addable without structural rewrites.
- The UI must be clear and functional.
    - Player understanding is critical: actions, state changes, and progression must be communicated through proper visual feedback.
- The project must have a clear and intentional structure.
- No reliance on external frameworks or third-party architecture libraries.
    - Unity built-in packages are fine; avoid third-party architecture frameworks.
- The project must include at least one meaningful and fun gamification mechanic directly tied to item collection or inventory progression.

### Timing  
  
Time UTC+3  
start ~10:00  
pause 14:40  
resume 15:20  
stop 20:58  
resume 7:52  
stop 12:48  

Total for now: ~16h  
  
### Result  
  
Items are structured with this idea:  
1. ItemData - read-only data (ItemDataSO.cs)  
2. Item - runtime object data (Item.cs)  
3. ItemDatabase - Database of items. Handled fully by Addressables Group.  
4. ItemsEditorWindowCreation - (ItemEditorWindow.cs): Unity -> Tools -> Item Manager:  
Editor Window that creates ItemData Scriptable Object assets in specific folder.  
Setups those assets with Addressables Group/Labels.  
Simple search & edit of created assets.  
5. Inventory - simple inventory system that can add/remove items in it.  
6. ItemFactory - handles how ItemDataSO is loaded from Addressables.  
7. CustomItems that inherit ItemDataSO & Item classes.  

8. ItemBehaviours - ItemData will store list of ItemBehaviours that will do something when: Acquired, Used, Lost.  
9. Collection Ledger - List of Items that will be discovered once collected into inventory. Track and progression.  
  
UI:  
1. Full inventory of a Player  (ItemDatabaseGridUI.cs)  
2. Full database of Items in Project  (PlayerInventoryGridUI.cs)  
3. Collection Ledger, registry name, progress. Popup for each item discovered  
  
Scene:  
1. Default template objects (Camera, Volume...)  
2. Player - simple script with Inventory inside  
3. UIManager - reference to global UITooltipObject  
4. UI -> Canvas 1 - canvas for Player Inventory (right screen)  
5. UI -> Canvas 2 - canvas for Item Database (left screen)  
6. UI -> Canvas 3 - canvas for Collection Ledger that tracks progress of current collection registry and shows popup when you discover a new item.  

  
Interactions:  
1. Click on items inside Database to add them to Player Inventory. (You add 1 count of a clicked item to player)  
2. Click on items inside Player Inventory to use and remove them. (You remove 1 item count when clicked. At zero count, item will be deleted)  
(If item contains any Behaviour, it will call 'Use' of this Behaviour as well)

Editor How To:  
1. To Create a new ItemData, open: Unity -> Tools -> Items -> Item Manager. Write Item Id and click create.  
2. To Create a new ItemBehaviour, open: Unity -> Tools -> Items -> Item Behaviours. Write Id and click create.  
Each ItemData contain list of ItemBehaviours.  
ItemBehaviours are custom scripts that do something, so you'll need to create a class that derives from abstract ItemBehaviourSO.cs, then go to EditorWindow and create this behaviour.  
3. To Create a new Collection registry, open: Unity -> Tools -> Collection Ledger -> Registry  
For now, Collection Ledger only able to track 1 registry.  
Just fill registry with a list of ItemData Scriptable Objects.  
4. To add itemData in database... You don't need anything, when you create ItemData (1.), it automatically adds this itemData into database.  
Database itself is just an Addressables group.  
