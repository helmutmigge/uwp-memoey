using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cache
{

    public struct ByteSpan
    {
        private readonly ulong _byte;
        public const long bytePerKilobyte = 1024;
        public const long bytePerMegabyte = 1024 * 1024;
        public const long bytePerGigabyte = 1024 * 1024 * 1024;


        private ByteSpan(ulong _byte)
        {
            this._byte = _byte;
        }

        public ulong Byte => _byte;

        public double Kilobyte => _byte / bytePerKilobyte;

        public double Megabyte => _byte / bytePerMegabyte;
        public double Gigabyte => _byte / bytePerGigabyte;

        public static ByteSpan FromKilobyte(ulong kilobyte)
        {
            return new ByteSpan(kilobyte * bytePerKilobyte);
        }
        public static ByteSpan FromMegabyte(ulong megabyte)
        {
            return new ByteSpan(megabyte * bytePerMegabyte);
        }
        public static ByteSpan FromGigabyte(ulong gigabyte)
        {
            return new ByteSpan(gigabyte * bytePerGigabyte);
        }
        public static ByteSpan FromByte(ulong _byte)
        {
            return new ByteSpan(_byte);
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly List<byte[]> _buffer = new List<byte[]>();
        private DispatcherTimer dispatcherTimer;
        public MainPage()
        {
            this.InitializeComponent();
            MemoryManager.AppMemoryUsageDecreased += OnMemoryUsageDecreased;
            MemoryManager.AppMemoryUsageIncreased += OnMemoryUsageIncreased;
            MemoryManager.AppMemoryUsageLimitChanging += OnMemoryUsageLimitChanging;

            BuilderDispatcherTimer().Start();
            
        }

        public DispatcherTimer BuilderDispatcherTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += OnTimeChanged;
            return dispatcherTimer;
        }

        private void OnTimeChanged(object sender, object e)
        {
            UpdateMemoryData();
        }


        public void UpdateMemoryData()
        {
            MemoryUsageLimit = ByteSpan.FromByte(MemoryManager.AppMemoryUsageLimit).Megabyte;
            MemoryUsage = ByteSpan.FromByte(MemoryManager.AppMemoryUsage).Megabyte;
        }

        private void OnMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            UpdateMemoryData();
        }

        private void OnMemoryUsageIncreased(object sender, object e)
        {
            Debug.WriteLine("Memory Usage Increased");
            UpdateMemoryData();
        }

        private void OnMemoryUsageDecreased(object sender, object e)
        {
            Debug.WriteLine("Memory Usage Decreased");
            UpdateMemoryData();
        }



        public double MemoryUsage
        {
            get { return (double)GetValue(MemoryUsageProperty); }
            set { SetValue(MemoryUsageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MemoryUsage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MemoryUsageProperty =
            DependencyProperty.Register("MemoryUsage", typeof(double), typeof(MainPage), new PropertyMetadata(0.0));



        public double MemoryUsageLimit
        {
            get => (double)GetValue(MemoryUsageLimitProperty);
            set => SetValue(MemoryUsageLimitProperty, value);
        }

        // Using a DependencyProperty as the backing store for MemoryUsageLimit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MemoryUsageLimitProperty =
            DependencyProperty.Register("MemoryUsageLimit", typeof(double), typeof(MainPage), new PropertyMetadata(0.0));

        private void AddItemClick(object sender, RoutedEventArgs e)
        {
            
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    _buffer.Add(new byte[10 * 1024 * 1024]);
                }
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine($"Out Memory {ex.Message}");
            }
        }

        private void ClearAllClick(object sender, RoutedEventArgs e)
        {
            _buffer.Clear();
        }
    }
}
