namespace Assets.Scripts
{
    public interface IInteractable
    {

        void Interact(Player source);

        void AlternateInteract(Player source);
    }
}