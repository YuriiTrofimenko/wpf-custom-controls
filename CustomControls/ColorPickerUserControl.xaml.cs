using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for ColorPickerUserControl.xaml
    /// </summary>
    
    /*1: выбираем, от какого класса из иерархии WPF наследоватьс€*/
    public partial class ColorPickerUserControl : System.Windows.Controls.UserControl
    {
        /*2: объ€вл€ем пол€ свойств зависимости*/
        public static DependencyProperty ColorProperty;
        public static DependencyProperty RedProperty;
        public static DependencyProperty GreenProperty;
        public static DependencyProperty BlueProperty;

        public ColorPickerUserControl()
        {
            InitializeComponent();
            SetUpCommands();
        }

        private void SetUpCommands()
        {
            // «десь при необходимости добавл€ть наборы команд, например:
            CommandBinding binding = new CommandBinding(ApplicationCommands.Undo, 
                UndoCommand_Executed, UndoCommand_CanExecute);
                this.CommandBindings.Add(binding);
        }

        /*3: регистрируем все свойства зависимости*/
        static ColorPickerUserControl()
        {
            //в аргументах указываем:
            //1 - им€ свойства
            //2 - тип значени€ свойства
            //3 - тип контрола-владельца свойства
            //4 - объект метаданных, его аргументы:
            //  4.1 - значение св-ва по умолчанию
            //  4.2 - указание на функцию обратного вызова ее по имени (будут созданы ниже)
            ColorProperty = DependencyProperty.Register("Color", typeof(Color),
                typeof(ColorPickerUserControl),
                new FrameworkPropertyMetadata(Colors.Black, new PropertyChangedCallback(OnColorChanged)));

            RedProperty = DependencyProperty.Register("Red", typeof(byte),
                typeof(ColorPickerUserControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged)));

            GreenProperty = DependencyProperty.Register("Green", typeof(byte),
                typeof(ColorPickerUserControl),
                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged)));

            BlueProperty = DependencyProperty.Register("Blue", typeof(byte),
                typeof(ColorPickerUserControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged)));

            //здесь можно зарегистрировать наборы команд:
            CommandManager.RegisterClassCommandBinding(typeof(ColorPickerUserControl),
                new CommandBinding(ApplicationCommands.Undo,
                UndoCommand_Executed, UndoCommand_CanExecute));
        }

        /*4: обертки установки и чтени€ значений свойств зависимости*/
        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public byte Red
        {
            get
            {
                return (byte)GetValue(RedProperty);
            }
            set
            {
                SetValue(RedProperty, value);
            }
        }

        public byte Green
        {
            get
            {
                return (byte)GetValue(GreenProperty);
            }
            set
            {
                SetValue(GreenProperty, value);
            }
        }

        public byte Blue
        {
            get
            {
                return (byte)GetValue(BlueProperty);
            }
            set
            {
                SetValue(BlueProperty, value);
            }
        }

        /*5: создаем функции обратного вызова*/
        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;

            Color oldColor = (Color)e.OldValue;
            Color newColor = (Color)e.NewValue;
            colorPicker.Red = newColor.R;
            colorPicker.Green = newColor.G;
            colorPicker.Blue = newColor.B;

            //newColor.A;

            //после изменени€ св-в Red, Green, Blue из этой ф-ции
            //ф-ци€ OnColorRGBChanged, вызываема€ 3 раза,
            //не вызовет ф-цию OnColorChanged на повторное исполнение

            //здесь можно сохран€ть предыдущее значение св-ва:
            colorPicker.previousColor = oldColor;
            //вызов издател€ событи€ (создетс€ ниже)
            colorPicker.OnColorChanged(oldColor, newColor);
        }

        private static void OnColorRGBChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;
            Color color = colorPicker.Color;
            if (e.Property == RedProperty)
                color.R = (byte)e.NewValue;
            else if (e.Property == GreenProperty)
                color.G = (byte)e.NewValue;
            else if (e.Property == BlueProperty)
                color.B = (byte)e.NewValue;

            colorPicker.Color = color;
        }

        /*6: объ€вл€ем и регистрируем маршрутизируемоое событие с аргументами:
         1 - им€ событи€
         2 - стратеги€ маршрутизации
         3 - сигнатура
         4 - тип контрола-владельца событи€*/
        public static readonly RoutedEvent ColorChangedEvent =
           EventManager.RegisterRoutedEvent("ColorChanged", RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPickerUserControl));

        /*7: обертка дл€ добавлени€/удалени€ подписчиков на событие*/
        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        /*8: создаем издател€ событи€:*/
        private void OnColorChanged(Color oldValue, Color newValue)
        {
            RoutedPropertyChangedEventArgs<Color> args = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue);
            args.RoutedEvent = ColorPickerUserControl.ColorChangedEvent;
            RaiseEvent(args);
        }

        /*здесь при необходимости описывать содержимое команд,
         * например(команды отмены или восстановлени€ изменений):*/
        private Color? previousColor;
        private static void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;
            e.CanExecute = colorPicker.previousColor.HasValue;
        }
        private static void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Use simple reverse-or-redo Undo (like Notepad).
            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;            
            colorPicker.Color = (Color)colorPicker.previousColor;
        }
    }
}