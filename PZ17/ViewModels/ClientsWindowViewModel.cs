using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using PZ17.Models;

namespace PZ17.ViewModels;

public class ClientsWindowViewModel : ViewModelBase {
    public ObservableCollection<Client> Users { get; set; } = new ObservableCollection<Client>();

    public ClientsWindowViewModel() {
        GetDataFromDb();
    }

    private async void GetDataFromDb() {
        using var db = new Database();

        var users = db.Get<Client>("clients");
        foreach (var user in users) {
            Users.Add(user);
        }
    }
}