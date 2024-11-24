﻿using ChiclanaRecordsNET.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChiclanaRecordsNET.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para SearchListView.xaml
    /// </summary>
    public partial class SearchListView : UserControl
    {
        public SearchListView()
        {
            InitializeComponent();
        }

        private void ScrollingTitle_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                StartScrollingAnimation(textBlock);
            }
        }

        public void StartScrollingAnimation(TextBlock textBlock)
        {
            var scrollTransform = textBlock.RenderTransform as TranslateTransform;
            if (scrollTransform == null)
                return;

            if (textBlock.ActualWidth < 310) return;

            Duration duration;

            var durations = new Dictionary<int, int>
            {
                { 900, 18 }, 
                { 800, 15 },  
                { 700, 13 },  
                { 600, 12 },  
                { 500, 9 },   
                { 400, 6 }, 
                { 300, 4 }    
            };
            int selectedDuration = 3;

            var matchingDuration = durations
                .Where(pair => textBlock.ActualWidth > pair.Key)
                .OrderByDescending(pair => pair.Key)
                .Select(pair => pair.Value)
                .FirstOrDefault();

            selectedDuration = matchingDuration == 0 ? selectedDuration : matchingDuration;


            duration = new Duration(TimeSpan.FromSeconds(selectedDuration));

            var animation = new DoubleAnimation
            {
                From = 0,
                To = -textBlock.ActualWidth + 260,
                Duration = duration,
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = false
            };

            scrollTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
    }
}
