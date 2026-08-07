using MEGME;
using MEGME.Settings;
using System.Reflection;
using UnityEngine;

namespace AvatarRotationController
{
    internal class Settings
    {
        internal static void AddSettings(AvatarRotationController rotationController)
        {
            RadialMenuController.RegisterSettingsMenu(
                ModSettings.SubMenu(
                    name: "AvatarRotationController",
                    icon: EResources.Load<Texture2D>("Icon"),
                    ModSettings.Radial(
                        name: "Scroll speed",
                        setting: Setting<float>.From(
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
                        setting: Setting<float>.From(
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
                        setting: Setting<float>.From(
                            rotationController,
                            typeof(AvatarRotationController).GetField("turnАroundThreshold", BindingFlags.NonPublic | BindingFlags.Instance)
                        ),
                        min: 20,
                        max: 120,
                        checkpoint: 65,
                        displayType: ModSettings.DisplayType.Degree
                    ),
                    ModSettings.Toggle(
                        name: "Turn around",
                        Setting<bool>.From(
                            rotationController,
                            typeof(AvatarRotationController).GetField("turnАround", BindingFlags.NonPublic | BindingFlags.Instance)
                        )
                    ),
                    ModSettings.Toggle(
                        name: "Block dragging",
                        Setting<bool>.From(
                            rotationController,
                            typeof(AvatarRotationController).GetField("blockDragging", BindingFlags.NonPublic | BindingFlags.Instance)
                        )
                    )
                )
            );
        }
    }
}