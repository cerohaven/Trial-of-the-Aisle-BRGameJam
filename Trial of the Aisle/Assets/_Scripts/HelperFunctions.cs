using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
