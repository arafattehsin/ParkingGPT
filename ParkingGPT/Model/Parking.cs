using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGPT.Model
{
    public partial class Parking : ObservableObject
    {
        [ObservableProperty]
        public string description;

        [ObservableProperty]
        public bool decision;
    }
}
