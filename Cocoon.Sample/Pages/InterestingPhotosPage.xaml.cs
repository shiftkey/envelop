﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cocoon.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Cocoon.Sample.Pages
{
    [PageExport(SpecialPageNames.Home)]
    public sealed partial class InterestingPhotosPage : Cocoon.Sample.Common.LayoutAwarePage
    {
        public InterestingPhotosPage()
        {
            this.InitializeComponent();
        }
    }
}
