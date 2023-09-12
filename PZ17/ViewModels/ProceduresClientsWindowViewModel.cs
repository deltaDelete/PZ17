using System.Collections.ObjectModel;
using PZ17.Models;

namespace PZ17.ViewModels; 

public class ProceduresClientsWindowViewModel : ViewModelBase {
    public ObservableCollection<ProcedureClient> ProceduresClients { get; set; } = new();

    public ProceduresClientsWindowViewModel() {
        GetDataFromDb();
    }

    private async void GetDataFromDb() {
        using var db = new Database();

        var list = db.Get<ProcedureClient>("procedures_clients");
        foreach (var item in list) {
            ProceduresClients.Add(item);
        }
    }
}