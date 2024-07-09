using Logic.Player;

public interface IDamagable
{
    public bool Damage(Player damageBy, float damage, bool ignoreDamageBy = false);
}