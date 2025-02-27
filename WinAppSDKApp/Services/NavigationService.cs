﻿using WinAppSDKApp.Contracts.Services;
using WinAppSDKApp.Contracts.ViewModels;
using WinAppSDKApp.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace WinAppSDKApp.Services
{
    /// <summary>
    /// The <see cref="INavigationService"/> implementation of this application.
    /// </summary>
    /// <seealso>https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/WinUI/navigation.md</seealso>
    public class NavigationService : INavigationService
    {
        private readonly IPageService _pageService;
        private object _lastParameterUsed;
        private Frame _frame;

        public event NavigatedEventHandler Navigated;

        public Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = App.MainWindow.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public bool CanGoBack => Frame.CanGoBack;

        public NavigationService(IPageService pageService)
        {
            _pageService = pageService;
        }

        private void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated += OnNavigated;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated -= OnNavigated;
            }
        }

        public bool GoBack()
        {
            if (CanGoBack)
            {
                var vmBeforeNavigation = _frame.GetPageViewModel();
                _frame.GoBack();
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// The page key corresponds to the Page's ViewModel FullName.
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="parameter"></param>
        /// <param name="clearNavigation"></param>
        /// <returns></returns>
        public bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false)
        {
            System.Type pageType = _pageService.GetPageType(pageKey);

            if (_frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed)))
            {
                _frame.Tag = clearNavigation;
                object vmBeforeNavigation = _frame.GetPageViewModel();
                bool navigated = _frame.Navigate(pageType, parameter);
                if (navigated)
                {
                    _lastParameterUsed = parameter;
                    if (vmBeforeNavigation is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatedFrom();
                    }
                }

                return navigated;
            }

            return false;
        }

        public void CleanNavigation()
        {
            _frame.BackStack.Clear();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                bool clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    frame.BackStack.Clear();
                }

                if (frame.GetPageViewModel() is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.Parameter);
                }

                Navigated?.Invoke(sender, e);
            }
        }
    }
}
