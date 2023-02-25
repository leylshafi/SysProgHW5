using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SysProgHW5;

public partial class MainWindow : Window
{
    public ObservableCollection<Thread> Created { get; set; }
    public ObservableCollection<Thread> Waiting { get; set; }
    public ObservableCollection<Thread> Working { get; set; }
    public int InitialCount { get; set; }
    private readonly Semaphore semaphore;
    int count= 0;
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        numeric.Value = 3;
        InitialCount =(int)numeric.Value;

        Created = new();
        Waiting = new();
        Working = new();

        semaphore = new(InitialCount, 3, "SEMAPHORE");
    }

    private void btnCreate_Click(object sender, RoutedEventArgs e)
    {
        var t = new Thread(ThreadWork);
        t.Name = "Thread " + count++;

        Created.Add(t);
    }
    private void ThreadWork(object? semaphore)
    {
        if (semaphore is Semaphore s)
        {
            Thread.Sleep(4000);

            if (s.WaitOne())
            {
                try
                {
                    var t = Thread.CurrentThread;
                    Dispatcher.Invoke(() => Waiting.Remove(t));
                    Dispatcher.Invoke(() => Working.Add(t));
                    Thread.Sleep(8000);
                    Dispatcher.Invoke(() => Working.Remove(t));
                }
                catch(Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    s.Release();
                }
                
               
            }
        }
    }
    private void Created_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (createdList.SelectedItem is Thread t)
        {
            Created.Remove(t);

            Waiting.Add(t);
            t.Start(semaphore);
        }
    }

    private void numeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if(numeric.Value < 0)
        {
            numeric.Value = 0;
        }

        InitialCount=(int)numeric.Value;

    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        numeric.Value = 3;
    }
}
