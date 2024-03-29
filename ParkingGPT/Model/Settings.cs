﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGPT.Model
{
    public partial class Settings : ObservableObject
    {
        [ObservableProperty]
        public bool isUseOpenAI;

        public string EndpointURL { get; set; }

        public string EndpointKey { get; set; }

        public string DeploymentModel { get; set; }
    }
}
