using System;
using UnityEngine;

public class RandomNumbers {
    public string seed = "";
    public int index = 0;
    
    public RandomNumbers(string seed) {
        this.seed = seed;
        this.index = 0;
    }

    public int GetNumber(int min, int max) {
        double value = this.GetValue();
        int range = max - min;

        int number = (int) Math.Floor(value * range + min);

        return number;
    }

    private int _getSignValue(char sign) {
        if (sign == '0') return 0;
        if (sign == '1') return 1;
        if (sign == '2') return 2;
        if (sign == '3') return 3;
        if (sign == '4') return 4;
        if (sign == '5') return 5;
        if (sign == '6') return 6;
        if (sign == '7') return 7;
        if (sign == '8') return 8;
        if (sign == '9') return 9;
        if (sign == 'a' || sign == 'A') return 10;
        if (sign == 'b' || sign == 'B') return 11;
        if (sign == 'c' || sign == 'C') return 12;
        if (sign == 'd' || sign == 'D') return 13;
        if (sign == 'e' || sign == 'E') return 14;
        if (sign == 'f' || sign == 'F') return 15;

        Debug.Log("error RandomNumbers(), sign = " + sign);
        return -1;
    }

    public double GetValue() {
        string toHash = this.seed + "-" + this.index;
        string hash = GameManager.HashString(toHash);

        double value = 0;

        for (int i = 0; i < 15; i++) {
            char sign = hash[i];

            value += ((double)this._getSignValue(sign) / 16) / 16;
        }

        this.index++;

        return value;
    }
}