using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;

internal class EResources
{
    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
    private static readonly string[] resNames = assembly.GetManifestResourceNames();
    private static AssetBundle resBundle = AssetBundle.LoadFromMemory(LoadBuffer("AvatarRotationController_Resources.assetbundle"));

    private static byte[] LoadBuffer(string resourceName)
    {
        resourceName = resNames.First(name => name.Contains(resourceName));

        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException(resourceName);

        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        return buffer;
    }

    internal static T Load<T>(string resourceName) where T : Object =>
        resBundle.LoadAsset<T>(resourceName) ?? throw new FileNotFoundException(resourceName);
}
