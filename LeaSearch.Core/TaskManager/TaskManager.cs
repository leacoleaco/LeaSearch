using System;

namespace LeaSearch.Core.TaskManager
{
    /// <summary>
    /// 管理定时任务的类
    /// </summary>
    public class TaskManager
    {
        private System.Timers.Timer _t;


        public TaskManager()
        {
            //每2个小时请求更新一次索引
            _t = new System.Timers.Timer(7200000);
            _t.Elapsed += new System.Timers.ElapsedEventHandler(UpdateIndexTask);
            _t.AutoReset = true;
        }

        public void StartTaskListen()
        {
            _t.Start();
        }

        public void UpdateIndexTask(object source, System.Timers.ElapsedEventArgs e)
        {
            OnUpdateIndexTaskActive();
        }

        public event Action UpdateIndexTaskActive;

        protected virtual void OnUpdateIndexTaskActive()
        {
            UpdateIndexTaskActive?.Invoke();
        }
    }
}
