﻿using CommunityToolkit.Mvvm.DependencyInjection;
using WinAppSDKApp.Contracts.Services;
using WinAppSDKApp.Helpers;
using WinAppSDKApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml.Media;

namespace WinAppSDKApp.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        internal UIElement TitleBar => CustomTitleBar;

        public ShellViewModel ViewModel { get; }

        public ShellPage(ShellViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            txtTitle.Text = ResourceExtensions.AppTitleKey.GetLocalized();
            ViewModel.NavigationService.Frame = shellFrame;
            ViewModel.NavigationViewService.Initialize(navigationView);
            ActualThemeChanged += OnActualThemeChanged;
        }

        /// <summary>
        /// Makes sure that the title bar obeys the system theme.
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object e)
        {
            UISettings DefaultTheme = new();

            txtTitle.Foreground = new SolidColorBrush(DefaultTheme.GetColorValue(UIColorType.Foreground));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            KeyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(_backKeyboardAccelerator);
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            KeyboardAccelerator keyboardAccelerator = new() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            INavigationService navigationService = Ioc.Default.GetService<INavigationService>();
            bool result = navigationService.GoBack();
            args.Handled = result;
        }
    }

}