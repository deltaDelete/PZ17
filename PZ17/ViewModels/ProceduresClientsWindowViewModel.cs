using System.Collections.ObjectModel;
using PZ17.Models;

namespace PZ17.ViewModels; 

public class ProceduresClientsWindowViewModel : ViewModelBase {
    public ObservableCollection<ProcedureClient> ProceduresClients { get; set; } = new();

    public ProceduresClientsWindowViewModel() {
        GetDataFromDb();
    }

    private async void GetDataFromDb() {
        await using var db = new Database();

        var list = db.GetAsync<ProcedureClient>();
        var enumerator = list.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync()) {
            ProceduresClients.Add(enumerator.Current);
        }
    }
}