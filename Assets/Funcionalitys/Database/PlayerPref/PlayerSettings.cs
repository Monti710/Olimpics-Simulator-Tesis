using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    // === CLAVES DE PLAYERPREFS ===
    private const string VolumeKey = "Volume";
    private const string BrightnessKey = "Brightness";
    private const string SoundKey = "Sound";
    private const string IsLeftHandedKey = "IsLeftHanded";
    private const string BulletMarkKey = "BulletMark";
    private const string AutoPointerKey = "AutoPointer";

    // === VALORES DEFAULTS ===
    private const float DefaultVolume = 0.5f;
    private const float DefaultSound = 0.5f;
    private const float DefaultBrightness = 0.5f;
    private const int DefaultIsLeftHanded = 0;
    private const int DefaultBulletMark = 0;
    private const int DefaultAutoPointer = 0;

    public void SavePlayerSettings(float volume, float sound, float brightness, bool isLeftHanded, bool bulletMark, bool autoPointer)
    {
        // Guardar datos en PlayerPrefs
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.SetFloat(SoundKey, sound);
        PlayerPrefs.SetFloat(BrightnessKey, brightness);
        PlayerPrefs.SetInt(IsLeftHandedKey, isLeftHanded ? 1 : 0);
        PlayerPrefs.SetInt(BulletMarkKey, bulletMark ? 1 : 0);
        PlayerPrefs.SetInt(AutoPointerKey, autoPointer ? 1 : 0);

        // Guardar los cambios permanentemente en el disco
        PlayerPrefs.Save();
        Debug.Log("Player settings saved to disk (PlayerPrefs)!");
    }

    public void LoadPlayerSettings(out float volume, out float sound, out float brightness, out bool isLeftHanded, out bool bulletMark, out bool autoPointer)
    {
        // Recuperar datos desde PlayerPrefs con valores por defecto
        volume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
        sound = PlayerPrefs.GetFloat(SoundKey, DefaultSound);
        brightness = PlayerPrefs.GetFloat(BrightnessKey, DefaultBrightness);
        isLeftHanded = PlayerPrefs.GetInt(IsLeftHandedKey, DefaultIsLeftHanded) == 1;
        bulletMark = PlayerPrefs.GetFloat (BulletMarkKey, DefaultBulletMark) == 1;
        autoPointer = PlayerPrefs.GetInt (AutoPointerKey, DefaultAutoPointer) == 1;

        Debug.Log($"Loaded: Vol:{volume} | Bright:{brightness} | Hand:{isLeftHanded}");
    }
}