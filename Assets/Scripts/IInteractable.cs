namespace Assets.Scripts
{
    public interface IInteractable
    {
        void Select();
        void Unselect();

        void Interact(Player source);
    }
}