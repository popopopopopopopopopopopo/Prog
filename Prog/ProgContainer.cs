using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prog
{
    /// <summary> コンバータの状況出力を行うためのコンテナです </summary>
    public class ProgContainer
    {
        private Lazy<IProgress<ProgQueue>> myParentProgress;
        public IProgress<ProgQueue> ParentProgress
        {
            get => myParentProgress.Value;
        }

        private Lazy<ProgQueue> myParentState = new Lazy<ProgQueue>(() => new ProgQueue(), LazyThreadSafetyMode.ExecutionAndPublication);
        public ProgQueue ParentState
        {
            get => myParentState.Value;
        }

        private Lazy<IProgress<ProgQueue>> myChildProgress;
        public IProgress<ProgQueue> ChildProgress
        {
            get => myChildProgress.Value;
        }

        private Lazy<ProgQueue> myChildState = new Lazy<ProgQueue>(() => new ProgQueue(), LazyThreadSafetyMode.ExecutionAndPublication);
        public ProgQueue ChildState
        {
            get => myChildState.Value;
        }

        private Action<ProgQueue> _onReportedParent = null;

        private Action<ProgQueue> _onReportedChild = null;

        /// <summary> コンストラクタ </summary>
        public ProgContainer()
        {
            myParentProgress =
                new Lazy<IProgress<ProgQueue>>(() => new Progress<ProgQueue>(p =>
                {
                    if (p.IsInitialSet)
                    {
                        ParentState.MaxValue = p.MaxValue;
                        ParentState.Value = p.Value;
                        ParentState.Situation = p.Situation;
                    }
                    else
                    {
                        //ParentState.MaxValue = ParentState.MaxValue;
                        ParentState.Value = ParentState.Value + p.Value;
                        ParentState.Situation = p.Situation;
                    }
                    //登録されているActionを実行します。
                    //Reportの度に、Redisに書き込みをしたり
                    //Mongoに書き込みをしたりするのに使用します。
                    _onReportedParent?.Invoke(ParentState);
                }), LazyThreadSafetyMode.ExecutionAndPublication);

            myChildProgress =
                new Lazy<IProgress<ProgQueue>>(() => new Progress<ProgQueue>(p =>
                {
                    if (p.IsInitialSet)
                    {
                        ChildState.MaxValue = p.MaxValue;
                        ChildState.Value = p.Value;
                        ChildState.Situation = p.Situation;
                    }
                    else
                    {
                        //ChildState.MaxValue = ChildState.MaxValue;
                        ChildState.Value = ChildState.Value + p.Value;
                        ChildState.Situation = p.Situation;
                    }
                    //登録されているActionを実行します。
                    //Reportの度に、Redisに書き込みをしたり
                    //Mongoに書き込みをしたりするのに使用します。
                    _onReportedChild?.Invoke(ChildState);
                }), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary> 状況表示開始時に、初期値を設定します </summary>
        /// <param name="parentMax"></param>
        /// <param name="parentValue"></param>
        public void SetParentInitialValue(int parentMax, int parentValue = 0)
        {
            ParentProgress.Report(new ProgQueue() { IsInitialSet = true, MaxValue = parentMax, Value = parentValue, Situation = "", });
        }

        public void SetChildInitialValue(int childMax, int childValue = 0)
        {
            ChildProgress.Report(new ProgQueue() { IsInitialSet = true, MaxValue = childMax, Value = childValue, Situation = "", });
        }

        /// <summary> 親の状況を進捗させます </summary>
        public void ReportToParent(int addcount = 1, string situation = "")
        {
            ParentProgress.Report(new ProgQueue() { Value = addcount, Situation = situation, });
        }

        public void ReportToParent(ProgQueue processing)
        {
            ParentProgress.Report(processing);
        }

        public async Task ReportToParentAsync(ProgQueue processing)
        {
            await Task.Run(() => ParentProgress.Report(processing));
        }

        /// <summary> 子の状況を進捗させます </summary>
        public void ReportToChild(int addcount = 1, string situation = "")
        {
            ChildProgress.Report(new ProgQueue() { Value = addcount, Situation = situation, });
        }

        public void ReportToChild(ProgQueue processing)
        {
            ChildProgress.Report(processing);
        }

        public async Task ReportToChildAsync(ProgQueue processing)
        {
            await Task.Run(() => ChildProgress.Report(processing));
        }

        /// <summary> 進捗通知後のアクションをセットします </summary>
        /// <param name="parentAction"></param>
        /// <param name="childAction"></param>
        public void SetReportedAction(Action<ProgQueue> parentAction, Action<ProgQueue> childAction)
        {
            _onReportedParent = parentAction;
            _onReportedChild = childAction;
        }

        /// <summary> 状況をクリアします </summary>
        public void Clear()
        {
            ParentState.Clear();
            ChildState.Clear();
        }
    }
}
