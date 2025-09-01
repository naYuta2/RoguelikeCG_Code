public interface IBuffable{
    public void AddBuff(Buff buff);
    public void RemoveBuff(BuffType type);
    public void ApplyBuffEffectsEachTurn();
}