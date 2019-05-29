using Bridge;
using Xunit;

namespace BridgeTest
{
    public class Tests
    {
        [Fact]
        public void SplitPokers()
        {
            var bridge = new Bridge.Bridge("Black: 2H 3D 5S 9C KD-White: 2C 3H 4S 8C AH");
            var pokers = bridge.SplitPokers("2H 3D 5S 9C KD");
            Assert.Equal(Symbols.H, pokers[0].Symbol);
            Assert.Equal(PokerNumber.N2, pokers[0].PokerNumber);
            Assert.Equal(Symbols.D, pokers[4].Symbol);
            Assert.Equal(PokerNumber.NK, pokers[4].PokerNumber);
        }

        [Fact]
        public void TestConstructor()
        {
            var bridge = new Bridge.Bridge("Black: 2H 3D 5S 9C KD-White: 2C 3H 4S 8C AH");
            Assert.Equal("Black", bridge.FirstPlayerAndPokers.Player);
            Assert.Equal("2H 3D 5S 9C KD", bridge.FirstPlayerAndPokers.PokersSource);
            Assert.Equal("White", bridge.SecondPlayerAndPokers.Player);
            Assert.Equal("2C 3H 4S 8C AH", bridge.SecondPlayerAndPokers.PokersSource);
        }

        [Fact]
        public void RemoveDuplicatesPokerNumber()
        {
            var bridge1 = new Bridge.Bridge("Black: 2H 3D 4S 5C 6D-White: 2C 2H 2S 2C AH");
            var bridge2 = new Bridge.Bridge("Black: 2H 3D 5S 2C 2D-White: AC 9H 2S 2C 5H");
            var bridge1FirstPokersNumber = bridge1.RemoveDuplicatesPokerNumber(bridge1.FirstPlayerAndPokers.Pokers);
            var bridge1SecondPokersNumber = bridge1.RemoveDuplicatesPokerNumber(bridge1.SecondPlayerAndPokers.Pokers);
            var bridge2FirstPokersNumber = bridge2.RemoveDuplicatesPokerNumber(bridge2.FirstPlayerAndPokers.Pokers);
            var bridge2SecondPokersNumber = bridge2.RemoveDuplicatesPokerNumber(bridge2.SecondPlayerAndPokers.Pokers);
            Assert.Equal(PokersCount.Five, bridge1FirstPokersNumber);
            Assert.Equal(PokersCount.Two, bridge1SecondPokersNumber);
            Assert.Equal(PokersCount.Three, bridge2FirstPokersNumber);
            Assert.Equal(PokersCount.Four, bridge2SecondPokersNumber);
        }


        [Theory]
        [InlineData("Black: 2H 3D 5S 2C 6D-White: 2H 6D AS KC 3D", Rules.Pair, Rules.HighCard)]
        [InlineData("Black: 2H 2D 2S 2C 6D-White: AC AH 2S 2C AH", Rules.FourOfAKind, Rules.FullHouse)]
        [InlineData("Black: 2H 2D 2S 5C 6D-White: AC AH 2S 2C KH", Rules.ThreeOfAKind, Rules.TwoPairs)]
        [InlineData("Black: 7H 3D 4S 5C 6D-White: AC JC KC TC QC", Rules.Straight, Rules.StraightFlush)]
        [InlineData("Black: 7H 3D 4S 5C 6D-White: AC 4C KC TC QC", Rules.Straight, Rules.Flush)]
        public void GetPokerType(string source, Rules firstRule, Rules secondRule)
        {
            var bridge = new Bridge.Bridge(source);
            var type1 = bridge.GetRules(bridge.FirstPlayerAndPokers.Pokers);
            var type2 = bridge.GetRules(bridge.SecondPlayerAndPokers.Pokers);
            Assert.Equal(firstRule, type1);
            Assert.Equal(secondRule, type2);
        }

