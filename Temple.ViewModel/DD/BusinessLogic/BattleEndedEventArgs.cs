namespace Temple.ViewModel.DD.BusinessLogic;

public enum BattleResult
{
    Victory,
    Defeat
}

public class BattleEndedEventArgs : EventArgs
{
    public readonly BattleResult BattleResult;

    public BattleEndedEventArgs(BattleResult battleResult)
    {
        BattleResult = battleResult;
    }
}