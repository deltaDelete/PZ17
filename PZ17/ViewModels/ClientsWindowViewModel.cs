using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using PZ17.Models;

namespace PZ17.ViewModels;

public class ClientsWindowViewModel : ViewModelBase {
    private string _searchQuery = "";
    private ObservableCollection<Client> _users = new();

    public ObservableCollection<Client> Users {
        get => _users;
        set {
            if (Equals(value, _users)) return;
            _users = value;
            RaisePropertyChanged();
        }
    }

    public string SearchQuery {
        get => _searchQuery;
        set {
            if (value == _searchQuery) return;
            _searchQuery = value;
            RaisePropertyChanged();
        }
    }

    public ClientsWindowViewModel() {
        GetDataFromDb();
        PropertyChanged += OnSearchChanged;
    }

    private void OnSearchChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName != nameof(SearchQuery)) {
            return;
        }

        if (SearchQuery == "") {
            GetDataFromDb();
            return;
        }
        
        Users = new(Users.Where(
            it => it.LastName.ToLower().Contains(SearchQuery.ToLower())
                  || it.LastName.ToLower().Contains(SearchQuery.ToLower())
        ));
    }

    private async void GetDataFromDb() {
        using var db = new Database();

        var users = db.Get<Client>();
        Users = new(users);
    }
}