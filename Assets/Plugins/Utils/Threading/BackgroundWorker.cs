using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using ThreadPriority = System.Threading.ThreadPriority;

namespace Utils.Threading {

public class BackgroundWorker
{
    private readonly Thread _backgroundWorkerThread;
    private readonly Queue<Action> _queue = new Queue<Action>();
    private readonly ManualResetEvent _workAvailable = new ManualResetEvent(false);

    public BackgroundWorker()
    {
        _backgroundWorkerThread = new Thread(BackgroundThread)
        {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal,
            Name = "BackgroundWorker Thread"
        };
        _backgroundWorkerThread.Start();
    }

    public bool IsAlive => _backgroundWorkerThread.IsAlive;
    public bool IsBackground => _backgroundWorkerThread.IsBackground;
    public bool IsThreadPoolThread => _backgroundWorkerThread.IsThreadPoolThread;
    
    public void EnqueueWork([NotNull] Action work)
    {
        if (work == null) throw new ArgumentNullException(nameof(work));
        
        lock (_queue)
        {
            _queue.Enqueue(work);
            _workAvailable.Set();
        }
    }

    private void BackgroundThread()
    {
        while (true)
        {
            _workAvailable.WaitOne();
            
            Action workItem;
            lock (_queue)
            {
                workItem = _queue.Dequeue();
                if (!_queue.Any())
                    _workAvailable.Reset();
            }

            try {
                workItem();
            }
            catch (Exception exception) {
                Debug.LogError($"Background Worker thread exception in method: {workItem.Method.Name}\n{exception}");
            }
        }
    }
}
 
} // Utils.Threading
