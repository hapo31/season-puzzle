using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CommonCoroutine
{
    public delegate IEnumerator ThenFunc();
    public class CoroutineBase : MonoBehaviour, IThenable
    {
        private List<ThenFunc> TaskList = new List<ThenFunc>();
        public IThenable Then(ThenFunc f = null)
        {
            TaskList.Add(f);
            return this;
        }

        public void StartCoroutine()
        {
            StartCoroutine(RootCoroutine());
        }

        private IEnumerator RootCoroutine()
        {
            for (var i = 0; i < TaskList.Count; ++i)
            {
                yield return TaskList[i]?.Invoke();
            }
            TaskList.Clear();
        }
    }

    public interface IThenable
    {
        IThenable Then(ThenFunc f);
    }
}
