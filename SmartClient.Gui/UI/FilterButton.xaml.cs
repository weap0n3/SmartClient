using System.Windows.Input;

namespace SmartClient.Gui.UI;

public partial class FilterButton : ContentView
{
	public FilterButton()
	{
		InitializeComponent();
        ButtonControl.Text = (string)LabelTextProperty.DefaultValue;

    }

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(FilterButton), propertyChanged: (bindable, oldValue, newValue) =>
        {
            if(bindable is FilterButton inst)
            {
                inst.ButtonControl.Text = (string)newValue;
            } 
        });
    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty CommandProperty =
    BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FilterButton), propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is FilterButton inst)
        {
            inst.ButtonControl.Command = (ICommand)newValue;
        }
    });
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
    BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FilterButton), propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is FilterButton inst)
        {
            inst.ButtonControl.CommandParameter = newValue;
        }
    });
    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly BindableProperty ImageAVisibleProperty =
    BindableProperty.Create(nameof(ImageAVisible), typeof(bool), typeof(FilterButton), propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is FilterButton inst)
        {
            inst.ImageAControl.IsVisible = (bool)newValue;
        }
    });
    public bool ImageAVisible
    {
        get => (bool)GetValue(ImageAVisibleProperty);
        set => SetValue(ImageAVisibleProperty, value);
    }

    public static readonly BindableProperty ImageDVisibleProperty =
    BindableProperty.Create(nameof(ImageDVisible), typeof(bool), typeof(FilterButton), propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is FilterButton inst)
        {
            inst.ImageDControl.IsVisible = (bool)newValue;
        }
    });
    public bool ImageDVisible
    {
        get => (bool)GetValue(ImageDVisibleProperty);
        set => SetValue(ImageDVisibleProperty, value);
    }
}