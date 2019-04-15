namespace Infrastructure.Interface.Manager
{
    public interface ICoinHistoryManager
    {
        System.Threading.Tasks.Task<Model.CoinHistory.CoinHistoryDisplayModel> Search(Model.CoinHistory.CoinHistorySearchModel model);
    }
}
