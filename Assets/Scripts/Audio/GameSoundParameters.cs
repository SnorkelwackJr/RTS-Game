using UnityEngine;

[CreateAssetMenu(fileName = "Sound Parameters", menuName = "Scriptable Objects/Game Sound Parameters", order = 11)]
public class GameSoundParameters : ScriptableObject
{
   [Header("Ambient sounds")]
   public AudioClip onStationCaptureSound;
   public AudioClip onStationLostSound;
   public AudioClip onBuildingPlacedSound;
}