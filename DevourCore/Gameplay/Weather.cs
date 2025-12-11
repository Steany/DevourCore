using HarmonyLib;
using UnityEngine;

namespace DevourCore
{
    internal static class WeatherAudioPatches
    {

        public static bool Play_Prefix(AudioSource __instance)
        {
            AudioClip clip = null;
            try { clip = __instance.clip; } catch { }

            if (Optimize.ShouldMuteWeatherAudio(__instance, clip))
            {

                return false;
            }

            return true;
        }

        public static bool PlayDelayed_Prefix(AudioSource __instance, float delay)
        {
            AudioClip clip = null;
            try { clip = __instance.clip; } catch { }

            if (Optimize.ShouldMuteWeatherAudio(__instance, clip))
            {
                return false;
            }

            return true;
        }

        public static bool PlayOneShot1_Prefix(AudioSource __instance, AudioClip clip)
        {
            if (Optimize.ShouldMuteWeatherAudio(__instance, clip))
            {
                return false;
            }

            return true;
        }

        public static bool PlayOneShot2_Prefix(AudioSource __instance, AudioClip clip, float volumeScale)
        {
            if (Optimize.ShouldMuteWeatherAudio(__instance, clip))
            {
                return false;
            }

            return true;
        }
    }
}