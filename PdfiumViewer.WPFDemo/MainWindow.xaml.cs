using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PdfiumViewer.WPFDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource tokenSource;
        Process currentProcess = Process.GetCurrentProcess();
        PdfDocument pdfDoc;
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void RenderToMemDCButton_Click(object sender, RoutedEventArgs e)
        {
            if (pdfDoc == null)
            {
                MessageBox.Show("First load the document");
                return;
            }

            int width = (int)(this.ActualWidth - 30) / 2;
            int height = (int)this.ActualHeight - 30;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                for (int i = 1; i < pdfDoc.PageCount; i++)
                {
                    imageMemDC.Source =
                        await
                            Task.Run<BitmapSource>(
                                new Func<BitmapSource>(
                                    () =>
                                    {
                                        tokenSource.Token.ThrowIfCancellationRequested();

                                        return RenderPageToMemDC(i, width, height);
                                    }
                            ), tokenSource.Token);

                    labelMemDC.Content = String.Format("Renderd Pages: {0}, Memory: {1} MB, Time: {2:0.0} sec",
                        i,
                        currentProcess.PrivateMemorySize64 / (1024 * 1024),
                        sw.Elapsed.TotalSeconds);

                    currentProcess.Refresh();

                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                tokenSource.Cancel();
                MessageBox.Show(ex.Message);
            }

            sw.Stop();
            labelMemDC.Content = String.Format("Rendered {0} Pages within {1:0.0} seconds, Memory: {2} MB",
                pdfDoc.PageCount,
                sw.Elapsed.TotalSeconds,
                currentProcess.PrivateMemorySize64 / (1024 * 1024));
        }

        private BitmapSource RenderPageToMemDC(int page, int width, int height)
        {
            var image = pdfDoc.Render(page, width, height, 96, 96, false);
            return BitmapHelper.ToBitmapSource(image);
        }

        private void LoadPDFButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                dialog.Title = "Open PDF File";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pdfDoc = PdfDocument.Load(dialog.FileName);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tokenSource = new CancellationTokenSource();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (tokenSource != null)
                tokenSource.Cancel();

            if (pdfDoc != null)
                pdfDoc.Dispose();
        }

        private void DoSearch_Click(object sender, RoutedEventArgs e)
        {
            string text = searchValueTextBox.Text;
            bool matchCase = matchCaseCheckBox.IsChecked.GetValueOrDefault();
            bool wholeWordOnly = wholeWordOnlyCheckBox.IsChecked.GetValueOrDefault();

            DoSearch(text, matchCase, wholeWordOnly);
        }

        private void DoSearch(string text, bool matchCase, bool wholeWord)
        {
            var matches = pdfDoc.Search(text, matchCase, wholeWord);
            var sb = new StringBuilder();

            foreach (var match in matches.Items)
            {
                sb.AppendLine(
                    String.Format(
                    "Found \"{0}\" in page: {1}, bounds: {2}", match.Text, match.Page, match.Location)
                );
            }

            searchResultLabel.Text = sb.ToString();
        }

    }
}
