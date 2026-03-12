using System;
using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Projekt2.ViewModels;

public partial class SklepViewModel : ViewModelBase
{
    private readonly ObservableCollection<string> _koszykItems;
    private readonly Action<string> _navigate;

    // Lista produktów w sklepie
    public ObservableCollection<string> Produkty { get; } = new()
    {
        "Jabłka",
        "Chleb",
        "Piwo",
        "Banany",
        "Czekolada",
        "Pianki",
        "Masło",
        "Pomidory",
        "Makaron",
        "Woda"
    };

    public SklepViewModel(ObservableCollection<string> koszykItems, Action<string> navigate)
    {
        _koszykItems = koszykItems;
        _navigate = navigate;
    }

    [RelayCommand]
    private void Dodaj(IList? selectedItems)
    {
        if (selectedItems != null && selectedItems.Count > 0)
        {
            var items = new System.Collections.Generic.List<string>();
            foreach (var item in selectedItems)
            {
                if (item is string s)
                    items.Add(s);
            }
            foreach (var s in items)
            {
                _koszykItems.Add(s);
            }
        }
    }

    [RelayCommand]
    private void Anuluj()
    {
        _navigate("Koszyk");
    }
}
