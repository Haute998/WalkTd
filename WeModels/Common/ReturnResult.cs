using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeModels
{
    public class RequestResult
    {
        private int _code;
        private object _data;
        private int _timestamp;
        private int _pages;
        private int _total;
        private string _message;
        private bool _success;

        public int code 
        {
            get 
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }
        public object data 
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        public int timestamp
        {
            get
            {
                if (_timestamp == 0)
                {
                    _timestamp = CommonFunc.GetNowTimestamp();
                }
                
                return _timestamp;
            }
            set
            {
                _timestamp = value;
            }
        }
        public int pages 
        {
            get {
                return _pages;
            }
            set {
                _pages = value;
            }
        }
        public int total {
            get {
                return _total;
            }
            set
            {
                _total = value;
            }
        }
        public string message {
            get {
                return _message;
            }
            set {
                _message = value;
            }
        }
        public bool success {
            get {
                return _success;
            }
            set {
                _success = value;
            }
        }
    }
}
