public interface IDamageable
{
    public void Damage(int value);
    public void Heal(int value);
    public void Death();
    public void Guard(int value);
    public void TurnStart();
}