        [Theory]
        [InlineData("Black: 2H 3D 5S 2C 6D-White: 2H 6D AS KC 3D", "Black", "", Rules.Pair)] // different rules
        [InlineData("Black: 2H 3D 5S AC 6D-White: 5H 6D 2S AC 3D", "Tie", "", null)]
        [InlineData("Black: 2H 3D 5S AC 6D-White: KH 6D 2S QC 3D", "Black", "Ace", Rules.HighCard)]
        [InlineData("Black: 9H 3D 2S KC 6D-White: 2H TD 6S KC 3D", "White", "Ten", Rules.HighCard)]
        [InlineData("Black: TH 3D 6S KC KD-White: KH TD 6S KC 3D", "Tie", "", null)]
        [InlineData("Black: 9H 3D 2S KC KD-White: TH TD 6S KC 3D", "Black", "King", Rules.Pair)]
        [InlineData("Black: 9H 3D 2S KC KD-White: KH TD 6S KC 3D", "White", "Ten", Rules.Pair)]
        [InlineData("Black: TH TD 6S KC KD-White: TH TD KS KC 6D", "Tie", "", null)]
        [InlineData("Black: 9H 9D 2S KC KD-White: KH TD TS KC 3D", "White", "Ten", Rules.TwoPairs)]
        [InlineData("Black: 9H 9D 2S KC KD-White: KH 9D 9S KC 3D", "White", "3", Rules.TwoPairs)]
        [InlineData("Black: KH TD 6S KC KD-White: TH KD KS KC 6D", "Tie", "", null)]
        [InlineData("Black: 9H 9D 2S KC 9C-White: KH TD TS TC 3D", "White", "Ten", Rules.ThreeOfAKind)]
        [InlineData("Black: 9H 9D 2S 9C KD-White: KH 9D 9S 9C 3D", "White", "3", Rules.ThreeOfAKind)]
        [InlineData("Black: 9H 9D 2S 9C KD-White: AH 9D 9S 9C 3D", "White", "Ace", Rules.ThreeOfAKind)]
        [InlineData("Black: 9H TD 8S JC 7D-White: 8H 9D TS JC 7D", "Tie", "", null)]
        [InlineData("Black: 9H TD 8S JC 7D-White: AH TD KS JC QD", "White", "Ace", Rules.Straight)]
        [InlineData("Black: 2H 3H 5H AH 6H-White: 5H 6H 2H AH 3H", "Tie", "", null)]
        [InlineData("Black: 2S 3S 5S AS 6S-White: KD 6D 2D QD 3D", "Black", "Ace", Rules.Flush)]
        [InlineData("Black: KH TD TS KC KD-White: TH KD KS KC TD", "Tie", "", null)]
        [InlineData("Black: 9H 9D 2S 2C 9C-White: KH TD TS TC KD", "White", "Ten", Rules.FullHouse)]
        [InlineData("Black: 9H 9D KS 9C KD-White: 3H 9D 9S 9C 3D", "Black", "King", Rules.FullHouse)]
        [InlineData("Black: KH TD KS KC KD-White: TH KD KS KC KD", "Tie", "", null)]
        [InlineData("Black: 9H 9D 9S 2C 9C-White: KH TD TS TC TH", "White", "Ten", Rules.FourOfAKind)]
        [InlineData("Black: 9H 9D KS 9C 9S-White: 9H 9D 9S 9C 3D", "Black", "King", Rules.FourOfAKind)]
        [InlineData("Black: 9D TD 8D JD 7D-White: 8S 9S TS JS 7S", "Tie", "", null)]
        [InlineData("Black: 9C TC 8C JC 7C-White: AH TH KH JH QH", "White", "Ace", Rules.StraightFlush)]
        public void ComparePokers(string source, string player, string pokerValue, Rules? rule)
        {
            var bridge = new Bridge.Bridge(source);
            var comparePokers = bridge.ComparePokers();
            Assert.Equal(player, comparePokers.Player);
            Assert.Equal(pokerValue, comparePokers.SourcePokerValue);
            Assert.Equal(rule, comparePokers.Rule);
        }
    }
}