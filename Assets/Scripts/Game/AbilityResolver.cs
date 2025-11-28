using System.Collections.Generic;
using UnityEngine;
public static class AbilityResolver
{
    public static void ResolveTurn(
        GameState state,
        List<int> p0Cards,
        List<int> p1Cards)
    {
        int p0Power = SumPower(p0Cards);
        int p1Power = SumPower(p1Cards);
        state.player0.score += p0Power;
        state.player1.score += p1Power;
        // Resolving the cards in order, player 0 then 1
        ResolveAbilitiesForPlayer(state, state.player0, state.player1, p0Cards);
        ResolveAbilitiesForPlayer(state, state.player1, state.player0, p1Cards);
    }
    private static int SumPower(List<int> cardIds)
    {
        int sum = 0;
        foreach (int id in cardIds)
        {
            var card = CardDatabase.Instance.GetCard(id);
            if (card != null)
                sum += card.power;
        }
        return sum;
    }
    private static void ResolveAbilitiesForPlayer(
        GameState state,
        PlayerState self,
        PlayerState opponent,
        List<int> cards)
    {
        foreach (int id in cards)
        {
            var card = CardDatabase.Instance.GetCard(id);
            if (card == null || card.ability == null) continue;
            //getting the instance of the particular card
            var ability = card.ability;
            switch (ability.type)
            {
                case "GainPoints":
                    self.score += ability.value;
                    break;
                case "StealPoints":
                    int steal = Mathf.Min(ability.value, opponent.score);
                    opponent.score -= steal;
                    self.score += steal;
                    break;
                case "DoublePower":
                    self.score += card.power * (ability.value - 1);
                    break;
                case "DrawExtraCard":
                    break;
            }
        }
    }
}
