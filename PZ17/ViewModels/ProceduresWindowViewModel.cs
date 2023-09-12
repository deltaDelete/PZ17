using System.Collections.ObjectModel;
using PZ17.Models;

namespace PZ17.ViewModels; 

public class ProceduresWindowViewModel : ViewModelBase {
    public ObservableCollection<Procedure> Procedures { get; set; } = new();

    public ProceduresWindowViewModel() {
        GetDataFromDb();
    }

    private async void GetDataFromDb() {
        using var db = new Database();

        var procedures = db.Get<Procedure>("procedures");
        foreach (var procedure in procedures) {
            Procedures.Add(procedure);
        }
    }
}