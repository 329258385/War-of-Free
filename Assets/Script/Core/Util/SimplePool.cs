using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solarmax
{
    public class IPoolObject<T> where T : IPoolObject<T>, new ()
    {
        private SimplePool<T> mAssociatedPool;

        public IPoolObject()
        {
            mAssociatedPool = null;
        }

        public void InitPool(SimplePool<T> associatedPool)
        {
            mAssociatedPool = associatedPool;
        }

        protected void Recycle(T t)
        {
            mAssociatedPool.Recycle(t);
        }

		public virtual void OnRecycle()
		{
			
		}
    }

    public class SimplePool<T> where T : IPoolObject<T>, new()
    {
        private object mAsyncLocker;
        private int mPoolSize;
		protected Queue<T> mFreeObjects; // modify private symbol to protected
		protected List<T> mBusyObjects; // modify private symbol to protected

        public SimplePool()
        {
            mAsyncLocker = new object();
            mPoolSize = 0;
            mFreeObjects = new Queue<T>();
            mBusyObjects = new List<T>();
        }

        public void Init(int size)
        {
            for (int i = 0; i < mPoolSize; ++i)
            {
                mFreeObjects.Enqueue(NewOne());
            }
        }

        private T NewOne()
        {
            T t = new T();
            if (null != t)
            {
                ++mPoolSize;
                t.InitPool(this);
            }

            return t;
        }

		private DerivedT NewOne<DerivedT>() where DerivedT : T, new ()
		{
			DerivedT t = new DerivedT();
			if (null != t)
			{
				++mPoolSize;
				t.InitPool(this);
			}

			return t;
		}

		public virtual T Alloc()
        {
			lock (mAsyncLocker)
            {
                T ret = default(T);

                if (mFreeObjects.Count > 0)
                {
                    ret = mFreeObjects.Dequeue();
                }
                else
                {
                    ret = NewOne();
                }

                mBusyObjects.Add(ret);
                return ret;
            }

        }

		public virtual DerivedT Alloc<DerivedT>() where DerivedT : T, new ()
		{
			lock (mAsyncLocker)
			{
				DerivedT ret = default(DerivedT);

				if (mFreeObjects.Count > 0)
				{
					ret = mFreeObjects.Dequeue() as DerivedT;
				}
				else
				{
					ret = NewOne<DerivedT>();
				}

				mBusyObjects.Add(ret);
				return ret;
			}
		}

		public virtual void Recycle(T t)
        {
			lock(mAsyncLocker)
            {
                if (null != t)
				{
					t.OnRecycle ();

					mBusyObjects.Remove(t);

                    mFreeObjects.Enqueue(t);

					--mPoolSize;
                }
            }
        }

        public int GetSize()
        {
            return mPoolSize;
        }

        public int GetFreeCount()
        {
            return mFreeObjects.Count;
        }

		public List<T> GetAllObjects()
		{
			List<T> ret = mFreeObjects.ToList ();
			ret.AddRange (mBusyObjects);
			return ret;
		}

		public void Clear()
		{
			mFreeObjects.Clear ();
			mBusyObjects.Clear ();
		}
    }
}
