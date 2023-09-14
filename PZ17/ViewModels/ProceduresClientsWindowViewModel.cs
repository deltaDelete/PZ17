using System.Collections.ObjectModel;
using System.Linq;
using PZ17.Models;

namespace PZ17.ViewModels; 

public class ProceduresClientsWindowViewModel : ViewModelBase {
    private ObservableCollection<ProcedureClient> _proceduresClients = new();

    public ObservableCollection<ProcedureClient> ProceduresClients
    {
        get => _proceduresClients;
        set
        {
            if (Equals(value, _proceduresClients)) return;
            _proceduresClients = value;
            RaisePropertyChanged();
        }
    }

    public ProceduresClientsWindowViewModel() {
        GetDataFromDb();
    }

    private async void GetDataFromDb() {
        var db = new Database();

        var list = db.GetAsync<ProcedureClient>();
        var enumerator = list.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync()) {
            ProceduresClients.Add(enumerator.Current);
        }

        db = new Database();
        var joined = ProceduresClients.Select(
            it =>
            {
                it.Procedure = db.GetById<Procedure>(it.ProcedureId);
                it.Client = db.GetById<Client>(it.ClientId);
                return it;
            }
        );
        ProceduresClients = new(joined);
    }
}