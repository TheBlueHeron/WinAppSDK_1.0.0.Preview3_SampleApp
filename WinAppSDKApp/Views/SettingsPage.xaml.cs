﻿using CommunityToolkit.Mvvm.DependencyInjection;

using WinAppSDKApp.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace WinAppSDKApp.Views
{
    // TODO WTS: Change the URL for your privacy policy, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            ViewModel = Ioc.Default.GetService<SettingsViewModel>();
            InitializeComponent();
        }
    }
}
