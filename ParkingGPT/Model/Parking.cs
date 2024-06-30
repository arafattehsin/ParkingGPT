using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ParkingGPT.Model
{
    public partial class Parking : ObservableObject
    {
        [property: JsonPropertyName("description")]
        [ObservableProperty]
        public string description;

        [property: JsonPropertyName("decision")]
        [ObservableProperty]
        public bool decision;
    }
}
