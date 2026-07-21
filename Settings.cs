using BlackStartX.GestureManager;
using System.Reflection;
using UnityEngine;
internal class Settings
{
    internal static void AddSettings(AvatarRotationController rotationController)
    {
        GestureManagerManager.RegisterSettingsMenu(
            "AvatarRotationController",
            [
                ModSettings.Radial(
                    name: "Scroll speed",
                    radialField: new ModSettings.FieldRef(
                        rotationController,
                        typeof(AvatarRotationController).GetField("scrollRotationSpeed", BindingFlags.NonPublic | BindingFlags.Instance)
                    ),
                    min: 0f,
                    max: 50f,
                    checkpoint: 10f,
                    displayType: ModSettings.DisplayType.Percentage
                ),
                ModSettings.Radial(
                    name: "Mouse speed",
                    radialField: new ModSettings.FieldRef(
                        rotationController,
                        typeof(AvatarRotationController).GetField("mouseRotationSpeed", BindingFlags.NonPublic | BindingFlags.Instance)
                    ),
                    min: 0f,
                    max: 2.5f,
                    checkpoint: 0.5f,
                    displayType: ModSettings.DisplayType.Percentage
                ),
                ModSettings.Radial(
                    name: "Turn threshold",
                    radialField: new ModSettings.FieldRef(
                        rotationController,
                        typeof(AvatarRotationController).GetField("turnАroundThreshold", BindingFlags.NonPublic | BindingFlags.Instance)
                    ),
                    min: 20,
                    max: 120,
                    checkpoint: 65,
                    displayType: ModSettings.DisplayType.Degree,
                    icon: EResources.Load<Texture2D>("TurnAroundThreshold")
                ),
                ModSettings.Toggle(
                    name: "Turn around",
                    toggleField: new ModSettings.FieldRef(
                        rotationController,
                        typeof(AvatarRotationController).GetField("turnАround", BindingFlags.NonPublic | BindingFlags.Instance)
                    )
                )
            ]
        );
    }
}

