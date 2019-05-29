using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge
{
    public enum Rules
    {
        HighCard,
        Pair,
        TwoPairs,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }

    public enum Symbols
    {
        D = 'D',
        S = 'S',
        H = 'H',
        C = 'C'
    }

    public enum PokersCount
    {
        Two = 2,
        Three,
        Four,
        Five
    }

    public enum PokerNumber
    {
        N2 = 2,
        N3,
        N4,
        N5,
        N6,
        N7,
        N8,
        N9,
        NT,
        NJ,
        NQ,
        NK,
        NA
    }

    public class Bridge
    {
        private static string TIE = "Tie";

        public Bridge(string playerAndPokers)
        {
            SetPlayerAndPokers(playerAndPokers);
        }

        public PlayerAndPokers FirstPlayerAndPokers { get; set; }

        public PlayerAndPokers SecondPlayerAndPokers { get; set; }

        private Dictionary<PokerNumber, string> pokerNumberSource = new Dictionary<PokerNumber, string>()
        {
            {PokerNumber.N2, "2"},
            {PokerNumber.N3, "3"},
            {PokerNumber.N4, "4"},
            {PokerNumber.N5, "5"},
            {PokerNumber.N6, "6"},
            {PokerNumber.N7, "7"},
            {PokerNumber.N8, "8"},
            {PokerNumber.N9, "9"},
            {PokerNumber.NT, "Ten"},
            {PokerNumber.NJ, "Jack"},
            {PokerNumber.NQ, "Queen"},
            {PokerNumber.NK, "King"},
            {PokerNumber.NA, "Ace"}
        };


        public List<Poker> SplitPokers(string pokersStr)
        {
            var pokers = pokersStr.Split(' ');
            var result = new List<Poker>();
            foreach (var poker in pokers)
            {
                Poker tempPoker;
                var charArray = poker.ToCharArray();

                tempPoker.Symbol = (Symbols) Enum.Parse(typeof(Symbols), charArray[1].ToString());
                tempPoker.PokerNumber = (PokerNumber) Enum.Parse(typeof(PokerNumber), 'N' + charArray[0].ToString());
                result.Add(tempPoker);
            }

            return result;
        }

        private void SetPlayerAndPokers(string str)
        {
            try
            {
                PlayerAndPokers playerAndPokers1;
                PlayerAndPokers playerAndPokers2;
                var strings = str.Split('-');
                var firstPlayer = strings[0].Split(':');
                var secondPlayer = strings[1].Split(':');
                playerAndPokers1.PokersSource = firstPlayer[1].Trim();
                playerAndPokers1.Player = firstPlayer[0].Trim();
                playerAndPokers1.Pokers = SplitPokers(playerAndPokers1.PokersSource);

                playerAndPokers2.PokersSource = secondPlayer[1].Trim();
                playerAndPokers2.Player = secondPlayer[0].Trim();
                playerAndPokers2.Pokers = SplitPokers(playerAndPokers2.PokersSource);

                FirstPlayerAndPokers = playerAndPokers1;
                SecondPlayerAndPokers = playerAndPokers2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public PokersCount RemoveDuplicatesPokerNumber(List<Poker> pokers)
        {
            var hashSet = new HashSet<PokerNumber>();
            pokers.ForEach(poker => { hashSet.Add(poker.PokerNumber); });
            return (PokersCount) Enum.Parse(typeof(PokersCount), hashSet.Count.ToString());
        }

        public Rules GetRules(List<Poker> pokers)
        {
            var pokersCount = RemoveDuplicatesPokerNumber(pokers);
            switch (pokersCount)
            {
                case PokersCount.Four:
                    return Rules.Pair;
                case PokersCount.Two:
                    return ReturnFullHouseOrFourOfKind(pokers);
                case PokersCount.Three:
                    return ReturnTwoPairsOrThreeOfAkind(pokers);
                default:
                    return ReturnFiveKindsResult(pokers);
            }
        }

        private Rules ReturnFiveKindsResult(List<Poker> pokers)
        {
            if (IsSameSymbol(pokers) && IsStraight(pokers))
            {
                return Rules.StraightFlush;
            }

            if (IsSameSymbol(pokers))
            {
                return Rules.Flush;
            }

            return IsStraight(pokers) ? Rules.Straight : Rules.HighCard;
        }

        private bool IsStraight(List<Poker> pokers)
        {
            var orderPokers = (from Poker poker in pokers select poker.PokerNumber).OrderBy(x => x);
            var number = orderPokers.First();
            return orderPokers.All(pokerNumber =>
            {
                var b = pokerNumber == number;
                number++;
                return b;
            });
        }

        private bool IsSameSymbol(List<Poker> pokers)
        {
            var hashSet = new HashSet<Symbols>();
            pokers.ForEach(poker => { hashSet.Add(poker.Symbol); });
            return hashSet.Count == 1;
        }

        private Rules ReturnTwoPairsOrThreeOfAkind(List<Poker> pokers)
        {
            var pokersKindsOfCount = GetPokersKindsOfCount(pokers);
            return pokersKindsOfCount == 2 ? Rules.TwoPairs : Rules.ThreeOfAKind;
        }

        private int GetPokersKindsOfCount(List<Poker> pokers)
        {
            var everyPokerNumberCount = GetEveryPokerNumberCount(pokers);
            return (from PokerNumber key in everyPokerNumberCount.Keys select everyPokerNumberCount[key]).Max();
        }

        private Rules ReturnFullHouseOrFourOfKind(List<Poker> pokers)
        {
            var pokersKindsOfCount = GetPokersKindsOfCount(pokers);
            return pokersKindsOfCount == 4 ? Rules.FourOfAKind : Rules.FullHouse;
        }

        public PokerResult ComparePokers()
        {
            var firstRules = GetRules(FirstPlayerAndPokers.Pokers);
            var secondRules = GetRules(SecondPlayerAndPokers.Pokers);
            if (firstRules != secondRules)
            {
                PokerResult pokerResult;
                if (firstRules > secondRules)
                {
                    pokerResult.Player = FirstPlayerAndPokers.Player;
                    pokerResult.SourcePokerValue = "";
                    pokerResult.Rule = firstRules;
                    return pokerResult;
                }

                pokerResult.Player = SecondPlayerAndPokers.Player;
                pokerResult.SourcePokerValue = "";
                pokerResult.Rule = secondRules;
                return pokerResult;
            }

            return CompareSameRules(firstRules);
        }

        private PokerResult CompareSameRules(Rules rule)
        {
            PokerResult pokerResult;
            pokerResult.Rule = rule;
            var firstEveryPokerNumberCount = GetEveryPokerNumberCount(FirstPlayerAndPokers.Pokers).ToList();
            var secondEveryPokerNumberCount = GetEveryPokerNumberCount(SecondPlayerAndPokers.Pokers).ToList();
            firstEveryPokerNumberCount.Sort(SortByValueAndKey);
            secondEveryPokerNumberCount.Sort(SortByValueAndKey);
            for (var i = 0; i < firstEveryPokerNumberCount.Count; i++)
            {
                if (firstEveryPokerNumberCount[i].Key == secondEveryPokerNumberCount[i].Key) continue;
                if (firstEveryPokerNumberCount[i].Key < secondEveryPokerNumberCount[i].Key)
                {
                    pokerResult.Player = SecondPlayerAndPokers.Player;
                    pokerResult.SourcePokerValue = pokerNumberSource[secondEveryPokerNumberCount[i].Key];
                    return pokerResult;
                }

                pokerResult.Player = FirstPlayerAndPokers.Player;
                pokerResult.SourcePokerValue = pokerNumberSource[firstEveryPokerNumberCount[i].Key];
                return pokerResult;
            }

            pokerResult.Player = TIE;
            pokerResult.SourcePokerValue = "";
            pokerResult.Rule = null;
            return pokerResult;
        }

        private int SortByValueAndKey(KeyValuePair<PokerNumber, int> kv1, KeyValuePair<PokerNumber, int> kv2)
        {
            if (kv1.Value != kv2.Value)
            {
                return kv2.Value - kv1.Value;
            }

            return kv2.Key - kv1.Key;
        }

        private Dictionary<PokerNumber, int> GetEveryPokerNumberCount(List<Poker> pokers)
        {
            var dictionary = new Dictionary<PokerNumber, int>();
            pokers.ForEach(poker =>
            {
                if (dictionary.ContainsKey(poker.PokerNumber))
                {
                    dictionary[poker.PokerNumber] = dictionary[poker.PokerNumber] + 1;
                }
                else
                {
                    dictionary.Add(poker.PokerNumber, 1);
                }
            });

            return dictionary;
        }
    }

    public struct PlayerAndPokers
    {
        public string Player;
        public string PokersSource;
        public List<Poker> Pokers;
    }

    public struct Poker
    {
        public Symbols Symbol;

        public PokerNumber PokerNumber;
    }

    public struct PokerResult
    {
        public string Player;
        public string SourcePokerValue;
        public Rules? Rule;
    }
}