using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace PdfiumViewer.Test
{
    [TestFixture]
    public class MultiAppDomainFixture
    {
        [Test]
        public void MultipleAppDomains()
        {
            RunThreads();
        }

        [Test]
        public void MultipleAppDomainsAndCurrent()
        {
            using (var runner = new Runner())
            {
                runner.Run();

                RunThreads();
            }
        }

        private void RunThreads()
        {
            const int scripts = 10;
            const int iterations = 20;
            var threads = new List<Thread>();

            for (int i = 0; i < scripts; i++)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        for (int j = 0; j < iterations; j++)
                        {
                            using (var runner = new Runner())
                            {
                                runner.Run();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                });

                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private class Script : MarshalByRefObject
        {
            public void Run()
            {
                using (var stream = typeof(MultiAppDomainFixture).Assembly.GetManifestResourceStream(
                    typeof(MultiAppDomainFixture).Namespace + ".Example" + (new Random().Next(2) + 1) + ".pdf"
                ))
                {
                    var document = PdfDocument.Load(stream);

                    for (int i = 0; i < document.PageCount; i++)
                    {
                        using (document.Render(i, 96, 96, false))
                        {
                        }
                    }
                }
            }
        }

        private class Runner : IDisposable
        {
            private bool _disposed;
            private AppDomain _appDomain;

            public Runner()
            {
                _appDomain = AppDomain.CreateDomain(
                    "Unit test",
                    AppDomain.CurrentDomain.Evidence,
                    new AppDomainSetup
                    {
                        ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                        ApplicationName = "Unit test"
                    }
                );
            }

            public void Run()
            {
                var script = (Script)_appDomain.CreateInstanceAndUnwrap(
                    typeof(Script).Assembly.FullName,
                    typeof(Script).FullName
                );

                script.Run();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    if (_appDomain != null)
                    {
                        AppDomain.Unload(_appDomain);
                        _appDomain = null;
                    }

                    _disposed = true;
                }
            }
        }
    }
}
