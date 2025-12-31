# Distance Displayer
##### Vintage Story Mod

A small mod to display the Euclidean distance from a set target in the world to your player.

<img width="291" height="408" alt="image" src="https://github.com/user-attachments/assets/731ff02b-53f4-4abd-8743-ce590a268e20" />

## Usage
| Command | Function |
| ------- | -------- |
|`.dd set <x> <y> <z>` | Sets the Distance Displayer target to these (`x`,`y`,`z`) world coordinates |
|`.dd clear` | Clears the last set Distance Displayer target |

The last Distance Displayer value is remembered on the client in a global config as of now.

>[!TIP]
>World positions including target selectors are supported when using `.dd set`! You can set the target to your current location by running `.dd set s`.

The Distance Displayer is tied into the Coordinates and Compass display. If you would like to toggle Distance Display, toggle the "Show/Hide coordinates and compass" bind (default CTRL+V).

>[!IMPORTANT]
>Distance Displayer [uses a workaround](https://github.com/bubner/DistanceDisplayer/blob/09e06389177ec41b4fb1d59e6db70c90b45755c0/DistanceDisplayer/DistanceDisplayerHud.cs#L14-L22) to hoist it to the top-right corner of the screen properly.
>
>However, this also removes the ability for the Coordinates and Compass display to automatically move to the far top-right corner when the minimap is *not* in the corner or disabled.
>
>A fixed offset is used from the top-right corner of the screen, which will leave a gap.
