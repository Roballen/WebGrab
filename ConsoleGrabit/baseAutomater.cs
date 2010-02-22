using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ConsoleGrabit.Interfaces;
using ConsoleGrabit.Models;
using Utilities;

namespace ConsoleGrabit
{
    public abstract class BaseAutomater
    {
//        protected string _county;
//        protected string _username;
//        protected int _daysback;
//        protected string _password;

        protected Config _config;
        protected FileSystemWatcher _listener;
        protected IList<Lead> _leads;
        protected bool _waitingforimage;
        protected string _pdfstore;
        protected string _imagelocation;

        public string Imagelocation
        {
            get { return _imagelocation; }
            set { _imagelocation = value; }
        }

//        public string County
//        {
//            get { return _county; }
//            set { _county = value; }
//        }

        private CountyPull _pull;

        //protected WebconfigsConfig _config;

        protected BaseAutomater(WebconfigsConfig config)
        {

            _config = new Config
                          {
                              Checkperday = config.checksperday,
                              County = config.type,
                              Daysback = config.daysback,
                              Interval = config.interval,
                              Password = config.password,
                              Priority = config.priority,
                              Starttime = config.starttime,
                              Username = config.username
                          };


            _pdfstore = Properties.Settings.Default.pdfstore;

            _listener = new FileSystemWatcher(Properties.Settings.Default.javacachedirectory);
            _listener.NotifyFilter = NotifyFilters.FileName;
            _listener.IncludeSubdirectories = true;
            _listener.Filter = "";
            _leads = new List<Lead>();
            _waitingforimage = false;
        }

//        protected bool NeedtoCheck()
//        {
//            var interval = _config.intervalSpecified ? _config.interval : 8 / _config.checksperday; // will check equally throughout the day
//
//            if ( DateTime.Now > _config.starttime ) // if we are past the start time then start check
//            {
//                if (_pullstoday == 0 || _lastupdate == null) // if we haven't done it today go for it
//                    return true;
//                if ( _pullstoday < _config.checksperday && (DateTime.Now > ((DateTime)_lastupdate).AddHours(interval))) // if we have done it today, check how many time and the intervals we are to check
//                    return true;
//            }
//            return false;
//        }
        

        protected void AddListener(string location)
        {
            _listener.Path = location;
            AddListener();
        }
        protected void AddListener()
        {
            _listener.Created += ProcessDocument;
            _listener.EnableRaisingEvents = true;
        }

        protected void RemoveListener()
        {
            _listener.Created -= ProcessDocument;
            _listener.EnableRaisingEvents = false;
        }

        protected void ProcessDocument(object source, FileSystemEventArgs e)
        {
            try
            {
                // Specify what is done when a file is changed, created, or deleted.
                var ext = Path.GetExtension(e.FullPath);
                if (ext == ".idx") return;

                var scrubbedpath = e.FullPath.Replace("-temp", "");

                WaitForFile(scrubbedpath);

                Console.WriteLine("File: " + scrubbedpath + " " + e.ChangeType);
                //var imagetype = GetFileTypeFromIndex(e.FullPath);

                var to = Path.Combine(_pdfstore, Path.GetFileNameWithoutExtension(e.FullPath) + ".tif");

                FileTools.SCopyFile(scrubbedpath, to);

                _waitingforimage = false;
                _imagelocation = to;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected void WaitForFile(string fullPath)
        {
            while ( true )
            {
                try
                {
                    using (var stream = new StreamReader(fullPath))
                    {
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }

        protected  string GetFileTypeFromIndex(string indextargetfile)
        {
//            WaitForFile(indextargetfile + ".idx");
//            var filestring = FileTools.SReadFile(indextargetfile + ".idx");
            //just return tif for now, if we see something else we will activate this method
            return ".tif";
        }

        protected bool WaitForJavaApplet()
        {
            _waitingforimage = true;
            var found = false;
            long totalwait = 0;

            while (_waitingforimage && totalwait < (1000 * 45))
            {
                Thread.Sleep(1000);
                totalwait += 1000;
            }
            found = !_waitingforimage;
            _waitingforimage = false;
            return found;
        }

        protected void PerformOCR(ref Lead lead)
        {
            return;
        }

        protected string GetUrlFromJavaPopHref(string href)
        {
            int firstindex = href.IndexOf("'");
            int secondindex = href.IndexOf("'", firstindex + 1);
            var url = href.Substring(firstindex + 1, secondindex - firstindex);
            
            return url;
 }

    }
}
