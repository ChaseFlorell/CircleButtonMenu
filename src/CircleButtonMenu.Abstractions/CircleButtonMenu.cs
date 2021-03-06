﻿using RealSimpleCircle.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace CircleButtonMenu.Abstractions
{
    public class CircleButtonMenu : ContentView
    {
        public bool IsOpened = false;
        public Grid Grid;
        public Image RootImage;
        public List<View> Buttons;
        public CircleButtonMenu()
        {
            Buttons = new List<View>();
            Grid = new Grid
            {
                // -- the padding is kind of a hack since --
                // After translating controls they can only be fired
                // if they are within the container bounds. Without the 
                // padding they will be outside the bounds and not fire
                // correctly.
                // We should figure out a better way to do this, but it is
                // okay for now
                Padding = new Thickness(0, 0, 0, 300),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(0, 15, 15, 0)
            };



            CreateRootButton();
            Content = Grid;
        }

        private void TapRootButton(object sender, EventArgs e)
        {
            if (IsOpened)
            {
                RootImage.Source = OpenImageSource;
                for (int index = 0; index < Buttons.Count; index++)
                {
                    Buttons[index].TranslateTo(0, 0);
                }
            }
            else
            {
                RootImage.Source = CloseImageSource;
                for (int index = 0; index < Buttons.Count; index++)
                {
                    var baseDistance = 80;
                    var distance = baseDistance * (index + 1);
                    Buttons[index].TranslateTo(0, distance);
                }

                Grid.IsVisible = false;
                Grid.IsVisible = true;
            }

            IsOpened = !IsOpened;
        }

        private void AddButton(ImageSource image)
        {
            var newControl = new Grid();
            newControl.Children.Add(new Circle
            {
                WidthRequest = 50,
                HeightRequest = 50,
                Margin = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Active = true,
                FillColor = FillColor,
                StrokeColor = StrokeColor
            });
            newControl.Children.Add(new Image
            {
                Source = image,
                Margin = new Thickness(10)
            });

            Buttons.Add(newControl);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Command = IndexSelected;
            tapGesture.CommandParameter = Buttons.IndexOf(newControl);
            newControl.GestureRecognizers.Add(tapGesture);

            Grid.Children.Add(newControl);
        }

        public void CreateRootButton()
        {
            var newControl = new Grid();
            newControl.Children.Add(new Circle
            {
                WidthRequest = 50,
                HeightRequest = 50,
                Margin = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Active = true,
                FillColor = FillColor,
                StrokeColor = StrokeColor
            });

            RootImage = new Image
            {
                Source = OpenImageSource,
                Margin = new Thickness(10)
            };

            newControl.Children.Add(RootImage);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += TapRootButton;
            newControl.GestureRecognizers.Add(tapGesture);

            Grid.Children.Add(newControl);
        }

        public ICommand IndexSelected
        {
            get { return (ICommand)GetValue(IndexSelectedProperty); }
            set { SetValue(IndexSelectedProperty, value); }
        }

        public static readonly BindableProperty IndexSelectedProperty = BindableProperty.Create(
            nameof(IndexSelected),
            typeof(ICommand),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnIndexSelectedPropertyChanged);

        private static void OnIndexSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            if (context != null)
            {
                foreach (var view in context.Buttons)
                {
                    var tapGesture = (TapGestureRecognizer)view.GestureRecognizers.FirstOrDefault();
                    tapGesture.Command = (ICommand)newValue;
                }
            }
        }

        public ImageSource OpenImageSource
        {
            get { return (ImageSource)GetValue(OpenImageSourceProperty); }
            set { SetValue(OpenImageSourceProperty, value); }
        }

        public static readonly BindableProperty OpenImageSourceProperty = BindableProperty.Create(
            nameof(OpenImageSource),
            typeof(ImageSource),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnOpenImageSourcePropertyChanged);

        private static void OnOpenImageSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            if (context != null && context.IsOpened)
            {
                context.RootImage.Source = (ImageSource)newValue;
            }
        }

        public ImageSource CloseImageSource
        {
            get { return (ImageSource)GetValue(CloseImageSourceProperty); }
            set { SetValue(CloseImageSourceProperty, value); }
        }

        public static readonly BindableProperty CloseImageSourceProperty = BindableProperty.Create(
            nameof(CloseImageSource),
            typeof(ImageSource),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnCloseImageSourcePropertyChanged);

        private static void OnCloseImageSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            if (context != null & !context.IsOpened)
            {
                context.RootImage.Source = (ImageSource)newValue;
            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(CircleButtonMenu),
            null,
            propertyChanged: OnItemsSourcePropertyChanged);

        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            if (context != null)
            {
                var items = (IEnumerable)newValue;
                context.Grid.Children.Clear();
                context.Buttons.Clear();

                foreach (var item in items)
                {
                    ImageSource source = null;
                    if (item.GetType() == typeof(string))
                    {
                        source = ImageSource.FromFile((string)item);
                    }
                    else if (item.GetType() == typeof(ImageSource))
                    {
                        source = (ImageSource)item;
                    }

                    context.AddButton(source);
                }

                context.CreateRootButton();
            }
        }

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
            nameof(StrokeColor),
            typeof(Color),
            typeof(CircleButtonMenu),
            Color.Black,
            propertyChanged: OnStrokeColorPropertyChanged);

        private static void OnStrokeColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            var color = (Color)newValue;
            if (context != null && color != null)
            {
                foreach (var view in context.Grid.Children)
                {
                    UpdateColor(view, c => c.StrokeColor = color);
                }
            }
        }

        public Color FillColor
        {
            get { return (Color)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
            nameof(FillColor),
            typeof(Color),
            typeof(CircleButtonMenu),
            Color.Black,
            propertyChanged: OnFillColorPropertyChanged);

        private static void OnFillColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var context = (CircleButtonMenu)bindable;
            var color = (Color)newValue;
            if (context != null && color != null)
            {
                foreach (var view in context.Grid.Children)
                {
                    UpdateColor(view, c => c.FillColor = color);
                }
            }
        }

        private static void UpdateColor(View v, Action<Circle> updateProperties)
        {
            if (v.GetType() == typeof(Grid))
            {
                var grid = (Grid)v;
                foreach (var item in grid.Children)
                {
                    UpdateColor(item, updateProperties);
                }
            }
            else if (v.GetType() == typeof(Circle))
            {
                var circle = (Circle)v;
                updateProperties(circle);
            }
        }
    }
}
