using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
   public AudioSource audioSource;
   public GameSoundParameters soundParameters;
   public AudioMixerSnapshot paused;
   public AudioMixerSnapshot unpaused;

   private void OnEnable()
   {
       EventManager.AddListener("PlaySoundByName", _OnPlaySoundByName);
       EventManager.AddListener("PauseGame", _OnPauseGame);
       EventManager.AddListener("ResumeGame", _OnResumeGame);
   }

   private void OnDisable()
   {
         EventManager.RemoveListener("PlaySoundByName", _OnPlaySoundByName);
         EventManager.RemoveListener("PauseGame", _OnPauseGame);
         EventManager.RemoveListener("ResumeGame", _OnResumeGame);
   }

   private void _OnPlaySoundByName(object data)
   {
       string clipName = (string) data;

       // try to find the clip in the parameters
       FieldInfo[] fields = typeof(GameSoundParameters).GetFields();
       AudioClip clip = null;
       foreach (FieldInfo field in fields)
       {
           if (field.Name == clipName)
           {
               clip = (AudioClip) field.GetValue(soundParameters);
               break;
           }
       }
       if (clip == null)
       {
           Debug.LogWarning($"Unknown clip name: '{clipName}'");
           return;
       }

       // play the clip
       audioSource.PlayOneShot(clip);
   }

   private void _OnPauseGame()
   {
        paused.TransitionTo(0.01f);
   }

   private void _OnResumeGame()
   {
    unpaused.TransitionTo(0.01f);
   }
}