﻿using CommunityToolkit.Mvvm.ComponentModel;
using WinAppSDKApp.Contracts.Services;
using WinAppSDKApp.ViewModels;
using WinAppSDKApp.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinAppSDKApp.Services
{
    public class PageService : IPageService
    {
        private readonly Dictionary<string, Type> _pages = new();

        public PageService()
        {
            Configure<MainViewModel, MainPage>();
            Configure<DataViewModel, DataGridPage>();
            Configure<SettingsViewModel, SettingsPage>();
        }

        public Type GetPageType(string key)
        {
            Type pageType;
            lock (_pages)
            {
                if (!_pages.TryGetValue(key, out pageType))
                {
                    throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
                }
            }

            return pageType;
        }

        private void Configure<VM, V>()
            where VM : ObservableObject
            where V : Page
        {
            lock (_pages)
            {
                string key = typeof(VM).FullName;
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException($"The key {key} is already configured in PageService");
                }

                Type type = typeof(V);
                if (_pages.Any(p => p.Value == type))
                {
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
                }

                _pages.Add(key, type);
            }
        }
    }
}
