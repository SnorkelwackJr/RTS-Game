using UnityEngine;

[CreateAssetMenu(fileName = "Sound Parameters", menuName = "Scriptable Objects/Game Sound Parameters", order = 11)]
public class GameSoundParameters : GameParameters
{
   public override string GetParametersName() => "Sound";

   [Header("Ambient sounds")]
   public AudioClip onStationCaptureSound;
   public AudioClip onStationLostSound;
   public AudioClip onBuildingPlacedSound;

   [Range(-80, -12)]
    public int musicVolume;

    [Range(-80, 0)]
    public int sfxVolume;
}