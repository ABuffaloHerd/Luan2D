using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;

namespace LuanSC.Data
{
    /// <summary>
    /// Plays background music for a scene.
    /// One scene should own one SoundManager instance.
    /// </summary>
    public class SoundManager
    {
        private Song currentSong;
        private float volume = 1.0f;

        /// <summary>
        /// True if a song is currently playing.
        /// </summary>
        public bool IsPlaying => MediaPlayer.State == MediaState.Playing;

        /// <summary>
        /// True if a song is currently paused.
        /// </summary>
        public bool IsPaused => MediaPlayer.State == MediaState.Paused;

        /// <summary>
        /// The song that is loaded now. Null if no song has been played yet.
        /// </summary>
        public Song CurrentSong => currentSong;

        /// <summary>
        /// Music volume, from 0.0 (silent) to 1.0 (full volume).
        /// </summary>
        public float Volume
        {
            get => volume;
            set
            {
                volume = MathHelper.Clamp(value, 0f, 1f);
                MediaPlayer.Volume = volume;
            }
        }

        public SoundManager()
        {
            MediaPlayer.Volume = volume;
        }

        /// <summary>
        /// Plays a song. If the same song is already playing, this does nothing.
        /// </summary>
        /// <param name="song">The song to play.</param>
        /// <param name="loop">If true, the song repeats when it ends.</param>
        public void Play(Song song, bool loop = true)
        {
            if (song is null) return;
            if (currentSong == song && IsPlaying) return;

            currentSong = song;
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(song);
        }

        /// <summary>
        /// Stops the current song. The song is cleared, so Resume will not work after this.
        /// </summary>
        public void Stop()
        {
            MediaPlayer.Stop();
            currentSong = null;
        }

        /// <summary>
        /// Pauses the current song. Use Resume to continue it.
        /// </summary>
        public void Pause()
        {
            if (IsPlaying) MediaPlayer.Pause();
        }

        /// <summary>
        /// Resumes a paused song.
        /// </summary>
        public void Resume()
        {
            if (IsPaused) MediaPlayer.Resume();
        }

        /// <summary>
        /// Switches to a new song only if it is not already playing.
        /// Use this in a scene's constructor or transition logic.
        /// </summary>
        /// <param name="song">The song to switch to.</param>
        /// <param name="loop">If true, the song repeats when it ends.</param>
        public void SwitchTo(Song song, bool loop = true)
        {
            if (song is null) return;
            if (currentSong == song) return;

            Play(song, loop);
        }
    }
}