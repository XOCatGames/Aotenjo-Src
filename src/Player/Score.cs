using System;
using UnityEngine;

namespace Aotenjo
{
    /// <summary>
    /// Immubatle Score类
    /// </summary>
    /// 
    [Serializable]
    public class Score : IComparable<Score>, ICloneable
    {
        [SerializeField] private double Fan;

        [SerializeField] private double Fu;

        public Score(double fan, double fu)
        {
            Fan = fan;
            Fu = fu;
        }

        public Score(Score score)
        {
            Fan = score.Fan;
            Fu = score.Fu;
        }

        public Score(Permutation permutation, Player player)
        {
            Fan = permutation.GetFan(player);
            Fu = permutation.GetFu();
        }

        public double GetFan()
        {
            return Fan;
        }

        public double GetFu()
        {
            return Fu;
        }

        public Score AddFan(double add)
        {
            double res = Math.Max(0, Fan + add);
            return new Score(res, Fu);
        }

        public Score AddFu(double add)
        {
            double res = Math.Max(0, Fu + add);
            return new Score(Fan, res);
        }

        public Score MultiplyFan(double mul)
        {
            return new Score(Fan * mul, Fu);
        }

        public double GetScore()
        {
            return Fan * Fu;
        }

        public override string ToString()
        {
            return Fu + " * " + Fan;
        }

        public int CompareTo(Score obj)
        {
            int res = Fan.CompareTo(obj.Fan);
            return res == 0 ? Fu.CompareTo(obj.Fu) : res;
        }

        public static Score Base()
        {
            return new Score(0, 0);
        }

        public object Clone()
        {
            return new Score(this);
        }
    }
}