namespace Aotenjo
{
    public interface IRegisterable
    {
        public void Register(RegistrationManager manager)
        {
            manager.Register(this, GetType());
        }

        public string GetRegName();
    }
}