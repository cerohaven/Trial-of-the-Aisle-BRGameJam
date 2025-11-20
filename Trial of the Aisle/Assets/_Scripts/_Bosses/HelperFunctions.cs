using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectilePatterns;
public class HelperFunctions : MonoBehaviour
{
    //Returns the attack to be used based on the percentage chance of getting it
    //Now with an attack, we can set the random attack to this one
    public static int RNGAttack(SO_BossProfile bossProfile)
    {
        float sumOfProbabilities = 0;
        float prevProbs = 0;

        int numOfAttacks = bossProfile.B_BossAttacks.Length;
        float[] probs = new float[numOfAttacks];
        float[] newChance = new float[numOfAttacks];
        float[] cumulativeProb = new float[numOfAttacks + 1];

        //Get the sum of all the probabilities
        for (int i = 0; i < probs.Length; i++)
        {
            sumOfProbabilities += bossProfile.B_BossAttacks[i].percentChanceToUse;
        }

        //now calculate the new chance of getting each snowball based on the sum
        for (int i = 0; i < probs.Length; i++)
        {
            newChance[i] = bossProfile.B_BossAttacks[i].percentChanceToUse / sumOfProbabilities;
        }



        //now for the cumulativeProbability, aligning them from a range of 0-1
        // s1 (0 - 0.2) s2 (0.2 - 0.4) s3 (0.4 - 0.8)...
        for (int i = 0; i < probs.Length; i++)
        {
            cumulativeProb[0] = 0.00f;

            prevProbs += newChance[i];
            cumulativeProb[i + 1] = prevProbs;
        }

        //now generate a random number between 0-1 and see which cumulativeProb falls into the range. Return the attack to be used
        float random = Random.Range(0.00f, 1.00f);

        for (int i = 0; i < cumulativeProb.Length; i++)
        {
            if (random > cumulativeProb[i] && random < cumulativeProb[i + 1])
                return i;
        }

        return 0;
    }



    public static GameObject RNGProjectile(SO_BossProfile bossProfile)
    {
        //If there's only 1 projectile in the list we don't need to do all this extra math, just return the 1 projectile
        if(bossProfile.B_BossThrowProjectiles.Length == 1)
        {
            return bossProfile.B_BossThrowProjectiles[0].projectilePrefab;
        }

        float sumOfProbabilities = 0;
        float prevProbs = 0;

        int numOfAttacks = bossProfile.B_BossThrowProjectiles.Length;
        float[] probs = new float[numOfAttacks];
        float[] newChance = new float[numOfAttacks];
        float[] cumulativeProb = new float[numOfAttacks + 1];

        //Get the sum of all the probabilities
        for (int i = 0; i < probs.Length; i++)
        {
            sumOfProbabilities += bossProfile.B_BossThrowProjectiles[i].percentChanceToUse;
        }

        //now calculate the new chance of getting each snowball based on the sum
        for (int i = 0; i < probs.Length; i++)
        {
            newChance[i] = bossProfile.B_BossThrowProjectiles[i].percentChanceToUse / sumOfProbabilities;
        }



        //now for the cumulativeProbability, aligning them from a range of 0-1
        // s1 (0 - 0.2) s2 (0.2 - 0.4) s3 (0.4 - 0.8)...
        for (int i = 0; i < probs.Length; i++)
        {
            cumulativeProb[0] = 0.00f;

            prevProbs += newChance[i];
            cumulativeProb[i + 1] = prevProbs;
        }

        //now generate a random number between 0-1 and see which cumulativeProb falls into the range. Return the attack to be used
        float random = Random.Range(0.00f, 1.00f);

        for (int i = 0; i < cumulativeProb.Length; i++)
        {
            if (random > cumulativeProb[i] && random < cumulativeProb[i + 1])
                return bossProfile.B_BossThrowProjectiles[i].projectilePrefab;
        }

        return bossProfile.B_BossThrowProjectiles[0].projectilePrefab;
    }


    public static float ProjectileSpeedAtPhase(SO_BossProfile bossProfile, int bossPhase)
    {
        if (bossPhase == 0)
        {
            //if it's the starting phase, use the base throw speed and attack delay
            return bossProfile.B_BaseProjectileThrowSpeed;
        }
        else
        {
            //if it's the starting phase, use the base throw speed and attack delay
            return bossProfile.B_BossPhases[bossPhase - 1].throwSpeed;
        }

    }

    public static SO_ProjectilePattern GetProjectilePatternAtPhase(SO_BossProfile bossProfile, int bossPhase)
    {
        if (bossPhase == 0)
        {
            return bossProfile.B_BaseProjectileThrowPattern;
        }

       
         return bossProfile.B_BossPhases[bossPhase - 1].projectilePattern;

    }

    public static float TimeBetweenAttacksAtPhase(SO_BossProfile bossProfile, int bossPhase)
    {
        if (bossPhase == 0)
        {
            //if it's the starting phase, use the base throw speed and attack delay
            return bossProfile.B_BaseTimeBetweenProjectileAttacks;
            
        }
        else
        {
            //if it's the starting phase, use the base throw speed and attack delay
            return bossProfile.B_BossPhases[bossPhase - 1].attackDelay;
           
        }
    }


}
