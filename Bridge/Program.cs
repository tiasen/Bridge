using System;
using System.Collections;
using System.Collections.Generic;

namespace Bridge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var strings = new string[]
            {
                "Black: 2H 3D 5S 9C KD-White: 2C 3H 4S 8C AH",
                "Black: 2H 4S 4C 2D 4H-White: 2S 8S AS QS 3S",
                "Black: 2H 3D 5S 9C KD-White: 2C 3H 4S 8C KH",
                "Black: 2H 3D 5S 9C KD-White: 2D 3H 5C 9S KH"
            };
            foreach (var s in strings)
            {
                var bridge = new Bridge(s);
                var comparePokers = bridge.ComparePokers();
                Console.WriteLine(comparePokers.Player == "Tie"
                    ? "Tie"
                    : $"{comparePokers.Player} wins - {comparePokers.Rule}{(comparePokers.SourcePokerValue.Equals("") ? null : $": {comparePokers.SourcePokerValue}")}");
            }
        }
    }
}