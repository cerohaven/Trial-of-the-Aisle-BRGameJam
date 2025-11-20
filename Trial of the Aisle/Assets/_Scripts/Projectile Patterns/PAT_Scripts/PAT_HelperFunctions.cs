
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectilePatterns
{
    public static class PAT_HelperFunctions
    {
       
        /// <summary>
        /// Get the travel direction for the projectile
        /// </summary>
        /// <param name="angle"> the angle mod value of the projectile </param>
        /// <param name="extraAngle">the extra angle mod value of the projectile</param>
        /// <param name="playerTransform">the player's transform. This is used if the projectile is targetting the player</param>
        /// <returns></returns>
        public static Vector2 GetDirectionFromAngle(float angle, float extraAngle, Transform playerTransform)
        {
            Vector2 dir = new Vector2();

            bool targetPlayer = angle == -99;
            if (targetPlayer)
            {
                //If the player transform is not passed through but we're targetting the player, failsafe to use the angle for what it is.
                if (playerTransform == null) return GetDirectionFromAngleNoPlayer(angle, extraAngle);

                Vector3 mousePosition = Mouse.current.position.ReadValue();
                Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                //Move this gameObject around the player
                dir = (Vector2)playerTransform.position - mouseWorldPosition;

                dir.Normalize();

                float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                dir = new Vector2(Mathf.Cos((a + extraAngle) * Mathf.Deg2Rad),
                                  Mathf.Sin((a + extraAngle) * Mathf.Deg2Rad));

            }
            else
            {
                dir = GetDirectionFromAngleNoPlayer(angle, extraAngle);
            }


            return dir;
        }
        private static Vector2 GetDirectionFromAngleNoPlayer(float angle, float extraAngle)
        {
            return new Vector2(Mathf.Cos((angle + extraAngle) * Mathf.Deg2Rad),
                               Mathf.Sin((angle + extraAngle) * Mathf.Deg2Rad));
        }

        public static PatternTypeMod[] GetProjectilePatternModType(ProjectilePatterns pattern)
        {
            PatternTypeMod[] patTypeMod = null;
            switch (pattern)
            {
                case ProjectilePatterns.Some:
                    patTypeMod = SO_ProjectilePattern.Initialize_SomePAT();
                    break;
                case ProjectilePatterns.Spread:
                    patTypeMod = SO_ProjectilePattern.Initialize_SpreadPAT();
                    break;
                case ProjectilePatterns.Randomize_Angle:
                    patTypeMod = SO_ProjectilePattern.Initialize_RandomizeAnglePAT();
                    break;
                case ProjectilePatterns.Rapid:
                    patTypeMod = SO_ProjectilePattern.Initialize_RapidPAT();
                    break;
                case ProjectilePatterns.Burst:
                    patTypeMod = SO_ProjectilePattern.Initialize_BurstPAT();
                    break;
                case ProjectilePatterns.Randomize_Spawn_Offset:
                    patTypeMod = SO_ProjectilePattern.Initialize_RandomizeSpawnOffsetPAT();
                    break;

                default:
                    Debug.LogWarning("Pattern not Implemented! Please add new Pattern to the switch statement");
                    break;
            }

            //default, if not any one of the patterns, return 
            return patTypeMod;
        }


    }


}
