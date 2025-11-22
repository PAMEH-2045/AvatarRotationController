# AvatarRotationController
Horizontal avatar rotation using either
ALT and mouse scrollwheel
or ALT, LMB and mouse horizontal movement

## Installation
- Download `.me` and `.dll` files from [releases](https://github.com/PAMEH-2045/AvatarRotationController/releases)
- Install `.me` as always
- Bring `.dll` into `GAME_ROOT_DIRECTORY\MateEngineX_Data\Managed`
- In `GAME_ROOT_DIRECTORY\MateEngineX_Data\ScriptingAssemblies.json` 
  add "AvatarRotationController.dll" to "names" and 16 to "types"
  
  so it would look like this:
  ```
  {
  "names":[ . . . .dll", "AvatarRotationController.dll"],
  "types":[ . . . , 16]
  }
  ```
