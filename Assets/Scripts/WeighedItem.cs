using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeighedItem<T> {
    
    private T value;
    private int weight;
    private int cumulativeSum;
    private static System.Random rndInst = new System.Random();

    public WeighedItem(T value, int weight) {
        this.value = value;
        this.weight = weight;
    }

    public static T Choose(List<WeighedItem<T>> items) {
        int cumulSum = 0;
        int count = items.Count();

        for (int slot = 0; slot < count; slot++) {
            cumulSum += items[slot].weight;
            items[slot].cumulativeSum = cumulSum;
        }

        double divSpot = rndInst.NextDouble() * cumulSum;
        WeighedItem<T> chosen =  items.FirstOrDefault(i => i.cumulativeSum >= divSpot);
        if (chosen == null)
            throw new Exception("No item chosen - there seems to be a problem with the probability distribution.");
        return chosen.value;
    }
}
