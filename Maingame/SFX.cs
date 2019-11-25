using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Origin
{
    public class SFX
    {
        public static SoundEffect FireLoop;
        public static SoundEffect Sword;
        public static SoundEffect Sword2;
        public static SoundEffect UnlockDoor;
        public static SoundEffect WaterFlow;
        public static SoundEffect MasterSong;
        
        
        public static SoundEffectInstance WaterFlow2;
        public static SoundEffectInstance FireLoop2;

        public static int WaterSources = 0;
        public static int FireSources = 0;

        public static void Load(ContentManager contentManager)
        {
            FireLoop = contentManager.Load<SoundEffect>("SFX\\FireLoop");
            Sword = contentManager.Load<SoundEffect>("SFX\\SWORD");
            Sword2 = contentManager.Load<SoundEffect>("SFX\\SWORD2");
            UnlockDoor = contentManager.Load<SoundEffect>("SFX\\UnlockDoor");
            WaterFlow = contentManager.Load<SoundEffect>("SFX\\WaterFlow");
            MasterSong = contentManager.Load<SoundEffect>("SFX\\GreatMission");
            WaterFlow2 = WaterFlow.CreateInstance();
            WaterFlow2.IsLooped = false;
            FireLoop2 = FireLoop.CreateInstance();
            FireLoop2.Volume = 0.5f;
            FireLoop2.IsLooped = true;
            var soundEffectInstance = MasterSong.CreateInstance();
            soundEffectInstance.IsLooped = true;
            soundEffectInstance.Volume = 0.2f;
            soundEffectInstance.Play();
        }

        public static void StartFire()
        {
            if (FireSources == 0)
            {
                FireLoop2.Play();
            }
            FireSources++;
        }
        public static void StartWater()
        {
            WaterFlow2.Play();
            WaterSources++;
        }
        public static void StopFire()
        {
            FireSources--;
            if (FireSources == 0)
            {
                FireLoop2.Pause();
            }
        }
        public static void StopWater()
        {
            WaterSources--;
        }
        
        public static void Play(SoundEffect effectName)
        {
            effectName.Play();
        }

        public static void StopAll()
        {
            FireSources = 0;
            WaterSources = 0;
            FireLoop2.Stop();
            WaterFlow2.Stop();
        }
    }

    public enum EffectName
    {
        FireLoop,
        Sword,
        Sword2,
        UnlockDoor,
        WaterFlow
    }
}