using System.Collections.Generic;

namespace Dragon.Core
{
    public class InstancePool<T> where T : new()
    {
        private List<T> _outList = new List<T>();
        private List<T> _inList = new List<T>();

        public void Create(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _outList.Add(new T());
            }
        }

        public void Clear()
        {
            _outList.Clear();
            _inList.Clear();
        }

        public T Get()
        {
            if (_outList.Count == 0)
            {
                Create(1);
            }

            T ins = _outList[0];
            _outList.RemoveAt(0);
            _inList.Add(ins);
            return ins;
        }

        public void Return(T ins)
        {
            _inList.Remove(ins);
            _outList.Add(ins);
        }
    }
}