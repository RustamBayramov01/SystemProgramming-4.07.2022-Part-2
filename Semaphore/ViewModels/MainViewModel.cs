using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Semap.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel() { }

        private Thread item;
        private int CreareCommandCount = 0;
        private int otherCount = 6;
        private static int count = 6;
        private static int resultCount;

        public ObservableCollection<Thread> WorkingThreads { get; set; } = new ObservableCollection<Thread>();
        public ObservableCollection<Thread> CollectionThreads { get; set; } = new ObservableCollection<Thread>();
        private Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;
        public System.Threading.Semaphore Semaphore { get; set; } = new System.Threading.Semaphore(2, count);
        public ObservableCollection<Thread> WaitThreads { get; set; } = new ObservableCollection<Thread>();
        public int Pcount  {  get => otherCount; set { otherCount = value; RaisePropertyChanged(); }  }
        public RelayCommand UpdateCommand  {  get => new RelayCommand( () => { Pcount++; });  }
        public RelayCommand DownCommand {  get => new RelayCommand(  () => {  Pcount--; }); }
    

        public Thread Crselitem
        {
            get => item;
            set { item = value;  RaisePropertyChanged(); }
        }
        public RelayCommand CreateCommand
        {
            get => new RelayCommand(() =>
            {
                CreareCommandCount++;
                CollectionThreads.Add(new Thread(() =>
                {
                    bool Veirf = false;
                    while (!Veirf)
                    {
                        if (Semaphore.WaitOne(10000))
                        {
                            try
                            {
                                if (WorkingThreads.Count < count)
                                {
                                    Thread.Sleep(1000);
                                    Dispatcher.Invoke(() => WorkingThreads.Add(WaitThreads[0]));
                                    Dispatcher.Invoke(() => WaitThreads.RemoveAt(0));
                                    Thread.Sleep(1000);
                                }
                            }
                            finally
                            {
                                Veirf = true;
                                Dispatcher.Invoke(() => WorkingThreads.RemoveAt(0));
                                Semaphore.Release();
                            }
                        }
                    }
                })
                { Name = CreareCommandCount.ToString() });
            });
        }

        public RelayCommand DoubleClickCommand
        {
            get => new RelayCommand(() =>
            {
                if (Crselitem != null)
                {
                    int ResultId = Crselitem.ManagedThreadId;
                    var Thread = Crselitem;
                    Dispatcher.Invoke(() => WaitThreads.Add(Crselitem));
                    resultCount = WaitThreads.Count;
                    CollectionThreads.Remove(Crselitem);
                    Thread.Start();
                }
            });
        }

        
    }
}
