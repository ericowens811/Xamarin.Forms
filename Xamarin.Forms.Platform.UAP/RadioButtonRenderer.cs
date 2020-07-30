﻿using System;
using System.ComponentModel;
using Windows.Devices.Radios;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WThickness = Windows.UI.Xaml.Thickness;

namespace Xamarin.Forms.Platform.UWP
{
	public class RadioButtonRenderer : ViewRenderer<RadioButton, FormsRadioButton>
	{
		bool _fontApplied;

		protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var button = new FormsRadioButton();

					button.Loaded += ButtonOnLoaded;
					button.Checked += OnRadioButtonCheckedOrUnchecked;
					button.Unchecked += OnRadioButtonCheckedOrUnchecked;

					SetNativeControl(button);
				}
				else
				{
					WireUpFormsVsm();
				}

				UpdateContent();

				//TODO: We may want to revisit this strategy later. If a user wants to reset any of these to the default, the UI won't update.
				if (Element.IsSet(VisualElement.BackgroundColorProperty) && Element.BackgroundColor != (Color)VisualElement.BackgroundColorProperty.DefaultValue)
					UpdateBackground();

				if (Element.IsSet(RadioButton.TextColorProperty) && Element.TextColor != (Color)RadioButton.TextColorProperty.DefaultValue)
					UpdateTextColor();

				if (Element.IsSet(RadioButton.BorderColorProperty) && Element.BorderColor != (Color)RadioButton.BorderColorProperty.DefaultValue)
					UpdateBorderColor();

				if (Element.IsSet(RadioButton.BorderWidthProperty) && Element.BorderWidth != (double)RadioButton.BorderWidthProperty.DefaultValue)
					UpdateBorderWidth();

				if (Element.IsSet(RadioButton.CornerRadiusProperty) && Element.CornerRadius != (int)RadioButton.CornerRadiusProperty.DefaultValue)
					UpdateBorderRadius();

				if (Element.IsSet(RadioButton.PaddingProperty) && Element.Padding != (Thickness)RadioButton.PaddingProperty.DefaultValue)
					UpdatePadding();

				UpdateFont();
				UpdateCheck();
			}
		}

		void ButtonOnLoaded(object o, RoutedEventArgs routedEventArgs)
		{
			WireUpFormsVsm();
		}

		void WireUpFormsVsm()
		{
			if (Element.UseFormsVsm())
			{
				InterceptVisualStateManager.Hook(Control.GetFirstDescendant<Windows.UI.Xaml.Controls.Grid>(), Control, Element);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == RadioButton.ContentProperty.PropertyName || e.PropertyName == Button.ImageSourceProperty.PropertyName)
			{
				UpdateContent();
			}
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				UpdateBackground();
			}
			else if (e.PropertyName == RadioButton.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.IsOneOf(RadioButton.FontFamilyProperty, RadioButton.FontSizeProperty, RadioButton.FontAttributesProperty))
			{
				UpdateFont();
			}
			else if (e.PropertyName == RadioButton.BorderColorProperty.PropertyName)
			{
				UpdateBorderColor();
			}
			else if (e.PropertyName == RadioButton.BorderWidthProperty.PropertyName)
			{
				UpdateBorderWidth();
			}
			else if (e.PropertyName == RadioButton.CornerRadiusProperty.PropertyName)
			{
				UpdateBorderRadius();
			}
			else if (e.PropertyName == RadioButton.PaddingProperty.PropertyName)
			{
				UpdatePadding();
			}
			else if (e.PropertyName == RadioButton.IsCheckedProperty.PropertyName)
			{
				UpdateCheck();
			}
		}

		protected override void UpdateBackgroundColor()
		{
			// Button is a special case; we don't want to set the Control's background
			// because it goes outside the bounds of the Border/ContentPresenter, 
			// which is where we might change the BorderRadius to create a rounded shape.
			return;
		}

		protected override bool PreventGestureBubbling { get; set; } = true;

		void OnRadioButtonCheckedOrUnchecked(object sender, RoutedEventArgs e)
		{
			if (Element == null || Control == null)
			{
				return;
			}

			Element.IsChecked = Control.IsChecked == true;
		}

		void UpdateBackground()
		{
			Control.BackgroundColor = Element.BackgroundColor != Color.Default ? Element.BackgroundColor.ToBrush() : (Brush)Windows.UI.Xaml.Application.Current.Resources["ButtonBackgroundThemeBrush"];
		}

		void UpdateBorderColor()
		{
			Control.BorderBrush = Element.BorderColor != Color.Default ? Element.BorderColor.ToBrush() : (Brush)Windows.UI.Xaml.Application.Current.Resources["ButtonBorderThemeBrush"];
		}

		void UpdateBorderRadius()
		{
			Control.BorderRadius = Element.CornerRadius;
		}

		void UpdateBorderWidth()
		{
			Control.BorderThickness = Element.BorderWidth == (double)RadioButton.BorderWidthProperty.DefaultValue ? new WThickness(3) : new WThickness(Element.BorderWidth);
		}

		void UpdateContent()
		{
			if (!(Element.Content is string text))
			{
				throw new InvalidOperationException($"{nameof(RadioButtonRenderer)} only supports string values for the {nameof(RadioButton)} {RadioButton.ContentProperty.PropertyName} property.");
			}

			Control.Content = text;
		}

		void UpdateFont()
		{
			if (Control == null || Element == null)
				return;

			Font font = Font.OfSize(Element.FontFamily, Element.FontSize).WithAttributes(Element.FontAttributes);

			if (font == Font.Default && !_fontApplied)
				return;

			Font fontToApply = font == Font.Default ? Font.SystemFontOfSize(NamedSize.Medium) : font;

			Control.ApplyFont(fontToApply);
			_fontApplied = true;
		}

		void UpdateTextColor()
		{
			Control.Foreground = Element.TextColor != Color.Default ? Element.TextColor.ToBrush() : (Brush)Windows.UI.Xaml.Application.Current.Resources["DefaultTextForegroundThemeBrush"];
		}

		void UpdatePadding()
		{
			Control.Padding = new WThickness(
				Element.Padding.Left,
				Element.Padding.Top,
				Element.Padding.Right,
				Element.Padding.Bottom
			);
		}

		void UpdateCheck()
		{
			if (Control == null || Element == null)
			{
				return;
			}

			Control.IsChecked = Element.IsChecked;
		}
	}
}
