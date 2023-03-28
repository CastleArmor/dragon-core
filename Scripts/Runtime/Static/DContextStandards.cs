using System.Collections.Generic;

namespace Dragon.Core
{
    public static class DContextStandards
    {
        public static IContext GetRelativeAtAddress(List<DataKey> stack,IContext starting)
        {
            if (stack.Count == 0) return null;
            return RecursiveGetRelativeAtAddress(starting,stack, 0);
        }

        /// <summary>
        /// Returns only on initials.
        /// </summary>
        /// <param name="relationOwner"></param>
        /// <param name="stack"></param>
        /// <param name="currentIndex"></param>
        /// <returns></returns>
        private static IContext RecursiveGetRelativeAtAddress(IContext relationOwner,List<DataKey> stack, int currentIndex)
        {
            if (stack.Count <= currentIndex)
            {
                return null;
            }
            if (relationOwner.ContainsData<IContext>(stack[currentIndex].ID))
            {
                IContext main = relationOwner.GetData<IContext>(stack[currentIndex].ID);
                IContext nextOwner = main;
                IContext recurse = RecursiveGetRelativeAtAddress(nextOwner,stack, currentIndex + 1);
                if (recurse != null)
                {
                    main = recurse;
                }

                return main;
            }
            return null;
        }
    }
}