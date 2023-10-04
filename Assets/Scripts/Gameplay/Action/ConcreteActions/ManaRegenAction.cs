using System;
using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.Actions {
    [CreateAssetMenu (menuName = "BossRoom/Actions/Mana Regen Action")]
    public partial class ManaRegenAction : Action {
        private float nextEffectTime;

        public override bool OnStart (ServerCharacter serverCharacter) {
            Data.TargetIds = new ulong[0];

            serverCharacter.serverAnimationHandler.NetworkAnimator.SetTrigger (Config.Anim);
            serverCharacter.clientCharacter.RecvDoActionClientRPC (Data);
            return ActionConclusion.Continue;
        }

        public override void Reset () {
            base.Reset ();
            nextEffectTime = 0;
        }

        public override bool OnUpdate (ServerCharacter clientCharacter) {
            //Waiting for the next aura effect
            if (Time.time - TimeStarted >= Config.ExecTimeSeconds + nextEffectTime) {
                nextEffectTime += Config.ExecTimeSeconds;
                PerformAuraEffect (clientCharacter);
            }
            return TimeRunning <= Config.EffectDurationSeconds;
        }

        private void PerformAuraEffect (ServerCharacter parent) {
            var colliders = Physics.OverlapSphere (parent.physicsWrapper.Transform.position, Config.Radius, LayerMask.GetMask ("PCs"));
            for (var i = 0; i < colliders.Length; i++) {
                var unit = colliders[i].GetComponent<IDamageable> ();
                if (unit != null)
                    unit.ReceiveMana (parent, -Config.Amount);
            }
        }
    }
}
