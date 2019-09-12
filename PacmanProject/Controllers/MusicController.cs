using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Pacman.Controllers
{
    public class MusicController : GameComponent
    {
        private List<SoundEffect> soundEffects;
        private SoundEffectInstance ballInstance;

        public MusicController(Game game) : base(game)
        {
            Game.Components.Add(this);

            soundEffects = new List<SoundEffect>();

            soundEffects.Add(Game.Content.Load<SoundEffect>("soundEffects/base"));
            soundEffects.Add(Game.Content.Load<SoundEffect>("soundEffects/siren"));
            soundEffects.Add(Game.Content.Load<SoundEffect>("soundEffects/powerup"));
            soundEffects.Add(Game.Content.Load<SoundEffect>("soundEffects/eat_ghost"));
            soundEffects.Add(Game.Content.Load<SoundEffect>("soundEffects/enemy_appears"));
            soundEffects.Add(Game.Content.Load<SoundEffect>("soundEffects/eat_ball"));
            soundEffects.Add(Game.Content.Load<SoundEffect>("soundEffects/pacman_die"));

            ballInstance = soundEffects[5].CreateInstance();
            ballInstance.Volume = .1f;

            soundEffects[0].Play();
        }

        public void Stop()
        {
            foreach (SoundEffect soundEffect in soundEffects)
                soundEffect.Dispose();
        }

        public void OnPacmanWanted()
        {
            soundEffects[1].Play();
        }

        public void OnPacmanPowerUp()
        {
            soundEffects[2].Play();
        }

        public void OnGhostEat()
        {
            soundEffects[3].Play();
        }

        public void OnEnemyRestored()
        {
            soundEffects[4].Play();
        }

        public void OnBallEated()
        {
            ballInstance.Play();
        }

        public void OnPacmanDie()
        {
            soundEffects[6].Play();
        }
    }
}
