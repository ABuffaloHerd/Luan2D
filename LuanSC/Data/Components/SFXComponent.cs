using LuanSC.Objects;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Plays short sound effects for its owner, such as attack hits, footsteps or death sounds.
    /// Sounds are stored by key, so one component can hold more than one clip.
    /// </summary>
    public class SFXComponent : IComponent
    {
        public string Name => "Sound Effect Component";

        public GameObject Owner { get; private set; }

        private Dictionary<string, SoundEffect> sounds = new();

        /// <summary>
        /// Volume for all sounds played by this component, from 0.0 (silent) to 1.0 (full volume).
        /// </summary>
        public float Volume { get; set; } = 1f;

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Adds a sound effect under a key, for example "attack" or "hurt".
        /// If the key already exists, this replaces the sound effect.
        /// </summary>
        /// <param name="key">Name used to play this sound later.</param>
        /// <param name="effect">The loaded sound effect.</param>
        public void Add(string key, SoundEffect effect)
        {
            if (effect is null) return;
            sounds[key] = effect;
        }

        /// <summary>
        /// Removes a sound effect by key.
        /// </summary>
        /// <param name="key">Name of the sound effect to remove.</param>
        public void Remove(string key)
        {
            sounds.Remove(key);
        }

        /// <summary>
        /// Checks if a sound effect has been added under this key.
        /// </summary>
        /// <param name="key">Name to check.</param>
        public bool Has(string key)
        {
            return sounds.ContainsKey(key);
        }

        /// <summary>
        /// Plays a sound effect once, fire-and-forget style.
        /// Does nothing if the key has not been added.
        /// </summary>
        /// <param name="key">Name of the sound effect to play.</param>
        /// <param name="pitch">Pitch adjustment, from -1.0 (down an octave) to 1.0 (up an octave).</param>
        /// <param name="pan">Stereo pan, from -1.0 (full left) to 1.0 (full right).</param>
        public void Play(string key, float pitch = 0f, float pan = 0f)
        {
            if (!sounds.TryGetValue(key, out SoundEffect effect)) return;

            effect.Play(Volume, pitch, pan);
        }

        /// <summary>
        /// Creates a controllable instance of a sound effect, for sounds that need to be paused,
        /// looped, or stopped early, such as a looping footstep or a charge-up sound.
        /// Does nothing and returns null if the key has not been added.
        /// </summary>
        /// <param name="key">Name of the sound effect to create an instance of.</param>
        public SoundEffectInstance CreateInstance(string key)
        {
            if (!sounds.TryGetValue(key, out SoundEffect effect)) return null;

            SoundEffectInstance instance = effect.CreateInstance();
            instance.Volume = Volume;
            return instance;
        }
    }
}