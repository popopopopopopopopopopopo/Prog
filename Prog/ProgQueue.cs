using System;
using System.Collections.Generic;
using System.Text;

namespace Prog
{
    /// <summary> 進捗をリアルタイムで取得するための進捗オブジェクトです </summary>
    public class ProgQueue
    {
        public bool IsInitialSet { get; set; } = false;

        private int _myMinValue = 0;
        public int MinValue
        {
            get { return _myMinValue; }
            set { _myMinValue = value; }
        }

        private int _myMaxValue = 0;
        public int MaxValue
        {
            get { return _myMaxValue; }
            set { _myMaxValue = value; }
        }

        private int _myValue = 0;
        public int Value
        {
            get { return _myValue; }
            set { _myValue = value; }
        }

        private string _mySituation = "";
        public string Situation
        {
            get { return _mySituation; }
            set { _mySituation = value; }
        }

        public void Clear()
        {
            MinValue = 0;
            MaxValue = 0;
            Value = 0;
            Situation = "";
            IsInitialSet = false;
        }

        public void Initialize(int max, int min)
        {
            MinValue = max;
            MaxValue = min;
            Value = 0;
            Situation = "";
            IsInitialSet = true;
        }
    }
}
