namespace ITCC.VkPlayer.Interfaces
{
    public interface ILongTaskRunner
    {
        void BeginOperation(string message);

        void EndOperation();

        void CancelOperations();
    }
}