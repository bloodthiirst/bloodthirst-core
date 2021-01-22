using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;

namespace Bloodthirst.Core.ThreadProcessor
{
    public class ThreadCommandTest : MonoBehaviour
    {

        [SerializeField]
        private int a;

        [SerializeField]
        private int b;

        private int Add()
        {
            int cnt = 0;
            for (int i = 0; i < 5; i++)
            {
                cnt += i;
                Thread.Sleep(1000);
            }

            return cnt;
        }

        private void UseResutl(int result)
        {
            Debug.Log("result of addition is : " + result);

            transform.position += Vector3.up * result;
        }

        [Button]
        public void TestThread()
        {
            ThreadCommandFunc<int> addCommand = new ThreadCommandFunc<int>(Add, UseResutl);

            ThreadCommandProcessor.Append(addCommand);
        }
    }
}
